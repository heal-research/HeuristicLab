#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Services.OKB.RunCreation.DataTransfer;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;

namespace HeuristicLab.Services.OKB.RunCreation {
  /// <summary>
  /// Interface of the OKB run creation service.
  /// </summary>
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IRunCreationService {
    [OperationContract]
    IEnumerable<Algorithm> GetAlgorithms(string platformName);

    [OperationContract]
    byte[] GetAlgorithmData(long algorithmId);

    [OperationContract]
    IEnumerable<Problem> GetProblems(string platformName);

    [OperationContract]
    byte[] GetProblemData(long problemId);

    [OperationContract]
    [FaultContract(typeof(MissingProblem))]
    IEnumerable<Solution> GetSolutions(long problemId);

    [OperationContract]
    Solution GetSolution(long solutionId);

    [OperationContract]
    [FaultContract(typeof(MissingSolution))]
    byte[] GetSolutionData(long solutionId);

    [OperationContract]
    long AddSolution(Solution solution, byte[] data);

    [OperationContract]
    void DeleteSolution(Solution solution);

    [OperationContract]
    void AddRun(Run run);

    [OperationContract]
    IEnumerable<Value> GetCharacteristicValues(long problemId);

    [OperationContract]
    [FaultContract(typeof(MissingProblem))]
    [FaultContract(typeof(UnknownCharacteristicType))]
    void SetCharacteristicValue(long problemId, Value value);

    [OperationContract]
    [FaultContract(typeof(MissingProblem))]
    [FaultContract(typeof(UnknownCharacteristicType))]
    void SetCharacteristicValues(long problemId, Value[] values);
  }
}
