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
using HeuristicLab.Hive.Contracts;

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
    private ClientDto hardwareInfo;        

    /// <summary>
    /// Constructor for the singleton, must recover Guid, Calendar, ...
    /// </summary>
    private ConfigManager() {      
      hardwareInfo = new ClientDto();

      if (Settings.Default.Guid == Guid.Empty) {
        hardwareInfo.Id = Guid.NewGuid();
        Settings.Default.Guid = hardwareInfo.Id;
        Settings.Default.Save();
      } else
        hardwareInfo.Id = Settings.Default.Guid;
      
      hardwareInfo.NrOfCores = Environment.ProcessorCount;
      hardwareInfo.Memory = 1024;
      hardwareInfo.Name = Environment.MachineName;
    }

    /// <summary>
    /// Get all the Information about the client
    /// </summary>
    /// <returns>the ClientInfo object</returns>
    public ClientDto GetClientInfo() {
      hardwareInfo.Login = WcfService.Instance.ConnectedSince;
      return hardwareInfo;          
    }

    /// <summary>
    /// Returns the connectioncontainer with the IP and Port, storred in the Settings framework
    /// </summary>
    /// <returns>The IP and Port of the server</returns>
    public ConnectionContainer GetServerIPAndPort() {
      ConnectionContainer cc = new ConnectionContainer();
      cc.IPAdress = Settings.Default.ServerIP;
      cc.Port = Settings.Default.ServerPort;
      return cc;
    }

    /// <summary>
    /// Sets and saves IP and Port in the Settings framework
    /// </summary>
    /// <param name="cc"></param>
    public void SetServerIPAndPort(ConnectionContainer cc) {
      Settings.Default.ServerIP = cc.IPAdress;
      Settings.Default.ServerPort = cc.Port;
      Settings.Default.Save();
    }
    /// <summary>
    /// collects and returns information that get displayed by the Client Console
    /// </summary>
    /// <returns></returns>
    public StatusCommons GetStatusForClientConsole() {
      //Todo: Locking
      StatusCommons st = new StatusCommons();
      st.ClientGuid = hardwareInfo.Id;
      
      st.Status = WcfService.Instance.ConnState;
      st.ConnectedSince = WcfService.Instance.ConnectedSince;

      st.JobsAborted = ClientStatusInfo.JobsAborted;
      st.JobsDone = ClientStatusInfo.JobsProcessed;
      st.JobsFetched = ClientStatusInfo.JobsFetched;      

      Dictionary<Guid, Executor> engines = Core.GetExecutionEngines();
      st.Jobs = new List<JobStatus>();

      lock (engines) {
        foreach (KeyValuePair<Guid, Executor> kvp in engines) {
          Executor e = kvp.Value;
          st.Jobs.Add(new JobStatus { JobId = e.JobId, Progress = e.Progress, Since = e.CreationTime });
        }
      }
      return st;      
    }

    public Dictionary<Guid, double> GetProgressOfAllJobs() {
      Dictionary<Guid, double> prog = new Dictionary<Guid, double>();
      Dictionary<Guid, Executor> engines = Core.GetExecutionEngines();
      lock (engines) {
        foreach (KeyValuePair<Guid, Executor> kvp in engines) {
          Executor e = kvp.Value;
          //if (!e.JobIsFinished) 
          prog[e.JobId] = e.Progress;
        }
      }
      return prog;
    }

    public int GetUsedCores() {
      Dictionary<Guid, Executor> engines = Core.GetExecutionEngines();
      Dictionary<Guid, JobDto> jobs = Core.GetJobs();
      int usedCores = 0;
      lock (engines) {        
        foreach (KeyValuePair<Guid, JobDto> kvp in jobs)
          usedCores += kvp.Value.CoresNeeded;
      }
      return usedCores;
    }

  }
}
