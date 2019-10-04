#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using System.Management;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;


namespace HeuristicLab.Clients.Hive.SlaveCore {
  /// <summary>
  /// accesses the server and sends his data (uuid, uptimes, hardware config) 
  /// </summary>
  public class ConfigManager {
    private static ConfigManager instance = null;
    private const string vmwareNameString = "VMware";
    private const string virtualboxNameString = "VirtualBox";
    private const int macLength = 6;
    private const int macLongLength = 8;

    public static ConfigManager Instance {
      get { return instance; }
      set { instance = value; }
    }

    /// <summary>
    /// if Asleep is true, the Slave won't accept any new jobs
    /// </summary>
    public bool Asleep { get; set; }
    private TaskManager jobManager;
    private Slave slave;
    private PerformanceCounter cpuCounter;
    private PerformanceCounter memCounter;

    /// <summary>
    /// Constructor for the singleton, must recover Guid, Calendar, ...
    /// </summary>
    public ConfigManager(TaskManager jobManager) {
      this.jobManager = jobManager;
      cpuCounter = new PerformanceCounter();
      cpuCounter.CategoryName = "Processor";
      cpuCounter.CounterName = "% Processor Time";
      cpuCounter.InstanceName = "_Total";
      memCounter = new PerformanceCounter("Memory", "Available Bytes", true);

      Asleep = false;
      slave = new Slave();
      slave.Id = GetUniqueMachineId();
      slave.Name = Environment.MachineName;
      if (Settings.Default.NrOfCoresToScavenge < 1 || Settings.Default.NrOfCoresToScavenge > Environment.ProcessorCount) {
        slave.Cores = Environment.ProcessorCount;
      } else {
        slave.Cores = Settings.Default.NrOfCoresToScavenge;
      }
      slave.Memory = GetPhysicalMemory();
      slave.CpuArchitecture = Environment.Is64BitOperatingSystem ? CpuArchitecture.x64 : CpuArchitecture.x86;
      slave.OperatingSystem = Environment.OSVersion.VersionString;
      slave.CpuSpeed = GetCpuSpeed();
      slave.IsDisposable = true;

      UpdateSlaveInfo();
    }

    private void UpdateSlaveInfo() {
      if (slave != null) {
        slave.FreeMemory = GetFreeMemory();
        slave.HbInterval = (int)Settings.Default.HeartbeatInterval.TotalSeconds;
      }
    }

    /// <summary>
    /// Get all the Information about the client
    /// </summary>
    /// <returns>the ClientInfo object</returns>
    public Slave GetClientInfo() {
      UpdateSlaveInfo();
      return slave;
    }

    public int GetFreeCores() {
      return slave.Cores.HasValue ? slave.Cores.Value - SlaveStatusInfo.UsedCores : 0;
    }

    /// <summary>
    /// collects and returns information that get displayed by the Client Console
    /// </summary>
    /// <returns></returns>
    public StatusCommons GetStatusForClientConsole() {
      StatusCommons st = new StatusCommons();
      st.ClientGuid = slave.Id;

      st.Status = WcfService.Instance.ConnState;
      st.ConnectedSince = WcfService.Instance.ConnectedSince;

      st.TotalCores = slave.Cores.HasValue ? slave.Cores.Value : 0;
      st.FreeCores = GetFreeCores();
      st.Asleep = this.Asleep;

      st.JobsStarted = SlaveStatusInfo.TasksStarted;
      st.JobsAborted = SlaveStatusInfo.TasksAborted;
      st.JobsFinished = SlaveStatusInfo.TasksFinished;
      st.JobsFetched = SlaveStatusInfo.TasksFetched;
      st.JobsFailed = SlaveStatusInfo.TasksFailed;

      st.Jobs = jobManager.GetExecutionTimes().Select(x => new TaskStatus { TaskId = x.Key, ExecutionTime = x.Value }).ToList();

      return st;
    }

    public Dictionary<Guid, TimeSpan> GetExecutionTimeOfAllJobs() {
      Dictionary<Guid, TimeSpan> prog = new Dictionary<Guid, TimeSpan>();
      try {
        prog = jobManager.GetExecutionTimes();
      } catch (Exception ex) {
        SlaveClientCom.Instance.LogMessage(string.Format("Exception was thrown while trying to get execution times: {0}", ex.Message));
      }
      return prog;
    }

    /// <summary>
    /// Returns the unique machine id of the slave
    /// </summary>
    /// <returns><see cref="Guid"/></returns>
    public static Guid GetUniqueMachineId() {
      // Due to the fact that repeated calculation of the machine ID can lead to a client registering at 
      // the Hive server multiple times with different IDs it's better to set the unique machine id only 
      // once, at first startup, and store it in core settings. 
      if (Settings.Default.MachineId == Guid.Empty) {
        Settings.Default.MachineId = Guid.NewGuid();
        Settings.Default.Save();
      }

      return Settings.Default.MachineId;
    }

    /// <summary>
    /// returns total physical memory of the machine in MB
    /// </summary>
    private static int? GetPhysicalMemory() {
      long? res = GetWMIValue("Win32_ComputerSystem", "TotalPhysicalMemory");
      if (res != null)
        return (int)(res / 1024 / 1024);
      else
        return null;
    }

    /// <summary>
    /// returns CPU frequence of the machine in Mhz
    /// </summary>
    private static int? GetCpuSpeed() {
      return (int)GetWMIValue("Win32_Processor", "MaxClockSpeed");
    }

    private static long? GetWMIValue(string clazz, string property) {
      ManagementClass mgtClass = new ManagementClass(clazz);
      ManagementObjectCollection mgtCol = mgtClass.GetInstances();

      foreach (ManagementObject mgtObj in mgtCol) {
        foreach (var prop in mgtObj.Properties) {
          if (prop.Value != null && prop.Name == property) {
            try {
              return long.Parse(prop.Value.ToString());
            } catch {
              return null;
            }
          }
        }
      }
      return null;
    }

    /// <summary>
    /// returns free memory of machine in MB
    /// </summary>    
    public int GetFreeMemory() {
      int mb = 0;

      try {
        mb = (int)(memCounter.NextValue() / 1024 / 1024);
      } catch { }
      return mb;
    }

    public float GetCpuUtilization() {
      float cpuVal = 0.0F;

      try {
        return cpuCounter.NextValue();
      } catch { }
      return cpuVal;
    }
  }
}
