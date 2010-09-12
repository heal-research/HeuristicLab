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

using System.Data;
using System.Net.Security;
using System.ServiceModel;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Service interface for downloading selected information about <see cref="Experiment"/> <see cref="Run"/>s.
  /// </summary>
  [ServiceContract(SessionMode = SessionMode.Required, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IQueryService {
    /// <summary>
    /// Gets a list of all possible attribute selectors.
    /// </summary>
    /// <returns>An array of <see cref="Server.AttributeSelector"/>s</returns>
    [OperationContract(IsInitiating = true, IsTerminating = true)]
    AttributeSelector[] GetAllAttributeSelectors();

    /// <summary>
    /// Prepares the a query using the specified selection criteria.
    /// </summary>
    /// <param name="selectors">The list of <see cref="Server.AttributeSelector"/>s.</param>
    /// <returns>An empty <see cref="DataTable"/> containing only the column headers.</returns>
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    DataTable PrepareQuery(AttributeSelector[] selectors);

    /// <summary>
    /// Gets the number of matching rows for the prepared query
    /// (see <see cref="IQueryService.PrepareQuery"/>).
    /// </summary>
    /// <returns>The number of matching rows.</returns>
    [OperationContract(IsInitiating = false, IsTerminating = false)]
    int GetCount();

    /// <summary>
    /// Fetches the next <paramref name="nRows"/> rows for the 
    /// prepared query (see <see cref="IQueryService.PrepareQuery"/>).
    /// </summary>
    /// <param name="nRows">The maximum number of rows to return.</param>
    /// <returns>A <see cref="DataTable"/> containing only the next
    /// <paramref name="nRows"/> rows.</returns>
    [OperationContract(IsInitiating = false, IsTerminating = false)]
    DataTable GetNextRows(int nRows);

    /// <summary>
    /// Terminates the session and closes the connection.
    /// </summary>
    [OperationContract(IsInitiating = false, IsTerminating = true)]
    void Terminate();
  }
}
