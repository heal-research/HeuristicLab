#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ServiceModel;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Clients.Hive.SlaveCore.ServiceContracts;
using System;

namespace HeuristicLab.Clients.Hive.SlaveCore {

  /// <summary>
  /// Singleton which encapsulates the SlaveCommunicationService
  /// </summary>
  public class SlaveClientCom {
    private static SlaveClientCom instance;
    private static DuplexChannelFactory<ISlaveCommunication> pipeFactory;
    public ISlaveCommunication ClientCom { get; set; }

    /// <summary>
    /// Getter for the Instance of the SlaveClientCom
    /// </summary>
    /// <returns>the Instance of the SlaveClientCom class</returns>
    public static SlaveClientCom Instance {
      get {
        if (instance == null) {
          instance = new SlaveClientCom();
        }
        return instance;
      }
    }

    public void LogMessage(string message) {
      try {
        ClientCom.LogMessage(message);
      }
      catch (Exception ex) {
        EventLogManager.LogMessage("Exception on LogMessage: " + ex.ToString() + Environment.NewLine + "Orginal message was: " + message);
      }
    }

    public void StatusChanged(StatusCommons status) {
      try {
        ClientCom.StatusChanged(status);
      }
      catch (Exception ex) {
        EventLogManager.LogMessage("Exception on StatusChanged: " + ex.ToString());
      }
    }

    private SlaveClientCom() {
      SetupClientCom();
    }

    private void SetupClientCom() {
      DummyListener dummy = new DummyListener();
      try {
        pipeFactory = new DuplexChannelFactory<ISlaveCommunication>(dummy, Settings.Default.SlaveCommunicationServiceEndpoint);
      }
      catch {
        EventLogManager.LogMessage("Couldn't create pipe for SlaveClientCom with config!");

        try {
          pipeFactory = new DuplexChannelFactory<ISlaveCommunication>(dummy, new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/HeuristicLabSlaveCom"));
        }
        catch {
          EventLogManager.LogMessage("Couldn't create pipe for SlaveClientCom with fixed addresse!");
          return;
        }
      }
      pipeFactory.Faulted += new System.EventHandler(pipeFactory_Faulted);

      ISlaveCommunication pipeProxy = pipeFactory.CreateChannel();
      ClientCom = pipeProxy;
    }

    void pipeFactory_Faulted(object sender, System.EventArgs e) {
      EventLogManager.LogMessage("SlaveClientCom just faulted. Trying to repair it...");

      try {
        pipeFactory.Faulted -= new System.EventHandler(pipeFactory_Faulted);
        SetupClientCom();
      }
      catch { }
    }

    public static void Close() {
      if (pipeFactory.State != CommunicationState.Closed && pipeFactory.State != CommunicationState.Faulted)
        pipeFactory.Close();
    }
  }
}