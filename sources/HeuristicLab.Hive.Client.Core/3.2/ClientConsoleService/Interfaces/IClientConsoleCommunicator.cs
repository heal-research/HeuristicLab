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
using Calendar;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService.Interfaces {
  /// <summary>
  /// Defines the interfaces for the Client Console
  /// </summary>
  [ServiceContract]
  public interface IClientConsoleCommunicator {
    /// <summary>
    /// Gets all status information concerning the client Service
    /// </summary>
    /// <returns>the StatusCommons Object, filled with the client details</returns>
    [OperationContract]
    StatusCommons GetStatusInfos();
    /// <summary>
    /// Set the connection settings in the wcfService
    /// </summary>
    /// <param name="container">the container with the connection details</param>
    [OperationContract]
    void SetConnection(ConnectionContainer container);
    /// <summary>
    /// Signals the client to disconnect from the server
    /// </summary>
    [OperationContract]
    void Disconnect();
    /// <summary>
    /// Get the current connection details
    /// </summary>
    /// <returns>the ConnectionContainer Object</returns>
    [OperationContract]
    ConnectionContainer GetCurrentConnection();
    /// <summary>
    /// Signals the client to exit all jobs and shutdown
    /// </summary>
    [OperationContract]
    void ShutdownClient();

    [OperationContract]
    void SetUptimeCalendar(List<Appointment> appointments);

    [OperationContract]
    List<Appointment> GetUptimeCalendar();

  }
}
