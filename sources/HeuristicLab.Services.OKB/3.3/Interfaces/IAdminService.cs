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

using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Service of administrating the OKB.
  /// </summary>  
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IAdminService {
    [OperationContract]
    AlgorithmClass GetAlgorithmClass(long algorithmClassId);
    [OperationContract]
    IEnumerable<AlgorithmClass> GetAlgorithmClasses();
    [OperationContract]
    void StoreAlgorithmClass(AlgorithmClass algorithmClass);
    [OperationContract]
    void DeleteAlgorithmClass(long algorithmClassId);

    [OperationContract]
    Algorithm GetAlgorithm(long algorithmId);
    [OperationContract]
    IEnumerable<Algorithm> GetAlgorithms();
    [OperationContract]
    void StoreAlgorithm(Algorithm algorithm);
    [OperationContract]
    void DeleteAlgorithm(long algorithmId);





    /// <summary>
    /// Gets all available platforms.
    /// </summary>
    /// <returns>A list of <see cref="Platform"/>s.</returns>
    [OperationContract]
    Platform[] GetPlatforms();

    /// <summary>
    /// Gets the complete algorithm object graph up to the following entities:
    /// <list type="bullet">
    /// <item>Parameter</item>
    /// <item>Algorithm_Paramters.Parameter.DataType</item>
    /// <item>Algorithm_Results.Result.DataType</item>
    /// </list>
    /// </summary>
    /// <param name="id">The algorithm id.</param>
    /// <returns>An <see cref="Algorithm"/></returns>
    [OperationContract]
    Algorithm GetCompleteAlgorithm(int id);

    /// <summary>
    /// Gets the complete problem object graph up to the following entities:
    /// <list type="bullet">
    /// <item>Platform</item>
    /// <item>SolutionRepresentation</item>
    /// <item>Problem_Parameters.Parameter</item>
    /// <item>IntProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// <item>FloatProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// <item>CharProblemCharacteristicValues.ProblemCharacteristic.DataType</item>
    /// </list>
    /// </summary>
    /// <param name="id">The problem id.</param>
    /// <returns>A <see cref="Problem"/></returns>
    [OperationContract]
    Problem GetCompleteProblem(int id);

    /// <summary>
    /// Updates the algorithm object graph including the following properties and linked entitites:
    /// <list type="bullet">
    /// <item>Name</item>
    /// <item>Description</item>
    /// <item>AlgorithmClassId</item>
    /// <item>PlatformId</item>
    /// <item>Algorithm_Parameters</item>
    /// <item>Algorithm_Results</item>
    /// </list>
    /// <remarks>
    /// New <see cref="Parameter"/>s or <see cref="Result"/>s will not be
    /// created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    [OperationContract]
    void UpdateCompleteAlgorithm(Algorithm algorithm);

    /// <summary>
    /// Updates the problem object graph including the following properties and linked entities:
    /// <list type="bullet">
    /// <item>Name</item>
    /// <item>Description</item>
    /// <item>ProblemClassId</item>
    /// <item>PlatformId</item>
    /// <item>SolutionRepresentationId</item>
    /// <item>IntProblemCharacteristicValues.Value</item>
    /// <item>FloatProblemCharacteristicValues.Value</item>
    /// <item>CharProblemCharacteristicValues.Value</item>
    /// <item>Problem_Parameters</item>
    /// </list>
    /// <remarks>
    /// New <see cref="ProblemCharacteristic"/>s or <see cref="Parameter"/>s will
    /// not be created but have to be pre-existing.
    /// </remarks>
    /// </summary>
    /// <param name="problem">The problem.</param>
    [OperationContract]
    void UpdateCompleteProblem(Problem problem);
  }
}
