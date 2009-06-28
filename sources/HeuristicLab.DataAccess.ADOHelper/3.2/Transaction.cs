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

    private Session session;

    private int usageCounter = 0;

    private TransactionIsolationLevel isolationLevel;

    public Transaction(Session session, TransactionIsolationLevel isolationLevel) {
      this.session = session;
      this.isolationLevel = isolationLevel;
    }

    public void IncrementUsageCounter() {
      this.usageCounter++;
    }

    public DbConnection Connection {
      set {
        if (value != null &&
          (transaction == null ||
           !(transaction.Connection != null &&
             transaction.Connection.Equals(value)))) {
          if (value.State != System.Data.ConnectionState.Open)
            value.Open();

          if (isolationLevel == TransactionIsolationLevel.Default)
            transaction = value.BeginTransaction(IsolationLevel.ReadCommitted);
          else if (isolationLevel == TransactionIsolationLevel.ReadUncommitted)
            transaction = value.BeginTransaction(IsolationLevel.ReadUncommitted);
          else if (isolationLevel == TransactionIsolationLevel.ReadCommitted)
            transaction = value.BeginTransaction(IsolationLevel.ReadCommitted);
          else if (isolationLevel == TransactionIsolationLevel.RepeatableRead)
            transaction = value.BeginTransaction(IsolationLevel.RepeatableRead);
          else if (isolationLevel == TransactionIsolationLevel.Serializable)
            transaction = value.BeginTransaction(IsolationLevel.Serializable);
          else
            transaction = value.BeginTransaction(IsolationLevel.ReadCommitted);
        }
      }
    }

    #region ITransaction Members
    public void Commit() {
      this.session.CheckThread();

      usageCounter--;

      if (transaction != null && usageCounter <= 0) {
        DbConnection conn =
          transaction.Connection;

        transaction.Commit();

        transaction = null;
        session.DetachTrasaction();

        if (conn != null && 
            conn.State == System.Data.ConnectionState.Open)
          conn.Close();
      }
    }

    public void Rollback() {
      this.session.CheckThread();

      usageCounter = 0;

      if (transaction != null) {
        DbConnection conn =
            transaction.Connection;

        transaction.Rollback();

        transaction = null;
        session.DetachTrasaction();

        if (conn != null &&
            conn.State == System.Data.ConnectionState.Open)
          conn.Close();
      }
    }

    public object InnerTransaction {
      get {
        this.session.CheckThread();

        return transaction;
      }
    }

    #endregion
  }
}
