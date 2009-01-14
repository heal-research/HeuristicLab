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

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// This is the faced for the management console
  /// </summary>
  [ServiceContract]
  public interface IClientManager {
    [OperationContract]
    ResponseList<ClientInfo> GetAllClients();
    [OperationContract]
    [ServiceKnownType(typeof (Resource))]
    [ServiceKnownType(typeof(ClientInfo))]
    [ServiceKnownType(typeof(ClientGroup))]
    ResponseList<ClientGroup> GetAllClientGroups();
    [OperationContract]
    Response AddClientGroup(ClientGroup clientGroup);
    [OperationContract]
    Response AddResourceToGroup(long clientGroupId, Resource resource);
    [OperationContract]
    Response DeleteResourceFromGroup(long clientGroupId, long resourceId);
    [OperationContract]
    ResponseList<UpTimeStatistics> GetAllUpTimeStatistics();
  }
}
