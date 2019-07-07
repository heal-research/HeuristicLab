#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ServiceModel;
using HeuristicLab.Clients.Hive.SlaveCore.ServiceContracts;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  [ServiceBehavior(UseSynchronizationContext = false)]
  public class SlaveCommunicationService : ISlaveCommunication {
    private static List<ISlaveCommunicationCallbacks> subscribers = new List<ISlaveCommunicationCallbacks>();

    public StatusCommons Subscribe() {
      try {
        ISlaveCommunicationCallbacks callback = OperationContext.Current.GetCallbackChannel<ISlaveCommunicationCallbacks>();
        if (!subscribers.Contains(callback)) {
          subscribers.Add(callback);
        }
        return ConfigManager.Instance.GetStatusForClientConsole();
      }
      catch {
        return null;
      }
    }

    public bool Unsubscribe() {
      try {
        ISlaveCommunicationCallbacks callback = OperationContext.Current.GetCallbackChannel<ISlaveCommunicationCallbacks>();
        if (subscribers.Contains(callback))
          subscribers.Remove(callback);
        return true;
      }
      catch {
        return false;
      }
    }

    public void LogMessage(string message) {
      try {
        subscribers.ForEach(delegate(ISlaveCommunicationCallbacks callback) {
          if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
            callback.OnMessageLogged(message);
          } else {
            subscribers.Remove(callback);
          }
        });
      }
      catch (Exception ex) {
        EventLogManager.LogException(ex);
      }
    }

    public void StatusChanged(StatusCommons status) {
      try {
        subscribers.ForEach(delegate(ISlaveCommunicationCallbacks callback) {
          if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
            callback.OnStatusChanged(status);
          } else {
            subscribers.Remove(callback);
          }
        });
      }
      catch (Exception ex) {
        EventLogManager.LogException(ex);
      }
    }

    public void Shutdown() {
      subscribers.ForEach(delegate(ISlaveCommunicationCallbacks callback) {
        if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
          callback.OnShutdown();
        } else {
          subscribers.Remove(callback);
        }
      });
    }

    public void Restart() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.Restart);
      MessageQueue.GetInstance().AddMessage(mc);
    }

    public void PauseAll() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.PauseAll);
      MessageQueue.GetInstance().AddMessage(mc);
    }

    public void StopAll() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.StopAll);
      MessageQueue.GetInstance().AddMessage(mc);
    }

    public void AbortAll() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.AbortAll);
      MessageQueue.GetInstance().AddMessage(mc);
    }

    public void Sleep() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.Sleep);
      MessageQueue.GetInstance().AddMessage(mc);
    }
  }
}
