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
using System.Runtime.CompilerServices;
using System.Data;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  abstract class CachedDataAdapter<AdapterT, ObjT, RowT, CacheT> :
    DataAdapterBase<AdapterT, ObjT, RowT>, 
    ICachedDataAdapter
    where CacheT: System.Data.TypedTableBase<RowT>, new()
    where AdapterT : new()
    where RowT : System.Data.DataRow
    where ObjT : IHiveObject, new() {
    protected CacheT cache = 
      new CacheT();

    protected ICollection<ICachedDataAdapter> parentAdapters =
      new List<ICachedDataAdapter>();

    private DataTable temp = new DataTable();

    protected CachedDataAdapter() {
      FillCache();

      ServiceLocator.GetTransactionManager().OnUpdate +=
        new EventHandler(CachedDataAdapter_OnUpdate);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual RowT FindSingleRow(Selector dbSelector, 
      Selector cacheSelector) {
      RowT row =
         FindSingleRow(cacheSelector);

      //not in cache
      if (row == null) {
        row =
          FindSingleRow(dbSelector);
      }

      return row;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual IEnumerable<RowT> FindMultipleRows(Selector dbSelector,
        Selector cacheSelector) {
      IList<RowT> result =
         new List<RowT>(cacheSelector());

      IEnumerable<RowT> result2 =
          dbSelector();

      foreach (RowT row in result2) {
        if (!IsCached(row)) {
          result.Add(row);
        }
      }

      return result;
    }

    protected virtual ObjT FindSingle(Selector dbSelector,
      Selector cacheSelector) {
      RowT row = FindSingleRow(dbSelector, cacheSelector);

      if (row != null) {
        ObjT obj = new ObjT();
        Convert(row, obj);

        return obj;
      } else {
        return default(ObjT);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual ICollection<ObjT> FindMultiple(Selector dbSelector,
      Selector cacheSelector) {
      ICollection<ObjT> result =
        FindMultiple(cacheSelector);

      ICollection<ObjT> resultDb =
        FindMultiple(dbSelector);

      foreach (ObjT obj in resultDb) {
        if (!result.Contains(obj))
          result.Add(obj);
      }

      return result;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract RowT InsertNewRowInCache(ObjT obj);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract void FillCache();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public abstract void SyncWithDb();

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract bool PutInCache(ObjT obj);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract RowT FindCachedById(long id);

    [MethodImpl(MethodImplOptions.Synchronized)]
    void CachedDataAdapter_OnUpdate(object sender, EventArgs e) {
      foreach (ICachedDataAdapter parent in this.parentAdapters) {
        parent.SyncWithDb();
      }

      this.SyncWithDb();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual void RemoveRowFromCache(RowT row) {      
      cache.Rows.Remove(row);
    }

    protected virtual bool IsCached(RowT row) {
      if (row == null)
        return false;
     else
        return FindCachedById((long)row[row.Table.PrimaryKey[0]]) != null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected override RowT GetRowById(long id) {
      RowT row =
        FindCachedById(id);
      
      if(row == null)
        row = FindSingleRow(
          delegate() {
            return FindById(id);
          });

      return row;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Update(ObjT obj) {
      if (obj != null) {
        RowT row = 
          GetRowById(obj.Id);

        if (row == null) {
          if (PutInCache(obj)) {
            row = InsertNewRowInCache(obj);
          } else {
            row = InsertNewRow(obj);
          }

          UpdateRow(row);
        }

        obj.Id = (long)row[row.Table.PrimaryKey[0]];

        Convert(obj, row);

        if (!IsCached(row))
          UpdateRow(row);
        
        if (IsCached(row) &&
            !PutInCache(obj)) {
          //remove from cache
          temp.ImportRow(row);

          UpdateRow(row);
          RemoveRowFromCache(row);
        } else if (!IsCached(row) &&
          PutInCache(obj)) {
          //add to cache
          cache.ImportRow(row);

          row.Table.Rows.Remove(row);
        }
      }
    }
  }
}
