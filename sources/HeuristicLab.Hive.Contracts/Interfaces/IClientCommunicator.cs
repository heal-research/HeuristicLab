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
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// This is the facade for the client communication
  /// </summary>
  [ServiceContract]
  public interface IClientCommunicator {
    [OperationContract]
    Response Login(ClientInfo clientInfo);
    [OperationContract]
    ResponseHB ProcessHeartBeat(HeartBeatData hbData);
    [OperationContract]
    ResponseJob SendJob(Guid clientId);
    [OperationContract]
    ResponseResultReceived StoreFinishedJobResult(Guid clientId, 
      Guid jobId, 
      byte[] result, 
      double percentage,
      Exception exception);
    [OperationContract]
    ResponseResultReceived ProcessSnapshot(Guid clientId,
      Guid jobId, 
      byte[] result, 
      double percentage, 
      Exception exception);
    [OperationContract]
    Response Logout(Guid clientId);
    [OperationContract]
    Response IsJobStillNeeded(Guid jobId);
    [OperationContract]
    ResponsePlugin SendPlugins(List<PluginInfo> pluginList);
  }
}
