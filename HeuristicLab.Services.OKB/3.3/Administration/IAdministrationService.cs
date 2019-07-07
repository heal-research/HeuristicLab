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
using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.OKB.Administration.DataTransfer;

namespace HeuristicLab.Services.OKB.Administration {
  /// <summary>
  /// Interface of the OKB administration service.
  /// </summary>
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IAdministrationService {
    #region Platform Methods
    [OperationContract]
    Platform GetPlatform(long id);
    [OperationContract]
    IEnumerable<Platform> GetPlatforms();
    [OperationContract]
    long AddPlatform(Platform dto);
    [OperationContract]
    void UpdatePlatform(Platform dto);
    [OperationContract]
    void DeletePlatform(long id);
    #endregion

    #region AlgorithmClass Methods
    [OperationContract]
    AlgorithmClass GetAlgorithmClass(long id);
    [OperationContract]
    IEnumerable<AlgorithmClass> GetAlgorithmClasses();
    [OperationContract]
    long AddAlgorithmClass(AlgorithmClass dto);
    [OperationContract]
    void UpdateAlgorithmClass(AlgorithmClass dto);
    [OperationContract]
    void DeleteAlgorithmClass(long id);
    #endregion

    #region Algorithm Methods
    [OperationContract]
    Algorithm GetAlgorithm(long id);
    [OperationContract]
    IEnumerable<Algorithm> GetAlgorithms();
    [OperationContract]
    long AddAlgorithm(Algorithm dto);
    [OperationContract]
    void UpdateAlgorithm(Algorithm dto);
    [OperationContract]
    void DeleteAlgorithm(long id);
    [OperationContract]
    IEnumerable<Guid> GetAlgorithmUsers(long algorithmId);
    [OperationContract]
    void UpdateAlgorithmUsers(long algorithmId, IEnumerable<Guid> users);
    [OperationContract]
    byte[] GetAlgorithmData(long algorithmId);
    [OperationContract]
    void UpdateAlgorithmData(long algorithmId, byte[] data);
    #endregion

    #region ProblemClass Methods
    [OperationContract]
    ProblemClass GetProblemClass(long id);
    [OperationContract]
    IEnumerable<ProblemClass> GetProblemClasses();
    [OperationContract]
    long AddProblemClass(ProblemClass dto);
    [OperationContract]
    void UpdateProblemClass(ProblemClass dto);
    [OperationContract]
    void DeleteProblemClass(long id);
    #endregion

    #region Problem Methods
    [OperationContract]
    Problem GetProblem(long id);
    [OperationContract]
    IEnumerable<Problem> GetProblems();
    [OperationContract]
    long AddProblem(Problem dto);
    [OperationContract]
    void UpdateProblem(Problem dto);
    [OperationContract]
    void DeleteProblem(long id);
    [OperationContract]
    IEnumerable<Guid> GetProblemUsers(long problemId);
    [OperationContract]
    void UpdateProblemUsers(long problemId, IEnumerable<Guid> users);
    [OperationContract]
    byte[] GetProblemData(long problemId);
    [OperationContract]
    void UpdateProblemData(long problemId, byte[] data);
    #endregion
  }
}
