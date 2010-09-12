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
using System.Data;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.OKB.AttributeSelection;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Implementation of the <see cref="IQueryService"/>.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
  public class QueryService : IQueryService, IDisposable {
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      Terminate();
    }

    private DataSetBuilder dataSetBuilder;
    private OKBDataContext okb;
    private IEnumerator<DataRow> dataEnumerator;

    /// <summary>
    /// Gets a list of all possible attribute selectors.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Server.AttributeSelector"/>s
    /// </returns>
    public AttributeSelector[] GetAllAttributeSelectors() {
      using (OKBDataContext okb = new OKBDataContext()) {
        return RunAttributeSelector.GetAllAttributeSelectors(okb).Select(s => new AttributeSelector(s)).ToArray();
      }
    }

    /// <summary>
    /// Prepares the a query using the specified selection criteria.
    /// </summary>
    /// <param name="selectors">The list of <see cref="Server.AttributeSelector"/>s.</param>
    /// <returns>
    /// An empty <see cref="DataTable"/> containing only the column headers.
    /// </returns>
    public DataTable PrepareQuery(AttributeSelector[] selectors) {
      using (OKBDataContext okb = new OKBDataContext()) {
        if (selectors.Length == 0)
          throw new FaultException("No columns selected");
        IEnumerable<RunAttributeSelector> checkedSelectors =
          selectors.Select(s => new RunAttributeSelector(new OKBDataContext(okb.Connection.ConnectionString), s)).ToList();
        dataSetBuilder = new DataSetBuilder(okb, checkedSelectors);
        dataEnumerator = dataSetBuilder.GetEnumerator();
        return dataSetBuilder.Runs;
      }
    }

    /// <summary>
    /// Gets the number of matching rows for the prepared query
    /// (see <see cref="IQueryService.PrepareQuery"/>).
    /// </summary>
    /// <returns>The number of matching rows.</returns>
    public int GetCount() {
      return dataSetBuilder.GetCount();
    }

    /// <summary>
    /// Gets the next rows.
    /// </summary>
    /// <param name="maxNRows">The max N rows.</param>
    /// <returns></returns>
    public DataTable GetNextRows(int maxNRows) {
      DataTable runs = dataSetBuilder.Runs.Clone();
      for (int i = 0; i < maxNRows; i++) {
        if (!dataEnumerator.MoveNext())
          break;
        DataRow row = runs.NewRow();
        row.ItemArray = (object[])dataEnumerator.Current.ItemArray.Clone();
        runs.Rows.Add(row);
      }
      return runs;
    }

    /// <summary>
    /// Terminates the session and closes the connection.
    /// </summary>
    public void Terminate() {
      dataSetBuilder = null;
      if (dataEnumerator != null) {
        dataEnumerator.Dispose();
        dataEnumerator = null;
      }
      if (okb != null) {
        okb.Dispose();
        okb = null;
      }
    }
  }
}
