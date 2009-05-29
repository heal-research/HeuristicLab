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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.PluginInfrastructure;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.Core {
  [ServiceBehavior(InstanceContextMode =
    InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  class ClientFacade: IClientFacade {

    private IClientCommunicator clientCommunicator =
      ServiceLocator.GetClientCommunicator();

    #region IClientCommunicator Members

    public Response Login(ClientInfo clientInfo) {
      return clientCommunicator.Login(clientInfo);
    }

    public ResponseHB ProcessHeartBeat(HeartBeatData hbData) {
      return clientCommunicator.ProcessHeartBeat(hbData);
    }

    public ResponseJob SendJob(Guid clientId) {
      return clientCommunicator.SendJob(clientId);
    }

    public ResponseResultReceived StoreFinishedJobResult(Guid clientId,
      Guid jobId,
      byte[] result,
      double percentage, 
      Exception exception) {
      return clientCommunicator.StoreFinishedJobResult(clientId, jobId, result, percentage, exception);
    }

    public Response Logout(Guid clientId) {
      return clientCommunicator.Logout(clientId);
    }

    public Response IsJobStillNeeded(Guid jobId) {
      return clientCommunicator.IsJobStillNeeded(jobId);
    }

    public ResponsePlugin SendPlugins(List<HivePluginInfo> pluginList) {
      return clientCommunicator.SendPlugins(pluginList);
    }

    public ResponseResultReceived ProcessSnapshot(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      return clientCommunicator.ProcessSnapshot(clientId, jobId, result, percentage, exception);
    }

    #endregion

    #region IClientFacade Members

    public Stream SendStreamedJob(Guid clientId) {
      return
        new StreamedObject<ResponseJob>(
          this.SendJob(clientId));
    }

    public Stream SendStreamedPlugins(List<HivePluginInfo> pluginList) {
      return
        new StreamedObject<ResponsePlugin>(
          this.SendPlugins(pluginList));
    }

    public ResponseResultReceived StoreFinishedJobResultStreamed(Stream stream) {
      BinaryFormatter formatter =
          new BinaryFormatter();
      JobResult result = (JobResult)formatter.Deserialize(stream);

      return this.StoreFinishedJobResult(
          result.ClientId,
          result.JobId,
          result.Result,
          result.Percentage,
          result.Exception);
    } 

    public ResponseResultReceived ProcessSnapshotStreamed(Stream stream) {
      BinaryFormatter formatter =
          new BinaryFormatter();
      JobResult result = (JobResult)formatter.Deserialize(stream);

      return this.ProcessSnapshot(
          result.ClientId,
          result.JobId,
          result.Result,
          result.Percentage,
          result.Exception);
    }

    #endregion
  }
}
