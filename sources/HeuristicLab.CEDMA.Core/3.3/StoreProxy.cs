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
using HeuristicLab.Core;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel.Channels;

namespace HeuristicLab.CEDMA.Core {
  public class StoreProxy : IStore {
    private string serverUri;
    private IStore store;
    private NetTcpBinding binding;

    public StoreProxy(string serverUri) {
      this.serverUri = serverUri;
      Reset();
      Reconnect();
    }

    private void Reset() {
      binding = new NetTcpBinding();
      binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
      binding.MaxReceivedMessageSize = int.MaxValue;
      binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
      binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
    }

    private void Reconnect() {
      ChannelFactory<IStore> factory = new ChannelFactory<IStore>(binding);
      store = factory.CreateChannel(new EndpointAddress(serverUri));
    }

    private T ExecuteSavely<T>(Func<T> a ) {
      try {
        return a();
      }
      catch (CommunicationException) {
        Reconnect();
        return a();
      }
      catch (TimeoutException) {
        Reconnect();
        return a();
      }
    }

    #region IStore Members

    public void Add(Statement statement) {
      ExecuteSavely(() => { store.Add(statement); return 1.0; });
    }

    public ICollection<VariableBindings> Query(ICollection<Statement> query, int page, int pageSize) {
      return ExecuteSavely(() => store.Query(query, page, pageSize));
    }

    public ICollection<VariableBindings> Query(string query, int page, int pageSize) {
      return ExecuteSavely(() => store.Query(query, page, pageSize));
    }

    #endregion
  }
}
