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
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Client.ExecutionEngine;
using HeuristicLab.Hive.Client.Core.ClientConsoleService;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Core.Properties;

namespace HeuristicLab.Hive.Client.Core.ConfigurationManager {
  /// <summary>
  /// accesses the Server and sends his data (uuid, uptimes, hardware config) 
  /// </summary>
  public class ConfigManager {
    private static ConfigManager instance = null;
    public static ConfigManager Instance {
      get {
        if (instance == null) {
          instance = new ConfigManager();
        }
        return instance;
      }
    }

    public Core Core { get; set; }    
    private ClientInfo hardwareInfo;        

    /// <summary>
    /// Constructor for the singleton, must recover Guid, Calendar, ...
    /// </summary>
    private ConfigManager() {
      //retrive GUID from XML file, or burn in hell. as in hell. not heaven.
      //this won't work this way. We need a plugin for XML Handling.      
      hardwareInfo = new ClientInfo();      

      if (Settings.Default.Guid == Guid.Empty)
        hardwareInfo.ClientId = Guid.NewGuid();
      else
        hardwareInfo.ClientId = Settings.Default.Guid;
      
      hardwareInfo.NrOfCores = Environment.ProcessorCount;
      hardwareInfo.Memory = 1024;
      hardwareInfo.Name = Environment.MachineName;
    }

    public ClientInfo GetClientInfo() {
      return hardwareInfo;          
    }

    public ConnectionContainer GetServerIPAndPort() {
      ConnectionContainer cc = new ConnectionContainer();
      cc.IPAdress = Settings.Default.ServerIP;
      cc.Port = Settings.Default.ServerPort;
      return cc;
    }

    public void SetServerIPAndPort(ConnectionContainer cc) {
      Settings.Default.ServerIP = cc.IPAdress;
      Settings.Default.ServerPort = cc.Port;
    }

    public StatusCommons GetStatusForClientConsole() {
      StatusCommons st = new StatusCommons();
      st.ClientGuid = hardwareInfo.ClientId;
      
      st.Status = WcfService.Instance.ConnState;
      st.ConnectedSince = WcfService.Instance.ConnectedSince;

      st.JobsAborted = ClientStatusInfo.JobsAborted;
      st.JobsDone = ClientStatusInfo.JobsProcessed;
      st.JobsFetched = ClientStatusInfo.JobsFetched;      

      Dictionary<long, Executor> engines = Core.GetExecutionEngines();
      st.Jobs = new List<JobStatus>();
      foreach (KeyValuePair<long, Executor> kvp in engines) {
        Executor e = kvp.Value;
        st.Jobs.Add(new JobStatus { JobId = e.JobId, Progress = e.Progress, Since = e.CreationTime });
      }
      return st;      
    }
  }
}
