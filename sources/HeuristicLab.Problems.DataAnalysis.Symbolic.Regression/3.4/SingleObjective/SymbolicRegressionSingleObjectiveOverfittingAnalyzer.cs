#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("SymbolicRegressionSingleObjectiveOverfittingAnalyzer", "Calculates and tracks correlation of training and validation fitness of symbolic regression models.")]
  [StorableClass]
  public sealed class SymbolicRegressionSingleObjectiveOverfittingAnalyzer : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<ISymbolicRegressionSingleObjectiveEvaluator, IRegressionProblemData> {
    private const string TrainingValidationCorrelationParameterName = "Training and validation fitness correlation";
    private const string TrainingValidationCorrelationTableParameterName = "Training and validation fitness correlation table";
    private const string LowerCorrelationThresholdParameterName = "LowerCorrelationThreshold";
    private const string UpperCorrelationThresholdParameterName = "UpperCorrelationThreshold";
    private const string OverfittingParameterName = "IsOverfitting";

    #region parameter properties
    public ILookupParameter<DoubleValue> TrainingValidationQualityCorrelationParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TrainingValidationCorrelationParameterName]; }
    }
    public ILookupParameter<DataTable> TrainingValidationQualityCorrelationTableParameter {
      get { return (ILookupParameter<DataTable>)Parameters[TrainingValidationCorrelationTableParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerCorrelationThresholdParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerCorrelationThresholdParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperCorrelationThresholdParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperCorrelationThresholdParameterName]; }
    }
    public ILookupParameter<BoolValue> OverfittingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[OverfittingParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionSingleObjectiveOverfittingAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionSingleObjectiveOverfittingAnalyzer(SymbolicRegressionSingleObjectiveOverfittingAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionSingleObjectiveOverfittingAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(TrainingValidationCorrelationParameterName, "Correlation of training and validation fitnesses"));
      Parameters.Add(new LookupParameter<DataTable>(TrainingValidationCorrelationTableParameterName, "Data table of training and validation fitness correlation values over the whole run."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerCorrelationThresholdParameterName, "Lower threshold for correlation value that marks the boundary from non-overfitting to overfitting.", new DoubleValue(0.65)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperCorrelationThresholdParameterName, "Upper threshold for correlation value that marks the boundary from overfitting to non-overfitting.", new DoubleValue(0.75)));
      Parameters.Add(new LookupParameter<BoolValue>(OverfittingParameterName, "Boolean indicator for overfitting."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveOverfittingAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      if (!rows.Any()) return base.Apply();

      double[] trainingQuality = QualityParameter.ActualValue.Select(x => x.Value).ToArray();
      var problemData = ProblemDataParameter.ActualValue;
      var evaluator = EvaluatorParameter.ActualValue;
      // evaluate on validation partition
      IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
      double[] validationQuality = SymbolicExpressionTree
        .AsParallel()
        .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
        .ToArray();
      double r = 0.0;
      try {
        r = alglib.spearmancorr2(trainingQuality, validationQuality);
      }
      catch (alglib.alglibexception) {
        r = 0.0;
      }

      TrainingValidationQualityCorrelationParameter.ActualValue = new DoubleValue(r);

      if (TrainingValidationQualityCorrelationTableParameter.ActualValue == null) {
        var dataTable = new DataTable(TrainingValidationQualityCorrelationTableParameter.Name, TrainingValidationQualityCorrelationTableParameter.Description);
        dataTable.Rows.Add(new DataRow(TrainingValidationQualityCorrelationParameter.Name, TrainingValidationQualityCorrelationParameter.Description));
        dataTable.Rows[TrainingValidationQualityCorrelationParameter.Name].VisualProperties.StartIndexZero = true;
        TrainingValidationQualityCorrelationTableParameter.ActualValue = dataTable;
        ResultCollectionParameter.ActualValue.Add(new Result(TrainingValidationQualityCorrelationTableParameter.Name, dataTable));
      }

      TrainingValidationQualityCorrelationTableParameter.ActualValue.Rows[TrainingValidationQualityCorrelationParameter.Name].Values.Add(r);

      if (OverfittingParameter.ActualValue != null && OverfittingParameter.ActualValue.Value) {
        // overfitting == true
        // => r must reach the upper threshold to switch back to non-overfitting state
        OverfittingParameter.ActualValue = new BoolValue(r < UpperCorrelationThresholdParameter.ActualValue.Value);
      } else {
        // overfitting == false 
        // => r must drop below lower threshold to switch to overfitting state
        OverfittingParameter.ActualValue = new BoolValue(r < LowerCorrelationThresholdParameter.ActualValue.Value);
      }

      return base.Apply();
    }
  }
}
