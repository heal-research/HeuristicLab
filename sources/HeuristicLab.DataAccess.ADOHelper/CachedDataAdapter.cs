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
using System.Text;
using System.Data;
using System.Threading;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.DataAccess.ADOHelper {
  public abstract class CachedDataAdapter<AdapterT, ObjT, RowT, CacheT> :
    DataAdapterBase<AdapterT, ObjT, RowT>,
    ICachedDataAdapter
    where CacheT : System.Data.TypedTableBase<RowT>, new()
    where AdapterT : new()
    where RowT : System.Data.DataRow
    where ObjT : IPersistableObject, new() {
    protected static CacheT cache =
      new CacheT();

    private static bool cacheFilled = false;

    private static ReaderWriterLockSlim cacheLock =
      new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    protected ICollection<ICachedDataAdapter> parentAdapters =
      new List<ICachedDataAdapter>();

    protected CachedDataAdapter(IDBSynchronizer synchronizer) {
      cacheLock.EnterWriteLock();

      if (!cacheFilled) {
        FillCache();
        cacheFilled = true;
      }

      cacheLock.ExitWriteLock();

      synchronizer.OnUpdate +=
        new EventHandler(CachedDataAdapter_OnUpdate);
    }

    protected virtual RowT FindSingleRow(Selector dbSelector,
      Selector cacheSelector) {
      cacheLock.EnterReadLock();
      
      RowT row =
         FindSingleRow(cacheSelector);

      //not in cache
      if (row == null) {
        row =
          FindSingleRow(dbSelector);
      }

      cacheLock.ExitReadLock();

      return row;
    }

    protected virtual IEnumerable<RowT> FindMultipleRows(Selector dbSelector,
        Selector cacheSelector) {
      cacheLock.EnterReadLock();

      IList<RowT> result =
         new List<RowT>(cacheSelector());

      IEnumerable<RowT> result2 =
          dbSelector();

      foreach (RowT row in result2) {
        if (!IsCached(row)) {
          result.Add(row);
        }
      }

      cacheLock.ExitReadLock();

      return result;
    }

    protected virtual ObjT FindSingle(Selector dbSelector,
      Selector cacheSelector) {
      ObjT obj = default(ObjT);

      cacheLock.EnterReadLock();

      RowT row = FindSingleRow(dbSelector, cacheSelector);

      if (row != null) {
        obj = new ObjT();
        obj = Convert(row, obj);
      }

      cacheLock.ExitReadLock();

      return obj;
    }

    protected virtual ICollection<ObjT> FindMultiple(Selector dbSelector,
      Selector cacheSelector) {
      cacheLock.EnterReadLock();

      ICollection<ObjT> result =
        FindMultiple(cacheSelector);

      ICollection<ObjT> resultDb =
        FindMultiple(dbSelector);

      cacheLock.ExitReadLock();

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

      cacheLock.EnterReadLock();

      this.SynchronizeWithDb();

      cacheLock.ExitReadLock();
    }

    void CachedDataAdapter_OnUpdate(object sender, EventArgs e) {
      this.SyncWithDb();
    }

    protected virtual bool IsCached(RowT row) {
      if (row == null)
        return false;
      else {
        cacheLock.EnterReadLock();

        bool cached = FindCachedById((long)row[row.Table.PrimaryKey[0]]) != null;

        cacheLock.ExitReadLock();

        return cached;
      }
    }

    protected override RowT GetRowById(long id) {
      cacheLock.EnterReadLock();

      RowT row =
        FindCachedById(id);

      //not in cache
      if (row == null)
        row = FindSingleRow(
          delegate() {
            return FindById(id);
          });

      cacheLock.ExitReadLock();

      return row;
    }

    private void AddToCache(RowT row) {
      cacheLock.EnterWriteLock();

      cache.ImportRow(row);
      row.Table.Rows.Remove(row);

      cacheLock.ExitWriteLock();
    }

    private RowT AddToCache(ObjT obj) {
      cacheLock.EnterWriteLock();

      RowT row =  InsertNewRowInCache(obj);

      cacheLock.ExitWriteLock();

      return row;
    }

    private void RemoveRowFromCache(RowT row) {
      cacheLock.EnterWriteLock();

      UpdateRow(row);

      row.Table.Rows.Remove(row);     

      cacheLock.ExitWriteLock();
    }

    public override void Update(ObjT obj) {
      if (obj != null) {
        RowT row = null;
        long locked = default(long);

        if (obj.Id != default(long)) {
          LockRow(obj.Id);
          locked = obj.Id;

          row = GetRowById(obj.Id);
        }

        if (row == null) {
          if (PutInCache(obj)) {
            row = AddToCache(obj);
          } else {
            row = InsertNewRow(obj);
          }

          UpdateRow(row);
          obj.Id = (long)row[row.Table.PrimaryKey[0]];
        }

        if (locked == default(long)) {
          LockRow(obj.Id);
          locked = obj.Id;
        }

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

        UnlockRow(locked);
      }
    }
  }
}

