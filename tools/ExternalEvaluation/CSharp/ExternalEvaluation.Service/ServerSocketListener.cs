#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  public class ServerSocketListener : IListener {
    private int port;
    private IPAddress bindAddress;
    private TcpListener server;
    private Thread listenerThread;

    public ServerSocketListener(int port) {
      this.port = port;
      this.bindAddress = null;
    }

    public ServerSocketListener(int port, String ipAddress) {
      this.port = port;
      this.bindAddress = IPAddress.Parse(ipAddress);
    }

    public void Listen() {
      if (server == null) {
        if (bindAddress == null)
          server = new TcpListener(new IPEndPoint(IPAddress.Any, port));
        else
          server = new TcpListener(new IPEndPoint(bindAddress, port));
        server.Start();
        listenerThread = new Thread(this.ListenForClient);
        listenerThread.Start();
      }
    }

    private void ListenForClient() {
      while (true) {
        try {
          TcpClient client = server.AcceptTcpClient();
          StreamChannel streamChannel = new StreamChannel(client.GetStream(), client.GetStream());
          OnDiscovered(streamChannel);
        } catch (Exception) {
          try {
            Stop();
          } catch (ThreadAbortException) { }
          break;
        }
      }
    }

    public void Stop() {
      listenerThread.Abort();
      if (server != null) {
        server.Stop();
        server = null;
      }
    }

    public event EventHandler<EventArgs<IChannel>> Discovered;
    protected void OnDiscovered(IChannel channel) {
      var handler = Discovered;
      if (handler != null) Discovered(this, new EventArgs<IChannel>(channel));
    }
  }
}
