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
using HeuristicLab.PluginInfrastructure;
using System.Data.Common;
using System.Threading;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class Session: ISession {
    private static DiscoveryService discoveryService =
      new DiscoveryService();

    private SessionFactory factory;

    private Transaction transaction;

    private DbConnection connection;

    private Thread ownerThread;

    private int counter;

    private IDictionary<Guid, object> adapters =
      new Dictionary<Guid, object>();

    public Session(SessionFactory factory) {
      this.factory = factory;
      this.ownerThread = Thread.CurrentThread;
      this.counter = 0;
    }

    public void CheckThread() {
      if (!Thread.CurrentThread.Equals(ownerThread)) {
        throw new Exception("Session is owned by another thread");
      }
    }

    public DbConnection Connection {
      get {
        if (connection == null) {
          connection = factory.CreateConnection();
        }

        return connection;
      }
    }

    public void DetachTrasaction() {
      this.transaction = null;
    }

    public void IncrementCounter() {
      this.counter++;
    }

    #region ISession Members
    public ITransaction BeginTransaction() {
      CheckThread();

      if (transaction == null) {
         transaction = new Transaction(this);
         transaction.Connection = Connection;
      }

      return transaction;
    }

    public ITransaction GetCurrentTransaction() {
      CheckThread();

      return transaction;
    }

    public IDataAdapter<ObjT> GetDataAdapter<ObjT>()
      where ObjT : IPersistableObject {
      CheckThread();

      Guid adapterId = typeof(IDataAdapter<ObjT>).GUID;

      if (!adapters.ContainsKey(adapterId)) {
        IDataAdapter<ObjT> adapter =
          discoveryService.GetInstances<IDataAdapter<ObjT>>()[0];

        adapter.Session = this;

        adapters.Add(adapterId, adapter);
      }

      return adapters[adapterId] as IDataAdapter<ObjT>;
    }

    public T GetDataAdapter<ObjT, T>()
      where ObjT : IPersistableObject
      where T : class, IDataAdapter<ObjT>
    {
      CheckThread();

      Guid adapterId = typeof(T).GUID;

      if (!adapters.ContainsKey(adapterId)) {
        T adapter =
          discoveryService.GetInstances<T>()[0];

        adapter.Session = this;

        adapters.Add(adapterId, adapter);
      }

      return adapters[adapterId] as T;
    }

    public void EndSession() {
      this.counter--;

      if (counter <= 0) {
        CheckThread();

        if (transaction != null) {
          transaction.Rollback();
          transaction = null;
        }
        if (connection.State == System.Data.ConnectionState.Open) {
          connection.Close();
          connection = null;
        }
        factory.EndSession(this);
      }
    }

    #endregion
  }
}
