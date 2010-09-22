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

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataTransfer;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Interface of the OKB administration service.
  /// </summary>
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IAdminService {
    #region Platform Methods
    [OperationContract]
    Platform GetPlatform(long id);
    [OperationContract]
    IEnumerable<Platform> GetPlatforms();
    [OperationContract]
    void StorePlatform(Platform dto);
    [OperationContract]
    void DeletePlatform(long id);
    #endregion

    #region AlgorithmClass Methods
    [OperationContract]
    AlgorithmClass GetAlgorithmClass(long id);
    [OperationContract]
    IEnumerable<AlgorithmClass> GetAlgorithmClasses();
    [OperationContract]
    void StoreAlgorithmClass(AlgorithmClass dto);
    [OperationContract]
    void DeleteAlgorithmClass(long id);
    #endregion

    #region Algorithm Methods
    [OperationContract]
    Algorithm GetAlgorithm(long id);
    [OperationContract]
    IEnumerable<Algorithm> GetAlgorithms();
    [OperationContract]
    void StoreAlgorithm(Algorithm dto);
    [OperationContract]
    void DeleteAlgorithm(long id);
    [OperationContract]
    IEnumerable<Guid> GetAlgorithmUsers(long algorithmId);
    [OperationContract]
    void StoreAlgorithmUsers(long algorithmId, IEnumerable<Guid> users);
    #endregion

    #region AlgorithmData Methods
    [OperationContract]
    AlgorithmData GetAlgorithmData(long algorithmId);
    [OperationContract]
    void StoreAlgorithmData(AlgorithmData dto);
    #endregion

    #region DataType Methods
    [OperationContract]
    DataType GetDataType(long id);
    [OperationContract]
    IEnumerable<DataType> GetDataTypes();
    [OperationContract]
    void StoreDataType(DataType dto);
    [OperationContract]
    void DeleteDataType(long id);
    #endregion
  }
}
