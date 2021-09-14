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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// Finds all RegressionSolutions in a ResultCollection and extracts the DoubleValue of a result (an optionally a secondary result) for each solution.
  /// All values are tracked in a single DataTable over generations.
  /// This can be used to extract training and test NMSE for the best solution for a chart of training/test NMSE over generations.
  /// </summary>
  [StorableType("52047D25-C042-4773-B12D-AB67DD707AE6")]
  public sealed class SolutionQualityAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string ResultCollectionParameterName = "Results";
    private const string SolutionResultsResultName = "Solution Results";
    private const string DefaultTrainingResultName = "Normalized mean squared error (training)";
    private const string DefaultTestResultName = "Normalized mean squared error (test)"; // use NMSE instead of R². R² is too optimistic for the test (assumes perfect scaling).

    public ILookupParameter<ResultCollection> ResultCollectionParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName]; }
    }
    public ILookupParameter<DoubleValue> ResultParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[DefaultTrainingResultName]; }
    }
    public ILookupParameter<DoubleValue> SecondaryResultParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[DefaultTestResultName]; }
    }

    public bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    private SolutionQualityAnalyzer(StorableConstructorFlag _) : base(_) { }
    private SolutionQualityAnalyzer(SolutionQualityAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SolutionQualityAnalyzer(this, cloner);
    }

    public SolutionQualityAnalyzer() {
      Parameters.Add(new LookupParameter<ResultCollection>(ResultCollectionParameterName, "The result collection to store the analysis results."));
      Parameters.Add(new LookupParameter<DoubleValue>(DefaultTrainingResultName, $"First solution result name (default: {DefaultTrainingResultName})."));
      Parameters.Add(new LookupParameter<DoubleValue>(DefaultTestResultName, $"Second solution result name (default: {DefaultTestResultName}). This is optional. Leave empty if not used."));
    }

    public override IOperation Apply() {
      var results = ResultCollectionParameter.ActualValue;

      if (!results.ContainsKey(SolutionResultsResultName)) {
        var newDataTable = new DataTable(SolutionResultsResultName);
        results.Add(new Result(SolutionResultsResultName, "Chart of solution result values of all regression solutions.", newDataTable));
      }

      var dataTable = (DataTable)results[SolutionResultsResultName].Value;

      // store actual names of parameter because they are changed below
      var resultParameter = ResultParameter;
      string savedResultParamName = resultParameter.ActualName;
      var secondaryResultParameter = SecondaryResultParameter;
      string savedSecondaryResultParamName = secondaryResultParameter.ActualName;
      try {
        foreach (var result in results.Where(r => r.Value is RegressionSolution)) {
          var solution = (RegressionSolution)result.Value;

          var resultRowName = result.Name + " " + savedResultParamName;
          var resultValue = solution[savedResultParamName].Value as DoubleValue;
          if (resultValue == null) throw new ArgumentException($"SolutionQualityAnalyzer: the selected result ({savedResultParamName}) is no DoubleValue");

          if (!dataTable.Rows.ContainsKey(resultRowName))
            dataTable.Rows.Add(new DataRow(resultRowName));

          dataTable.Rows[resultRowName].Values.Add(resultValue.Value);
          // HACK: we change the ActualName of the parameter to write the results for each solution to the scope
          resultParameter.ActualName = resultRowName;
          resultParameter.ActualValue = new DoubleValue(resultValue.Value);

          // same for the second result if it is given
          if (!string.IsNullOrWhiteSpace(savedSecondaryResultParamName)) {
            var secondaryResultRowName = result.Name + " " + savedSecondaryResultParamName;
            var secondaryResultValue = solution[savedSecondaryResultParamName].Value as DoubleValue;
            if (secondaryResultValue == null) throw new ArgumentException($"SolutionQualityAnalyzer: the selected result ({savedSecondaryResultParamName}) is no DoubleValue");

            if (!dataTable.Rows.ContainsKey(secondaryResultRowName))
              dataTable.Rows.Add(new DataRow(secondaryResultRowName));

            dataTable.Rows[secondaryResultRowName].Values.Add(secondaryResultValue.Value);
            secondaryResultParameter.ActualName = secondaryResultRowName;
            secondaryResultParameter.ActualValue = new DoubleValue(secondaryResultValue.Value);
          }
        }
      } finally {
        // restore saved parameter names
        resultParameter.ActualName = savedResultParamName;
        secondaryResultParameter.ActualName = savedSecondaryResultParamName;
      }
      return base.Apply();
    }
  }
}
