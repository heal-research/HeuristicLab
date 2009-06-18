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
using System.Text;
using System.ServiceModel;
using HeuristicLab.Tracing;
using System.Threading;
using HeuristicLab.Core;

namespace HeuristicLab.Grid {
  public class GridServerProxy : IGridServer {
    private const int MAX_CONNECTION_RETRIES = 10;
    private const int RETRY_TIMEOUT_SEC = 60;
    private string address;
    private object connectionLock = new object();
    private ChannelFactory<IGridServer> factory;
    private IGridServer server;

    public GridServerProxy(string address) {
      this.address = address;
    }

    public JobState JobState(Guid guid) {
      return SavelyExecute(() => server.JobState(guid));
    }

    public Guid BeginExecuteEngine(byte[] engine) {
      return SavelyExecute(() => server.BeginExecuteEngine(engine));
    }

    public byte[] TryEndExecuteEngine(Guid guid) {
      return SavelyExecute(() => server.TryEndExecuteEngine(guid));
    }

    private void ResetConnection() {
      Logger.Info("Reset connection in JobManager");
      lock (connectionLock) {
        // open a new channel
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;

        factory = new ChannelFactory<IGridServer>(binding);
        server = factory.CreateChannel(new EndpointAddress(address));
      }
    }

    private TResult SavelyExecute<TResult>(Func<TResult> a) {
      int retries = 0;
      do {
        try {
          lock (connectionLock) {
            return a();
          }
        }
        catch (TimeoutException) {
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
        catch (CommunicationException) {
          ResetConnection();
          retries++;
          Thread.Sleep(TimeSpan.FromSeconds(RETRY_TIMEOUT_SEC));
        }
      } while (retries < MAX_CONNECTION_RETRIES);
      Logger.Warn("Reached max connection retries");
      return default(TResult);
    }
  }
}
