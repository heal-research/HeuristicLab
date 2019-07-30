#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.OKB.Query.DataTransfer;

namespace HeuristicLab.Services.OKB.Query {
  /// <summary>
  /// Interface of the OKB query service.
  /// </summary>
  [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IQueryService {
    [OperationContract]
    IEnumerable<Filter> GetFilters();

    [OperationContract]
    long GetNumberOfRuns(Filter filter);

    [OperationContract]
    IEnumerable<long> GetRunIds(Filter filter);

    [OperationContract]
    IEnumerable<Run> GetRuns(IEnumerable<long> ids, bool includeBinaryValues);

    [OperationContract]
    IEnumerable<Run> GetRunsWithValues(IEnumerable<long> ids, bool includeBinaryValues, IEnumerable<DataTransfer.ValueName> valueNames);

    [OperationContract]
    IEnumerable<ValueName> GetValueNames();
  }
}
