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

    protected IDictionary<RowT, DataTable> parentTable =
      new Dictionary<RowT, DataTable>();

    protected ICollection<ICachedDataAdapter> parentAdapters =
      new List<ICachedDataAdapter>();

    protected CachedDataAdapter() {
      FillCache();

      ServiceLocator.GetTransactionManager().OnUpdate +=
        new EventHandler(CachedDataAdapter_OnUpdate);
    }

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

    protected abstract RowT InsertNewRowInCache(ObjT obj);

    protected abstract void FillCache();

    public abstract void SyncWithDb();

    protected abstract bool PutInCache(ObjT obj);

    protected abstract RowT FindCachedById(long id);

    void CachedDataAdapter_OnUpdate(object sender, EventArgs e) {
      foreach (ICachedDataAdapter parent in this.parentAdapters) {
        parent.SyncWithDb();
      }

      this.SyncWithDb();
    }

    protected virtual void RemoveRowFromCache(RowT row) {
      if (parentTable.ContainsKey(row)) {
        parentTable[row].Rows.Add(row);
        parentTable.Remove(row);
      }
      
      cache.Rows.Remove(row);
    }

    protected virtual bool IsCached(RowT row) {
      return cache.Contains<RowT>(row);
    }

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

        Convert(obj, row);

        if (IsCached(row) &&
            !PutInCache(obj)) {
          //remove from cache
          RemoveRowFromCache(row);
          UpdateRow(row);
        } else if (!IsCached(row) &&
          PutInCache(obj)) {
          //add to cache
          if (row.Table != null) {
            parentTable[row] = row.Table;
            row.Table.Rows.Remove(row);
          }
          cache.Rows.Add(row);
        }

        obj.Id = (long)row[row.Table.PrimaryKey[0]];

        if (!IsCached(row))
          UpdateRow(row);
      }
    }
  }
}
