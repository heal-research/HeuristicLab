#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Service interface for submitting new <see cref="Experiment"/> <see cref="Run"/>s.
  /// </summary>
  [ServiceContract(SessionMode = SessionMode.Required, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IRunnerService {

    /// <summary>
    /// Logs the specified username in. In case the user or client
    /// does not exist yet, they are created on the server. This
    /// method is currently not used for authentication but merely
    /// for auditing.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="clientname">The clientname.</param>
    /// <returns><c>true</c> if the login was successful; <c>false</c> otherwise.</returns>
    [OperationContract(IsInitiating = true)]
    bool Login(string username, string clientname);

    /// <summary>
    /// Gets a <see cref="StarterKit"/>: A partially populated object
    /// graph to enable selection of algorithm and problem for a certain
    /// platofrm.
    /// </summary>
    /// <param name="platformName">Name of the platform.</param>
    /// <returns>A <see cref="StarterKit"/>.</returns>
    [OperationContract(IsInitiating = false)]
    StarterKit GetStarterKit(string platformName);

    /// <summary>
    /// Get all necessary information to conduct an experiment. This retrieves the
    /// <see cref="Algorithm"/>'s:
    /// <list type="bullet">
    /// <item><see cref="Parameter"/>s</item>
    /// <item><see cref="Result"/>s</item>
    /// <item>and their respective <see cref="DataType"/>s</item>
    /// </list>
    /// as well as the <see cref="Problem"/>'s:
    /// <list>
    /// <item><see cref="SolutionRepresentation"/></item>
    /// <item><see cref="Parameter"/>s</item>
    /// </list>
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    /// <param name="problem">The problem.</param>
    /// <returns>A more complete object graph contained in an
    /// <see cref="ExperimentKit"/>.</returns>
    [OperationContract(IsInitiating = false)]
    ExperimentKit PrepareExperiment(Algorithm algorithm, Problem problem);

    /// <summary>
    /// Adds the a new <see cref="Experiment"/> <see cref="Run"/>.
    /// 
    /// The <see cref="Experiment"/> is created if necessary as well as
    /// all <see cref="Parameter"/>s and <see cref="Result"/>s. If an
    /// identical experiment has been conducted before the new run is
    /// linked to this previous experiment instead.
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    /// <param name="problem">The problem.</param>
    /// <param name="project">The project.</param>
    [OperationContract(IsInitiating = false)]
    void AddRun(Algorithm algorithm, Problem problem, Project project);

    /// <summary>
    /// Determines whether this instance is connected.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
    /// </returns>
    [OperationContract(IsInitiating = false)]
    bool IsConnected();

    /// <summary>
    /// Logout out and closes the connection.
    /// </summary>
    [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
    void Logout();
  }

}
