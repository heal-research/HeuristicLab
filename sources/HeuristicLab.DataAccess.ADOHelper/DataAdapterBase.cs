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
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.DataAccess.ADOHelper {
  public abstract class DataAdapterBase<AdapterT, ObjT, RowT>
    where AdapterT : new()
    where RowT : System.Data.DataRow
    where ObjT : IPersistableObject, new() {

    private static Mutex lockersMutex =
      new Mutex();

    private static IDictionary<object, Mutex> lockers =
      new Dictionary<object, Mutex>();

    private static IDictionary<object, int> lockCount =
      new Dictionary<object, int>();

    protected void LockRow(object id) {
      Mutex rowLock = null;

      /////begin critical section////
      lockersMutex.WaitOne();

      if (!lockers.ContainsKey(id)) {
        lockers[id] = new Mutex();
        lockCount[id] = 0;
      }
      rowLock = lockers[id];
      lockCount[id]++;
      
      lockersMutex.ReleaseMutex();
      /////end critical section////

      rowLock.WaitOne();
    }

    protected void UnlockRow(object id) {
      Mutex rowLock = lockers[id];
      rowLock.ReleaseMutex();

      /////begin critical section////
      lockersMutex.WaitOne();

      lockCount[id]--;
      if (lockCount[id] == 0)
        lockers.Remove(id);

      lockersMutex.ReleaseMutex();
      /////end critical section////
    }
    
    protected AdapterT Adapter {
      get {
        return new AdapterT();
      }
    }

    #region Abstract methods
    protected abstract RowT ConvertObj(ObjT obj, RowT row);

    protected abstract ObjT ConvertRow(RowT row, ObjT obj);

    protected abstract RowT InsertNewRow(ObjT obj);

    protected abstract void UpdateRow(RowT row);

    protected abstract IEnumerable<RowT> FindById(Guid id);

    protected abstract IEnumerable<RowT> FindAll();
    #endregion

    protected delegate IEnumerable<RowT> Selector();

    protected ObjT Convert(RowT row, ObjT obj) {
      try {
        obj = ConvertRow(row, obj);
        return obj;
      }
      catch (DeletedRowInaccessibleException) {
        return default(ObjT);
      }
      catch (RowNotInTableException) {
        return default(ObjT);
      }
    }

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
        obj = Convert(row, obj);

        return obj;
      } else {
        return default(ObjT);
      }
    }

    protected virtual ICollection<ObjT> FindMultiple(Selector selector) {
      IEnumerable<RowT> found =
        selector();

      IList<ObjT> result =
        new List<ObjT>();

      foreach (RowT row in found) {
        ObjT obj = new ObjT();
        obj = Convert(row, obj);
        if(obj != null)
          result.Add(obj);
      }

      return result;
    }

    protected virtual RowT GetRowById(Guid id) {
      return FindSingleRow(
        delegate() {
          return FindById(id);
        });
    }

    public virtual void Update(ObjT obj) {
      if (obj != null) {
        RowT row = null;
        Guid locked = Guid.Empty;

        if (obj.Id != Guid.Empty) {
          LockRow(obj.Id);
          locked = obj.Id;

          row = GetRowById(obj.Id);
        } else {
          obj.Id = Guid.NewGuid();
        }

        if (row == null) {
          row = InsertNewRow(obj);
          UpdateRow(row);
        }

        if (locked == Guid.Empty) {
          LockRow(obj.Id);
          locked = obj.Id;
        }

        ConvertObj(obj, row);
        UpdateRow(row);

        UnlockRow(locked);
      }
    }

    public virtual ObjT GetById(Guid id) {
      return FindSingle(delegate() {
        return FindById(id);
      });
    }

    public virtual ICollection<ObjT> GetAll() {
      return new List<ObjT>(
        FindMultiple(
          new Selector(FindAll)));
    }

    public virtual bool Delete(ObjT obj) {
      bool success = false;
      
      if (obj != null) {
        LockRow(obj.Id);

        RowT row =
          GetRowById(obj.Id);

        if (row != null) {
          row.Delete();
          UpdateRow(row);

          success = true;
        }

        UnlockRow(obj.Id);
      }

      return success;
    }
  }
}

