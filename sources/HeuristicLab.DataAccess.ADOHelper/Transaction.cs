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
using System.Data;
using System.Data.Common;

namespace HeuristicLab.DataAccess.ADOHelper {
  class Transaction: ITransaction {
    private DbTransaction transaction;
    private TransactionManager manager;

    #region ITransaction Members

    public Transaction(TransactionManager manager) {
      this.manager = manager;
    }

    public DbConnection Connection {
      set {
        if (value != null &&
          (transaction == null ||
           !(transaction.Connection != null && 
             transaction.Connection.Equals(value)))) {
            if (value.State != System.Data.ConnectionState.Open)
              value.Open();

            transaction = value.BeginTransaction(IsolationLevel.RepeatableRead);
        }
      }
    }

    public void Commit() {
      manager.RemoveTransaction(this);
      if (transaction != null) {
        DbConnection conn =
          transaction.Connection;

        transaction.Commit();

        if (conn != null && 
            conn.State == System.Data.ConnectionState.Open)
          conn.Close();
      }
    }

    public void Rollback() {
      manager.RemoveTransaction(this);
      DbConnection conn =
          transaction.Connection;

      transaction.Rollback();

      if (conn != null &&
          conn.State == System.Data.ConnectionState.Open)
        conn.Close();
    }

    public object InnerTransaction {
      get {
        return transaction;
      }
    }

    #endregion
  }
}
