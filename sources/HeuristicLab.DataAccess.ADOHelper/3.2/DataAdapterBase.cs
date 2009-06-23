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

    private ITableAdapterWrapper<AdapterT, ObjT, RowT> dataAdapter;

    protected DataAdapterBase(
      ITableAdapterWrapper<AdapterT, ObjT, RowT> dataAdapter) {
      this.dataAdapter = dataAdapter;
    }

    protected AdapterT Adapter {
      get {
        return dataAdapter.TransactionalAdapter;
      }
    }

    protected ITableAdapterWrapper<AdapterT, ObjT, RowT> DataAdapterWrapper {
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
    protected delegate object TransactionalAction();

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
      return 
        (ObjT)doInTransaction(
         delegate() {
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
        });
    }

    private ICollection<ObjT> FindMultiple(Selector selector, 
      int from, int size) {
      return (ICollection<ObjT>)doInTransaction(
        delegate() {
          IEnumerable<RowT> found =
            selector();

          if (from > 0 && size > 0)
            found = found.Skip<RowT>(from).Take<RowT>(size);

          IList<ObjT> result =
            new List<ObjT>();

          foreach (RowT row in found) {
            ObjT obj = new ObjT();
            obj = Convert(row, obj);
            if (obj != null)
              result.Add(obj);
          }

          return result;
        });   
    }

    protected ICollection<ObjT> FindMultiple(Selector selector) {
      return FindMultiple(
        selector, 0, -1);
    }

    protected virtual RowT GetRowById(Guid id) {
      return FindSingleRow(
        delegate() {
          return dataAdapter.FindById(id);
        });
    }

    protected object doInTransaction(TransactionalAction action) {
      ITransaction trans =
        session.GetCurrentTransaction();
      bool transactionExists = trans != null;
      if (!transactionExists) {
        trans = session.BeginTransaction();
      }

      try {
        bool result = (bool)action();

        if (!transactionExists && trans != null) {
            trans.Commit();
        }

        return result;
      }
      catch (Exception e) {
        if (!transactionExists && trans != null) {
          trans.Rollback();
        }

        throw e;
      }
    }

    protected virtual void doUpdate(ObjT obj) {
      if (obj != null) {
        RowT row = null;

        if (obj.Id != Guid.Empty) {
          row = GetRowById(obj.Id);
        } else {
          obj.Id = Guid.NewGuid();
        }

        if (row == null) {
          row = dataAdapter.InsertNewRow(obj);
        }

        ConvertObj(obj, row);
        dataAdapter.UpdateRow(row);
      }
    }

    public void Update(ObjT obj) {
      try {
        doInTransaction(
          delegate() {
            doUpdate(obj);
            return true;
          });        
      }
      catch (DBConcurrencyException ex) {
        DataRow row = ex.Row;

        RowT current = GetRowById(obj.Id);
        if (current != null) {
          //find out changes
          for (int i = 0; i < row.ItemArray.Length; i++) {            
            if (!row[i, DataRowVersion.Current].Equals(
                 row[i, DataRowVersion.Original])) {
              current[i] = row[i];
            }
          }

          ConvertRow(current, obj);
          //try updating again
          Update(obj);
        }
        //otherwise: row was deleted in the meantime - nothing to do
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

    public virtual ICollection<ObjT> GetAll(int from, int size) {
      //note - this base implementation is inefficient,
      //consider overriding the implementation 
      //in derived adapters (based on a query (SQL LIMIT))
      return new List<ObjT>(
        FindMultiple(
          new Selector(dataAdapter.FindAll), 
          from, size));
    }

    protected virtual bool doDelete(ObjT obj) {
      bool success = false;

      if (obj != null) {
        RowT row =
          GetRowById(obj.Id);

        if (row != null) {
          row.Delete();
          dataAdapter.UpdateRow(row);

          success = true;
        }
      }

      return success;
    }

    public bool Delete(ObjT obj) {
      try {
        return (bool)doInTransaction(
          delegate() {
            return doDelete(obj);
          });
      }
      catch (DBConcurrencyException) {
        RowT current = GetRowById(obj.Id);
        if (current != null) {
          ConvertRow(current, obj);
          //try deleting again
          return Delete(obj);
        } else {
          //row has already been deleted
          return false;
        }
      }
    }
  }
}

