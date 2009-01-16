#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Data;
using System.Threading;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  abstract class CachedDataAdapter<AdapterT, ObjT, RowT, CacheT> :
    DataAdapterBase<AdapterT, ObjT, RowT>,
    ICachedDataAdapter
    where CacheT : System.Data.TypedTableBase<RowT>, new()
    where AdapterT : new()
    where RowT : System.Data.DataRow
    where ObjT : IHiveObject, new() {
    protected static CacheT cache =
      new CacheT();

    private static bool cacheFilled = false;

    private static ReaderWriterLock cacheLock =
      new ReaderWriterLock();

    protected DataTable dataTable =
      new DataTable();

    protected ICollection<ICachedDataAdapter> parentAdapters =
      new List<ICachedDataAdapter>();

    protected CachedDataAdapter() {
      cacheLock.AcquireWriterLock(Timeout.Infinite);

      if (!cacheFilled) {
        FillCache();
        cacheFilled = true;
      }

      cacheLock.ReleaseWriterLock();

      ServiceLocator.GetTransactionManager().OnUpdate +=
        new EventHandler(CachedDataAdapter_OnUpdate);
    }

    protected virtual RowT FindSingleRow(Selector dbSelector,
      Selector cacheSelector) {
      cacheLock.AcquireReaderLock(Timeout.Infinite);
      
      RowT row =
         FindSingleRow(cacheSelector);

      //not in cache
      if (row == null) {
        row =
          FindSingleRow(dbSelector);
      }

      cacheLock.ReleaseReaderLock();

      return row;
    }

    protected virtual IEnumerable<RowT> FindMultipleRows(Selector dbSelector,
        Selector cacheSelector) {
      cacheLock.AcquireReaderLock(Timeout.Infinite);

      IList<RowT> result =
         new List<RowT>(cacheSelector());

      IEnumerable<RowT> result2 =
          dbSelector();

      foreach (RowT row in result2) {
        if (!IsCached(row)) {
          result.Add(row);
        }
      }

      cacheLock.ReleaseReaderLock();

      return result;
    }

    protected virtual ObjT FindSingle(Selector dbSelector,
      Selector cacheSelector) {
      ObjT obj = default(ObjT);

      cacheLock.AcquireReaderLock(Timeout.Infinite);

      RowT row = FindSingleRow(dbSelector, cacheSelector);

      if (row != null) {
        obj = new ObjT();
        obj = Convert(row, obj);
      }

      cacheLock.ReleaseReaderLock();

      return obj;
    }

    protected virtual ICollection<ObjT> FindMultiple(Selector dbSelector,
      Selector cacheSelector) {
      cacheLock.AcquireReaderLock(Timeout.Infinite);

      ICollection<ObjT> result =
        FindMultiple(cacheSelector);

      ICollection<ObjT> resultDb =
        FindMultiple(dbSelector);

      cacheLock.ReleaseReaderLock();

      foreach (ObjT obj in resultDb) {
        if (!result.Contains(obj))
          result.Add(obj);
      }

      return result;
    }

    protected abstract RowT InsertNewRowInCache(ObjT obj);

    protected abstract void FillCache();

    protected abstract void SynchronizeWithDb();

    protected abstract bool PutInCache(ObjT obj);

    protected abstract RowT FindCachedById(long id);

    public void SyncWithDb() {
      foreach (ICachedDataAdapter parent in this.parentAdapters) {        
        parent.SyncWithDb();
      }

      cacheLock.AcquireReaderLock(Timeout.Infinite);

      this.SynchronizeWithDb();

      cacheLock.ReleaseReaderLock();
    }

    void CachedDataAdapter_OnUpdate(object sender, EventArgs e) {
      this.SyncWithDb();
    }

    protected virtual bool IsCached(RowT row) {
      if (row == null)
        return false;
      else {
        cacheLock.AcquireReaderLock(Timeout.Infinite);

        bool cached = FindCachedById((long)row[row.Table.PrimaryKey[0]]) != null;

        cacheLock.ReleaseReaderLock();

        return cached;
      }
    }

    protected override RowT GetRowById(long id) {
      cacheLock.AcquireReaderLock(Timeout.Infinite);

      RowT row =
        FindCachedById(id);

      //not in cache
      if (row == null)
        row = FindSingleRow(
          delegate() {
            return FindById(id);
          });

      cacheLock.ReleaseReaderLock();

      return row;
    }

    private void AddToCache(RowT row) {
      cacheLock.AcquireWriterLock(Timeout.Infinite);

      cache.ImportRow(row);
      row.Table.Rows.Remove(row);

      cacheLock.ReleaseWriterLock();
    }

    private RowT AddToCache(ObjT obj) {
      cacheLock.AcquireWriterLock(Timeout.Infinite);

      RowT row =  InsertNewRowInCache(obj);

      cacheLock.ReleaseWriterLock();

      return row;
    }

    private void RemoveRowFromCache(RowT row) {
      cacheLock.AcquireWriterLock(Timeout.Infinite);

      dataTable.ImportRow(row);
      cache.Rows.Remove(row);

      cacheLock.ReleaseWriterLock();

      UpdateRow(row);
    }

    public override void Update(ObjT obj) {
      if (obj != null) {
        RowT row =
          GetRowById(obj.Id);

        if (row == null) {
          if (PutInCache(obj)) {
            row = AddToCache(obj);
          } else {
            row = InsertNewRow(obj);
          }

          UpdateRow(row);
        }

        obj.Id = (long)row[row.Table.PrimaryKey[0]];
        LockRow(obj.Id);

        ConvertObj(obj, row);

        if (!IsCached(row))
          UpdateRow(row);

        if (IsCached(row) &&
            !PutInCache(obj)) {          
          RemoveRowFromCache(row);
        } else if (!IsCached(row) &&
          PutInCache(obj)) {
          AddToCache(row);
        }

        UnlockRow(obj.Id);
      }
    }
  }
}

