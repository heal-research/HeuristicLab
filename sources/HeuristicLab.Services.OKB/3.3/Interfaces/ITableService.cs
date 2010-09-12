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
  /// Service interface for downloading and uploading data tables.
  /// </summary>
  [ServiceContract(SessionMode = SessionMode.Required, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface ITableService {
    /// <summary>
    /// Prepares the data table to be downloaded.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="count">The number of rows.</param>
    /// <returns>An empyt <see cref="DataType"/> that contains just
    /// the column headers.</returns>
    [OperationContract(IsInitiating = true)]
    DataTable PrepareDataTable(string tableName, out int count);

    /// <summary>
    /// Gets the next few rows.
    /// </summary>
    /// <param name="count">The maximum number of rows to return.</param>
    /// <returns>A partial <see cref="DataTable"/> with the
    /// next few rows.</returns>
    [OperationContract(IsInitiating = false, IsTerminating = false)]
    DataTable GetNextRows(int count);

    /// <summary>
    /// Finishes fetching rows and closes the connection.
    /// </summary>
    [OperationContract(IsTerminating = true)]
    void FinishFetchingRows();

    /// <summary>
    /// Updates the data table using the partial <see cref="DataTable"/>
    /// with changed rows.
    /// </summary>
    /// <param name="changedRows">The <see cref="DataTable"/> with
    /// changed rows.</param>
    /// <param name="tableName">Name of the table.</param>
    [OperationContract(IsInitiating = true, IsTerminating = true)]
    void UpdateDataTable(DataTable changedRows, string tableName);

    /// <summary>
    /// Deletes the selected table rows using the value of the
    /// "Id" column.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="tableName">Name of the table.</param>
    [OperationContract(IsInitiating = true, IsTerminating = true)]
    void DeleteTableRows(int[] ids, string tableName);
  }
}
