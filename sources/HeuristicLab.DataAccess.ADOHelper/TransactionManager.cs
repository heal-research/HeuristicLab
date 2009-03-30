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
using System.Timers;
using HeuristicLab.DataAccess.Interfaces;
using System.Threading;
using System.Data;

namespace HeuristicLab.DataAccess.ADOHelper {
  class TransactionManager: ITransactionManager {
    private IDictionary<Thread, ITransaction> transactions =
      new Dictionary<Thread, ITransaction>();

    public TransactionManager() {
      System.Timers.Timer timer =
        new System.Timers.Timer();

      TimeSpan interval =
        new TimeSpan(0, 1, 0);

      timer.Interval = interval.TotalMilliseconds;
      timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
      timer.Start();
    }

    void timer_Elapsed(object sender, ElapsedEventArgs e) {
      lock (this) {
        ICollection<Thread> tobeRemoved =
          new List<Thread>();

        foreach (Thread t in transactions.Keys) {
          if (t.ThreadState == ThreadState.Stopped) {
            try {
              transactions[t].Rollback();
            }
            catch (Exception ex) {
              //TODO: Error handling
            }
            finally {
              tobeRemoved.Add(t);
            }
          }
        }

        foreach (Thread t in tobeRemoved) {
          transactions.Remove(t);
        }
      }
    }

    #region ITransactionManager Members
    public ITransaction BeginTransaction() {
      lock (this) {
        Thread current = Thread.CurrentThread;

        if (!transactions.ContainsKey(current)) {
          ITransaction trans = new Transaction(this);
          transactions[current] = trans;
        }

        return transactions[current];
      }
    }

    public ITransaction GetTransactionForCurrentThread() {
      lock (this) {
        Thread current = Thread.CurrentThread;

        if(transactions.ContainsKey(current))
          return transactions[current];
        else
          return null;
      }
    }

    internal void RemoveTransaction(ITransaction trans) {
      lock (this) {
        ICollection<Thread> tobeRemoved =
          new List<Thread>();

        foreach (Thread t in transactions.Keys) {
          if (transactions[t].Equals(trans)) {
            tobeRemoved.Add(t);
          }
        }

        foreach (Thread t in tobeRemoved) {
          transactions.Remove(t);
        }
      }
    }

    #endregion
  }
}
