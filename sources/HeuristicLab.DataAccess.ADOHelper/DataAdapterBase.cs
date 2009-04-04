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

    private ISession session;

    private IDataAdapterWrapper<AdapterT, ObjT, RowT> dataAdapter;

    private static Mutex lockersMutex =
      new Mutex();

    private static IDictionary<object, Mutex> lockers =
      new Dictionary<object, Mutex>();

    private static IDictionary<object, int> lockCount =
      new Dictionary<object, int>();

    protected DataAdapterBase(
      IDataAdapterWrapper<AdapterT, ObjT, RowT> dataAdapter) {
      this.dataAdapter = dataAdapter;
    }

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
        return dataAdapter.TransactionalAdapter;
      }
    }

    protected IDataAdapterWrapper<AdapterT, ObjT, RowT> DataAdapterWrapper {
      get {
        return dataAdapter;
      }
    }

    public ISession Session {
      get {
        return this.session;
      }

      set {
        if (!(value is Session))
          throw new Exception("Can only bind to ADO session");

        this.session = value;
        this.dataAdapter.Session = value as Session;
      }
    }

    public object InnerAdapter {
      get {
        return this.Adapter;
      }
    }

    #region Abstract methods
    protected abstract RowT ConvertObj(ObjT obj, RowT row);

    protected abstract ObjT ConvertRow(RowT row, ObjT obj);
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

    protected RowT FindSingleRow(Selector selector) {
      RowT row = default(RowT);

      IEnumerable<RowT> found =
        selector();

      if (found.Count<RowT>() == 1)
        row = found.First<RowT>();

      return row;
    }

    protected ObjT FindSingle(Selector selector) {
      ITransaction trans =
       session.GetCurrentTransaction();
      bool transactionExists = trans != null;
      if (!transactionExists) {
        trans = session.BeginTransaction();
      }

      try {
        RowT row = FindSingleRow(selector);

        ObjT result;
        if (row != null) {
          ObjT obj = new ObjT();
          obj = Convert(row, obj);

          result = obj;
        } else {
          result = default(ObjT);
        }

        return result;
      }
      finally {
        if (!transactionExists && trans != null) {
          trans.Commit();
        }
      }
    }

    protected ICollection<ObjT> FindMultiple(Selector selector) {
      ITransaction trans =
       session.GetCurrentTransaction();
      bool transactionExists = trans != null;
      if (!transactionExists) {
        trans = session.BeginTransaction();
      }

      try {
        IEnumerable<RowT> found =
          selector();

        IList<ObjT> result =
          new List<ObjT>();

        foreach (RowT row in found) {
          ObjT obj = new ObjT();
          obj = Convert(row, obj);
          if (obj != null)
            result.Add(obj);
        }

        return result;
      }
      finally {
        if (!transactionExists && trans != null) {
          trans.Commit();
        }
      }     
    }

    protected virtual RowT GetRowById(Guid id) {
      return FindSingleRow(
        delegate() {
          return dataAdapter.FindById(id);
        });
    }

    protected virtual void doUpdate(ObjT obj) {
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
          row = dataAdapter.InsertNewRow(obj);
        }

        if (locked == Guid.Empty) {
          LockRow(obj.Id);
          locked = obj.Id;
        }

        ConvertObj(obj, row);
        dataAdapter.UpdateRow(row);

        UnlockRow(locked);
      }
    }

    public void Update(ObjT obj) {
      ITransaction trans =
        session.GetCurrentTransaction();
      bool transactionExists = trans != null;
      if (!transactionExists) {
        trans = session.BeginTransaction();
      }

      try {
        doUpdate(obj);
      }
      finally {
        if (!transactionExists && trans != null) {
          trans.Commit();
        }
      }
    }

    public virtual ObjT GetById(Guid id) {
      return FindSingle(delegate() {
        return dataAdapter.FindById(id);
      });
    }

    public virtual ICollection<ObjT> GetAll() {
      return new List<ObjT>(
        FindMultiple(
          new Selector(dataAdapter.FindAll)));
    }

    protected virtual bool doDelete(ObjT obj) {
      bool success = false;

      if (obj != null) {
        LockRow(obj.Id);

        RowT row =
          GetRowById(obj.Id);

        if (row != null) {
          row.Delete();
          dataAdapter.UpdateRow(row);

          success = true;
        }

        UnlockRow(obj.Id);
      }

      return success;
    }

    public bool Delete(ObjT obj) {
      ITransaction trans =
        session.GetCurrentTransaction();
      bool transactionExists = trans != null;
      if (!transactionExists) {
        trans = session.BeginTransaction();
      }

      try {
        return doDelete(obj);
      }
      finally {
        if (!transactionExists && trans != null) {
          trans.Commit();
        }
      }  
    }
  }
}

