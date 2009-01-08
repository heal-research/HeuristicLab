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

namespace HeuristicLab.Hive.Server.ADODataAccess {
  abstract class DataAdapterBase<AdapterT, ObjT, RowT>
    where AdapterT: new() 
    where RowT: System.Data.DataRow
    where ObjT: IHiveObject, new() {
    protected AdapterT adapter =
      new AdapterT();

    #region Abstract methods
    protected abstract RowT Convert(ObjT obj, RowT row);

    protected abstract ObjT Convert(RowT row, ObjT obj);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract RowT InsertNewRow(ObjT obj);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract void UpdateRow(RowT row);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract IEnumerable<RowT> FindById(long id);

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected abstract IEnumerable<RowT> FindAll();
    #endregion

    protected delegate IEnumerable<RowT> Selector();

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual RowT FindSingleRow(Selector selector) {
      RowT row = default(RowT);

      IEnumerable<RowT> found =
        selector();

      if (found.Count<RowT>() == 1)
        row = found.First<RowT>();

      return row;
    }

    protected virtual ObjT FindSingle(Selector selector) {
      RowT row = FindSingleRow(selector);

      if (row != null) {
        ObjT obj = new ObjT();
        Convert(row, obj);

        return obj;
      } else {
        return default(ObjT);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    protected virtual ICollection<ObjT> FindMultiple(Selector selector) {
      IEnumerable<RowT> found =
        selector();

      IList<ObjT> result =
        new List<ObjT>();

      foreach (RowT row in found) {
        ObjT obj = new ObjT();
        Convert(row, obj);
        result.Add(obj);
      }

      return result;
    }

    protected virtual RowT GetRowById(long id) {
      return FindSingleRow(
        delegate() {
          return FindById(id);
        });
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void Update(ObjT obj) {
      if (obj != null) {
        RowT row =
          GetRowById(obj.Id);

        if (row == null) {
          row = InsertNewRow(obj);
        }

        Convert(obj, row);

        UpdateRow(row);

        obj.Id = (long)row[row.Table.PrimaryKey[0]];
      }
    }

    public virtual ObjT GetById(long id) {
      return FindSingle(delegate() {
        return FindById(id);
      });
    }

    public virtual ICollection<ObjT> GetAll() {
      return new List<ObjT>(
        FindMultiple(
          new Selector(FindAll)));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual bool Delete(ObjT obj) {
      if (obj != null) {
        RowT row =
          GetRowById(obj.Id);

        if (row != null) {
          row.Delete();
          UpdateRow(row);

          return true;
        }
      }

      return false;
    }
  }
}
