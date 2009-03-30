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
using System.Threading;
using System.Timers;
using System.Data.Common;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class SessionFactory: ISessionFactory {
    private IDictionary<Thread, ISession> sessions =
      new Dictionary<Thread, ISession>();

    private ITransactionManager transManager =
          new TransactionManager();

    public Type DbConnectionType { get;  set; }
    public String DbConnectionString { get;  set; }

    public SessionFactory() {
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
        ICollection<Thread> toBeRemoved =
          new List<Thread>();

        foreach (Thread t in sessions.Keys) {
          if (t.ThreadState == ThreadState.Stopped) {
            toBeRemoved.Add(t);
          }
        }

        foreach (Thread t in toBeRemoved) {
          sessions.Remove(t);
        }
      }
    }

    public DbConnection CreateConnection() {
      DbConnection conn = 
        Activator.CreateInstance(DbConnectionType) as DbConnection;

      conn.ConnectionString = DbConnectionString;

      return conn;
    }

    #region ISessionFactory Members

    public ISession GetSessionForCurrentThread() {
      lock (this) {
        Thread current = Thread.CurrentThread;

        if (!sessions.ContainsKey(current)) {
          sessions[current] =
            new Session(this, transManager);
        }

        return sessions[current];
      }
    }

    public void EndSession(ISession session) {
      lock (this) {
        ICollection<Thread> toBeRemoved =
          new List<Thread>();

        foreach(Thread t in sessions.Keys) {
          if(sessions[t].Equals(session)) {
            toBeRemoved.Add(t);
          }
        }

        foreach (Thread t in toBeRemoved) {
          sessions.Remove(t);
        }
      }
    }

    #endregion
  }
}
