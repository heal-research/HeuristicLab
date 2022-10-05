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

using System;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// An operator that optimizes the parameters for the best symbolic expression tress in the current generation.
  /// </summary>
  [Item("ParameterOptimizationAnalyzer", "An operator that performs a parameter optimization on the best symbolic expression trees.")]
  [StorableType("9FB87E7B-A9E2-49DD-A92A-78BD9FC17916")]
  public sealed class ParameterOptimizationAnalyzer : SymbolicDataAnalysisSingleObjectiveAnalyzer, IStatefulItem {
    private const string PercentageOfBestSolutionsParameterName = "PercentageOfBestSolutions";
    private const string ParameterOptimizationEvaluatorParameterName = "ParameterOptimizationOperator";

    private const string DataTableNameParameterOptimizationImprovement = "Parameter Optimization Improvement";
    private const string DataRowNameMinimumImprovement = "Minimum improvement";
    private const string DataRowNameMedianImprovement = "Median improvement";
    private const string DataRowNameAverageImprovement = "Average improvement";
    private const string DataRowNameMaximumImprovement = "Maximum improvement";

    #region parameter properties
    public IFixedValueParameter<PercentValue> PercentageOfBestSolutionsParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[PercentageOfBestSolutionsParameterName]; }
    }

    public IFixedValueParameter<SymbolicRegressionParameterOptimizationEvaluator> ParameterOptimizationEvaluatorParameter {
      get { return (IFixedValueParameter<SymbolicRegressionParameterOptimizationEvaluator>)Parameters[ParameterOptimizationEvaluatorParameterName]; }
    }
    #endregion

    #region properties
    public SymbolicRegressionParameterOptimizationEvaluator ParameterOptimizationEvaluator {
      get { return ParameterOptimizationEvaluatorParameter.Value; }
    }
    public double PercentageOfBestSolutions {
      get { return PercentageOfBestSolutionsParameter.Value.Value; }
    }

    private DataTable ParameterOptimizationImprovementDataTable {
      get {
        IResult result;
        ResultCollection.TryGetValue(DataTableNameParameterOptimizationImprovement, out result);
        if (result == null) return null;
        return (DataTable)result.Value;
      }
    }
    private DataRow MinimumImprovement {
      get { return ParameterOptimizationImprovementDataTable.Rows[DataRowNameMinimumImprovement]; }
    }
    private DataRow MedianImprovement {
      get { return ParameterOptimizationImprovementDataTable.Rows[DataRowNameMedianImprovement]; }
    }
    private DataRow AverageImprovement {
      get { return ParameterOptimizationImprovementDataTable.Rows[DataRowNameAverageImprovement]; }
    }
    private DataRow MaximumImprovement {
      get { return ParameterOptimizationImprovementDataTable.Rows[DataRowNameMaximumImprovement]; }
    }
    #endregion

    [StorableConstructor]
    private ParameterOptimizationAnalyzer(StorableConstructorFlag _) : base(_) { }
    private ParameterOptimizationAnalyzer(ParameterOptimizationAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new ParameterOptimizationAnalyzer(this, cloner); }
    public ParameterOptimizationAnalyzer()
      : base() {
      Parameters.Add(new FixedValueParameter<PercentValue>(PercentageOfBestSolutionsParameterName, "The percentage of the top solutions which should be analyzed.", new PercentValue(0.1)));
      Parameters.Add(new FixedValueParameter<SymbolicRegressionParameterOptimizationEvaluator>(ParameterOptimizationEvaluatorParameterName, "The operator used to perform the parameter optimization"));

      //Changed the ActualName of the EvaluationPartitionParameter so that it matches the parameter name of symbolic regression problems.
      ParameterOptimizationEvaluator.EvaluationPartitionParameter.ActualName = "FitnessCalculationPartition";
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ParameterOptimizationEvaluatorParameterName)) {
        if (Parameters.ContainsKey("ConstantOptimizationOperator")) {
          Parameters.Add(new FixedValueParameter<SymbolicRegressionParameterOptimizationEvaluator>(ParameterOptimizationEvaluatorParameterName, "The operator used to perform the parameter optimization"));
          Parameters.Remove("ConstantOptimizationOperator");
        } else {
          Parameters.Add(new FixedValueParameter<SymbolicRegressionParameterOptimizationEvaluator>(ParameterOptimizationEvaluatorParameterName, "The operator used to perform the parameter optimization"));
        }
      }
    }


    private double[] qualitiesBeforeCoOp = null;
    private int[] scopeIndexes = null;
    void IStatefulItem.InitializeState() {
      qualitiesBeforeCoOp = null;
      scopeIndexes = null;
    }
    void IStatefulItem.ClearState() {
      qualitiesBeforeCoOp = null;
      scopeIndexes = null;
    }

    public override IOperation Apply() {
      //code executed in the first call of analyzer
      if (qualitiesBeforeCoOp == null) {
        double[] trainingQuality;
        // sort is ascending and we take the first n% => order so that best solutions are smallest
        // sort order is determined by maximization parameter
        if (Maximization.Value) {
          // largest values must be sorted first
          trainingQuality = Quality.Select(x => -x.Value).ToArray();
        } else {
          // smallest values must be sorted first
          trainingQuality = Quality.Select(x => x.Value).ToArray();
        }
        // sort trees by training qualities
        int topN = (int)Math.Max(trainingQuality.Length * PercentageOfBestSolutions, 1);
        scopeIndexes = Enumerable.Range(0, trainingQuality.Length).ToArray();
        Array.Sort(trainingQuality, scopeIndexes);
        scopeIndexes = scopeIndexes.Take(topN).ToArray();
        qualitiesBeforeCoOp = scopeIndexes.Select(x => Quality[x].Value).ToArray();

        OperationCollection operationCollection = new OperationCollection();
        operationCollection.Parallel = true;
        foreach (var scopeIndex in scopeIndexes) {
          var childOperation = ExecutionContext.CreateChildOperation(ParameterOptimizationEvaluator, ExecutionContext.Scope.SubScopes[scopeIndex]);
          operationCollection.Add(childOperation);
        }

        return new OperationCollection { operationCollection, ExecutionContext.CreateOperation(this) };
      }

      //code executed to analyze results of parameter optimization
      double[] qualitiesAfterCoOp = scopeIndexes.Select(x => Quality[x].Value).ToArray();
      var qualityImprovement = qualitiesBeforeCoOp.Zip(qualitiesAfterCoOp, (b, a) => a - b).ToArray();

      if (!ResultCollection.ContainsKey(DataTableNameParameterOptimizationImprovement)) {
        var dataTable = new DataTable(DataTableNameParameterOptimizationImprovement);
        ResultCollection.Add(new Result(DataTableNameParameterOptimizationImprovement, dataTable));
        dataTable.VisualProperties.YAxisTitle = "R²";

        dataTable.Rows.Add(new DataRow(DataRowNameMinimumImprovement));
        MinimumImprovement.VisualProperties.StartIndexZero = true;

        dataTable.Rows.Add(new DataRow(DataRowNameMedianImprovement));
        MedianImprovement.VisualProperties.StartIndexZero = true;

        dataTable.Rows.Add(new DataRow(DataRowNameAverageImprovement));
        AverageImprovement.VisualProperties.StartIndexZero = true;

        dataTable.Rows.Add(new DataRow(DataRowNameMaximumImprovement));
        MaximumImprovement.VisualProperties.StartIndexZero = true;
      }

      MinimumImprovement.Values.Add(qualityImprovement.Min());
      MedianImprovement.Values.Add(qualityImprovement.Median());
      AverageImprovement.Values.Add(qualityImprovement.Average());
      MaximumImprovement.Values.Add(qualityImprovement.Max());

      qualitiesBeforeCoOp = null;
      scopeIndexes = null;
      return base.Apply();
    }
  }
}
