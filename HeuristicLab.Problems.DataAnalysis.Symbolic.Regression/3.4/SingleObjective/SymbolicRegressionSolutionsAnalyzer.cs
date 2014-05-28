#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  public class SymbolicRegressionSolutionsAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string ResultCollectionParameterName = "Results";
    private const string RegressionSolutionQualitiesResultName = "Regression Solution Qualities";

    public ILookupParameter<ResultCollection> ResultCollectionParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName]; }
    }

    public virtual bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    protected SymbolicRegressionSolutionsAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSolutionsAnalyzer(SymbolicRegressionSolutionsAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolutionsAnalyzer(this, cloner);
    }

    public SymbolicRegressionSolutionsAnalyzer() {
      Parameters.Add(new LookupParameter<ResultCollection>(ResultCollectionParameterName, "The result collection to store the analysis results."));
    }

    public override IOperation Apply() {
      var results = ResultCollectionParameter.ActualValue;

      if (!results.ContainsKey(RegressionSolutionQualitiesResultName)) {
        var newDataTable = new DataTable(RegressionSolutionQualitiesResultName);
        results.Add(new Result(RegressionSolutionQualitiesResultName, "Chart displaying the training and test qualities of the regression solutions.", newDataTable));
      }

      var dataTable = (DataTable)results[RegressionSolutionQualitiesResultName].Value;
      foreach (var result in results.Where(r => r.Value is IRegressionSolution)) {
        var solution = (IRegressionSolution)result.Value;

        var trainingR2 = result.Name + Environment.NewLine + "Training R²";
        if (!dataTable.Rows.ContainsKey(trainingR2))
          dataTable.Rows.Add(new DataRow(trainingR2));

        var testR2 = result.Name + Environment.NewLine + " Test R²";
        if (!dataTable.Rows.ContainsKey(testR2))
          dataTable.Rows.Add(new DataRow(testR2));

        dataTable.Rows[trainingR2].Values.Add(solution.TrainingRSquared);
        dataTable.Rows[testR2].Values.Add(solution.TestRSquared);
      }

      return base.Apply();
    }
  }
}
