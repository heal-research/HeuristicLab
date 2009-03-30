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

namespace HeuristicLab.DataAccess.ADOHelper {
  public class Session: ISession {
    private static DiscoveryService discoveryService =
      new DiscoveryService();

    private SessionFactory factory;

    private ITransactionManager transManager;

    private DbConnection connection;

    private IDictionary<Guid, object> adapters =
      new Dictionary<Guid, object>();

    public Session(SessionFactory factory, 
      ITransactionManager transManager) {
      this.factory = factory;
      this.transManager = transManager;
    }

    public DbConnection Connection {
      get {
        if (connection == null) {
          connection = factory.CreateConnection();
        }

        return connection;
      }
    }

    #region ISession Members
    public ITransaction BeginTransaction() {
      ITransaction trans = transManager.BeginTransaction();
      if(trans is Transaction)
        ((Transaction)trans).Connection = Connection;

      return trans;
    }

    public ITransaction GetTransactionForCurrentThread() {
      ITransaction trans = transManager.GetTransactionForCurrentThread();
      if (trans != null && trans is Transaction)
        ((Transaction)trans).Connection = Connection;

      return trans;
    }

    public IDataAdapter<ObjT> GetDataAdapter<ObjT>()
      where ObjT : IPersistableObject {
      lock(this) {
        Guid adapterId = typeof(IDataAdapter<ObjT>).GUID;

        if (!adapters.ContainsKey(adapterId)) {
          IDataAdapter<ObjT> adapter =
            discoveryService.GetInstances<IDataAdapter<ObjT>>()[0];

          adapter.Session = this;

          adapters.Add(adapterId, adapter);
        }

        return adapters[adapterId] as IDataAdapter<ObjT>;
      }
    }

    public T GetDataAdapter<ObjT, T>()
      where ObjT : IPersistableObject
      where T : class, IDataAdapter<ObjT>
    {
      lock (this) {
        Guid adapterId = typeof(T).GUID;

        if (!adapters.ContainsKey(adapterId)) {
          T adapter =
            discoveryService.GetInstances<T>()[0];

          adapter.Session = this;

          adapters.Add(adapterId, adapter);
        }

        return adapters[adapterId] as T;
      }
    }

    public void EndSession() {
      if(connection.State == System.Data.ConnectionState.Open)
        connection.Close();
      factory.EndSession(this);
    }

    #endregion
  }
}
