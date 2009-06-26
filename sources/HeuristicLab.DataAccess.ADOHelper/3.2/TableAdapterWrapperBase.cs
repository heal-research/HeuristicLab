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
using HeuristicLab.DataAccess.Interfaces;
using System.Data.Common;

namespace HeuristicLab.DataAccess.ADOHelper {
  public abstract class TableAdapterWrapperBase<AdapterT, ObjT, RowT>:
    ITableAdapterWrapper<AdapterT, ObjT, RowT>
    where AdapterT : new()
    where ObjT : new()
    where RowT : System.Data.DataRow {
    protected AdapterT adapter =
      new AdapterT();

    private Session session;
    
    #region IDataAdapterWrapper<AdapterT,ObjT,RowT> Members

    public AdapterT TransactionalAdapter {
      get {
        if (session == null)
          throw new Exception("No session set!");

        ITransaction trans =
          session.GetCurrentTransaction();
        if (trans != null)
          SetTransaction(trans.InnerTransaction as DbTransaction);
        else
          SetTransaction(null);

        return this.adapter; 
      }
    }

    public Session Session {
      protected get {
        return this.session;
      }

      set { 
        this.session = value;

        SetConnection(value.Connection); 
      }
    }

    public abstract void UpdateRow(RowT row);

    public abstract RowT InsertNewRow(ObjT obj);

    public abstract IEnumerable<RowT> FindById(Guid id);

    public abstract IEnumerable<RowT> FindAll();

    protected abstract void SetConnection(DbConnection connection);

    protected abstract void SetTransaction(DbTransaction transaction);

    #endregion
  }
}
