#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  public class SymbolicRegressionSolutionsAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string ResultCollectionParameterName = "Results";
    private const string RegressionSolutionQualitiesResultName = "Regression Solution Qualities";
    private const string TrainingQualityParameterName = "TrainingRSquared";
    private const string TestQualityParameterName = "TestRSquared";

    public ILookupParameter<ResultCollection> ResultCollectionParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName]; }
    }
    public ILookupParameter<DoubleValue> TrainingQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TrainingQualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> TestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TestQualityParameterName]; }
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
      Parameters.Add(new LookupParameter<DoubleValue>(TrainingQualityParameterName));
      Parameters.Add(new LookupParameter<DoubleValue>(TestQualityParameterName));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3

      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(TrainingQualityParameterName))
        Parameters.Add(new LookupParameter<DoubleValue>(TrainingQualityParameterName));
      if (!Parameters.ContainsKey(TestQualityParameterName))
        Parameters.Add(new LookupParameter<DoubleValue>(TestQualityParameterName));
      #endregion
    }

    public override IOperation Apply() {
      var results = ResultCollectionParameter.ActualValue;

      if (!results.ContainsKey(RegressionSolutionQualitiesResultName)) {
        var newDataTable = new DataTable(RegressionSolutionQualitiesResultName);
        results.Add(new Result(RegressionSolutionQualitiesResultName, "Chart displaying the training and test qualities of the regression solutions.", newDataTable));
      }

      var dataTable = (DataTable)results[RegressionSolutionQualitiesResultName].Value;

      // only if the parameters are available (not available in old persisted code)
      ILookupParameter<DoubleValue> trainingQualityParam = null;
      ILookupParameter<DoubleValue> testQualityParam = null;
      // store actual names of parameter because it is changed below
      trainingQualityParam = TrainingQualityParameter;
      string prevTrainingQualityParamName = trainingQualityParam.ActualName;
      testQualityParam = TestQualityParameter;
      string prevTestQualityParamName = testQualityParam.ActualName;
      foreach (var result in results.Where(r => r.Value is IRegressionSolution)) {
        var solution = (IRegressionSolution)result.Value;

        var trainingR2Name = result.Name + " Training R²";
        if (!dataTable.Rows.ContainsKey(trainingR2Name))
          dataTable.Rows.Add(new DataRow(trainingR2Name));

        var testR2Name = result.Name + " Test R²";
        if (!dataTable.Rows.ContainsKey(testR2Name))
          dataTable.Rows.Add(new DataRow(testR2Name));

        dataTable.Rows[trainingR2Name].Values.Add(solution.TrainingRSquared);
        dataTable.Rows[testR2Name].Values.Add(solution.TestRSquared);

        // also add training and test R² to the scope using the parameters
        // HACK: we change the ActualName of the parameter to write two variables for each solution in the results collection
        trainingQualityParam.ActualName = trainingR2Name;
        trainingQualityParam.ActualValue = new DoubleValue(solution.TrainingRSquared);
        testQualityParam.ActualName = testR2Name;
        testQualityParam.ActualValue = new DoubleValue(solution.TestRSquared);
      }

      trainingQualityParam.ActualName = prevTrainingQualityParamName;
      testQualityParam.ActualName = prevTestQualityParamName;

      return base.Apply();
    }
  }
}
