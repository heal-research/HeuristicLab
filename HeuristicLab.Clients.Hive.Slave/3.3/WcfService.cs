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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive.SlaveCore {

  /// <summary>
  /// WcfService class is implemented as a singleton and works as a communication layer with the Hive server
  /// </summary>
  public class WcfService : MarshalByRefObject, IPluginProvider {
    private static WcfService instance;
    public DateTime ConnectedSince { get; private set; }
    public NetworkEnum.WcfConnState ConnState { get; private set; }

    /// <summary>
    /// Getter for the Instance of the WcfService
    /// </summary>
    /// <returns>the Instance of the WcfService class</returns>
    public static WcfService Instance {
      get {
        if (instance == null) {
          instance = new WcfService();
          HiveServiceLocator.Instance.Username = HeuristicLab.Clients.Hive.SlaveCore.Properties.Settings.Default.SlaveUser;
          HiveServiceLocator.Instance.Password = HeuristicLab.Clients.Hive.SlaveCore.Properties.Settings.Default.SlavePwd;
        }
        return instance;
      }
    }

    private WcfService() {
      ConnState = NetworkEnum.WcfConnState.Disconnected;
    }

    #region Task Methods
    public Task GetTask(Guid taskId) {
      return CallHiveService(s => s.GetTask(taskId));
    }

    public void UpdateTask(Task task) {
      CallHiveService(s => s.UpdateTask(task));
    }
    #endregion

    #region TaskData Methods
    public TaskData GetTaskData(Guid taskId) {
      return CallHiveService(s => s.GetTaskData(taskId));
    }

    /// <summary>
    /// Uploads the taskData and sets a new taskState (while correctly setting Transferring state)
    /// </summary>
    public void UpdateTaskData(Task task, TaskData taskData, Guid slaveId, TaskState state, string exception = "") {
      CallHiveService(service => {
        service.UpdateTask(task);
        task = service.UpdateTaskState(task.Id, TaskState.Transferring, slaveId, null, null);
        HiveClient.TryAndRepeat(() => {
          service.UpdateTaskData(task, taskData);
        }, HeuristicLab.Clients.Hive.SlaveCore.Properties.Settings.Default.PluginDeletionRetries, "Could not upload jobdata.");
        service.UpdateTaskState(task.Id, state, slaveId, null, exception);
      });
    }

    public Task UpdateJobState(Guid taskId, TaskState taskState, string exception) {
      return CallHiveService(s => s.UpdateTaskState(taskId, taskState, ConfigManager.Instance.GetClientInfo().Id, null, exception));
    }
    #endregion

    #region Heartbeat Methods
    public List<MessageContainer> SendHeartbeat(Heartbeat heartbeat) {
      return CallHiveService(s => s.Heartbeat(heartbeat));
    }
    #endregion

    #region Plugin Methods
    public Plugin GetPlugin(Guid id) {
      return CallHiveService(s => s.GetPlugin(id));
    }

    public IEnumerable<Plugin> GetPlugins() {
      return CallHiveService(s => s.GetPlugins());
    }

    public IEnumerable<PluginData> GetPluginDatas(List<Guid> pluginIds) {
      return CallHiveService(s => s.GetPluginDatas(pluginIds));
    }
    #endregion

    #region Events
    public event EventHandler Connected;
    private void OnConnected() {
      var handler = Connected;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception e) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Exception>(e));
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Connects with the server, registers the events and fires the Connected event.
    /// </summary>
    public void Connect(Slave slaveInfo) {
      CallHiveService(service => {
        ConnState = NetworkEnum.WcfConnState.Connected;
        ConnectedSince = DateTime.Now;
        service.Hello(slaveInfo);
        OnConnected();
      });
    }

    /// <summary>
    /// Disconnects the slave from the server
    /// </summary>
    public void Disconnect() {
      CallHiveService(service => {
        service.GoodBye(ConfigManager.Instance.GetClientInfo().Id);
        ConnState = NetworkEnum.WcfConnState.Disconnected;
      });
    }

    public int GetNewHeartbeatInterval(Guid id) {
      int ret = -1;
      CallHiveService(s => ret = s.GetNewHeartbeatInterval(id));
      return ret;
    }

    /// <summary>
    /// Network communication error handler.
    /// Every network error gets logged and the connection switches to faulted state
    /// </summary>
    private void HandleNetworkError(Exception e) {
      ConnState = NetworkEnum.WcfConnState.Failed;
      OnExceptionOccured(e);
    }

    public void CallHiveService(Action<IHiveService> call) {
      try {
        HiveServiceLocator.Instance.CallHiveService(call);
      }
      catch (Exception ex) {
        HandleNetworkError(ex);
      }
    }

    private T CallHiveService<T>(Func<IHiveService, T> call) {
      try {
        return HiveServiceLocator.Instance.CallHiveService(call);
      }
      catch (Exception ex) {
        HandleNetworkError(ex);
        return default(T);
      }
    }
    #endregion
  }
}