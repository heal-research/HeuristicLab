#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.ServiceModel;
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.ServiceContracts;

namespace HeuristicLab.Clients.Hive.SlaveCore.ConsoleClient {

  /// <summary>
  /// Mock a client, simply print out messages
  /// </summary>
  [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
  public class SlaveCommListener : ISlaveCommunicationCallbacks, IDisposable {
    private ISlaveCommunication pipeProxy;
    private DuplexChannelFactory<ISlaveCommunication> pipeFactory;
    private StreamWriter debugOutput;

    public void Open() {
      pipeFactory = new DuplexChannelFactory<ISlaveCommunication>(this, "SlaveCommunicationServiceEndpoint");
      debugOutput = new StreamWriter("slave-debug.txt");
      debugOutput.AutoFlush = true;

      while (!ReconnectToSlaveCore()) {
        Thread.Sleep(700);
      }
    }

    public bool ReconnectToSlaveCore() {
      try {
        pipeProxy = pipeFactory.CreateChannel();
        pipeProxy.Subscribe();
        return true;
      }
      catch (Exception) {
        OnMessageLogged("Couldn't connect to Slave Core. Waiting for the Core to start.");
        return false;
      }
    }

    public void Close() {
      if (pipeFactory.State != CommunicationState.Closed &&
          pipeFactory.State != CommunicationState.Closing &&
          pipeFactory.State != CommunicationState.Faulted) {
        pipeProxy.Unsubscribe();
      }
      try {
        pipeFactory.Close();
      }
      catch (Exception) {
        pipeFactory.Abort();
      }
      debugOutput.Close();
    }

    public void OnStatusChanged(StatusCommons status) {
      string msg = string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss"), status);
      Console.WriteLine(msg);
      debugOutput.WriteLine(msg);
    }

    public void OnMessageLogged(string message) {
      string msg = string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss"), message);
      Console.WriteLine(msg);
      debugOutput.WriteLine(msg);
    }

    public void OnShutdown() {
      Console.WriteLine("SlaveCommListner: Slave quit");
    }

    public void Dispose() {
      Close();
    }
  }
}
