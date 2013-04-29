#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Transactions;

namespace HeuristicLab.Services.Hive.DataAccess {
  public class TransactionManager : ITransactionManager {
    public void UseTransaction(Action call, bool repeatableRead = false, bool longRunning = false) {
      int n = 10;
      while (n > 0) {
        TransactionScope transaction = CreateTransaction(repeatableRead, longRunning);
        try {
          call();
          transaction.Complete();
          n = 0;
        }
        catch (System.Data.SqlClient.SqlException e) {
          n--; // probably deadlock situation, let it roll back and repeat the transaction n times
          LogFactory.GetLogger(typeof(TransactionManager).Namespace).Log(string.Format("Exception occured, repeating transaction {0} more times. Details: {1}", n, e.ToString()));
          if (n <= 0) throw;
        }
        finally {
          transaction.Dispose();
        }
      }
    }

    public T UseTransaction<T>(Func<T> call, bool repeatableRead = false, bool longRunning = false) {
      int n = 10;
      while (n > 0) {
        TransactionScope transaction = CreateTransaction(repeatableRead, longRunning);
        try {
          T result = call();
          transaction.Complete();
          n = 0;
          return result;
        }
        catch (System.Data.SqlClient.SqlException e) {
          n--; // probably deadlock situation, let it roll back and repeat the transaction n times
          LogFactory.GetLogger(typeof(TransactionManager).Namespace).Log(string.Format("Exception occured, repeating transaction {0} more times. Details: {1}", n, e.ToString()));
          if (n <= 0) throw;
        }
        finally {
          transaction.Dispose();
        }
      }
      throw new Exception("This code should not be reached");
    }

    private TransactionScope CreateTransaction(bool repeatableRead, bool longRunning) {
      var options = new TransactionOptions();
      if (repeatableRead)
        options.IsolationLevel = IsolationLevel.RepeatableRead;
      else
        options.IsolationLevel = IsolationLevel.ReadUncommitted;

      if (longRunning)
        options.Timeout = Settings.Default.LongRunningDatabaseCommandTimeout;

      return new TransactionScope(TransactionScopeOption.Required, options);
    }
  }
}
