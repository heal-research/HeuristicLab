#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("ResultsHistoryWiper", "An operator that removes the history of a ResultsCollection by setting all values in all DataTables to NaN.")]
  [StorableClass]
  public sealed class ResultsHistoryWiper : SingleSuccessorOperator {
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    [StorableConstructor]
    private ResultsHistoryWiper(bool deserializing) : base(deserializing) {
    }

    private ResultsHistoryWiper(ResultsHistoryWiper original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResultsHistoryWiper(this, cloner);
    }

    public ResultsHistoryWiper()
      : base() {
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The ResultCollection the history is wiped from."));
    }

    public override IOperation Apply() {
      var results = ResultsParameter.ActualValue;
      ClearHistoryRecursively(results);
      return base.Apply();
    }

    private void ClearHistoryRecursively(ResultCollection results) {
      var values = results.Select(r => r.Value);

      // Reset all values within results in results
      foreach (var resultsCollection in values.OfType<ResultCollection>())
        ClearHistoryRecursively(resultsCollection);

      // Reset values
      foreach (var dataTable in values.OfType<DataTable>()) {
        foreach (var row in dataTable.Rows)
          for (int i = 0; i < row.Values.Count; i++)
            row.Values[i] = double.NaN;
      }
    }
  }
}