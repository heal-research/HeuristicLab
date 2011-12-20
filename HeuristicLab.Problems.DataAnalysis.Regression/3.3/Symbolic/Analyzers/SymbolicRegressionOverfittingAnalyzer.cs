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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  [Item("SymbolicRegressionOverfittingAnalyzer", "Calculates and tracks correlation of training and validation fitness of symbolic regression models.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicRegressionOverfittingAnalyzer : SymbolicRegressionValidationAnalyzer, ISymbolicRegressionAnalyzer {
    private const string MaximizationParameterName = "Maximization";
    private const string QualityParameterName = "Quality";
    private const string TrainingValidationCorrelationParameterName = "TrainingValidationCorrelation";
    private const string TrainingValidationCorrelationTableParameterName = "TrainingValidationCorrelationTable";
    private const string LowerCorrelationThresholdParameterName = "LowerCorrelationThreshold";
    private const string UpperCorrelationThresholdParameterName = "UpperCorrelationThreshold";
    private const string OverfittingParameterName = "IsOverfitting";
    private const string ResultsParameterName = "Results";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
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
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionOverfittingAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionOverfittingAnalyzer(SymbolicRegressionOverfittingAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionOverfittingAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "Training fitness"));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new LookupParameter<DoubleValue>(TrainingValidationCorrelationParameterName, "Correlation of training and validation fitnesses"));
      Parameters.Add(new LookupParameter<DataTable>(TrainingValidationCorrelationTableParameterName, "Data table of training and validation fitness correlation values over the whole run."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerCorrelationThresholdParameterName, "Lower threshold for correlation value that marks the boundary from non-overfitting to overfitting.", new DoubleValue(0.65)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperCorrelationThresholdParameterName, "Upper threshold for correlation value that marks the boundary from overfitting to non-overfitting.", new DoubleValue(0.75)));
      Parameters.Add(new LookupParameter<BoolValue>(OverfittingParameterName, "Boolean indicator for overfitting."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The results collection."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionOverfittingAnalyzer(this, cloner);
    }

    protected override void Analyze(SymbolicExpressionTree[] trees, double[] validationQuality) {
      double[] trainingQuality = QualityParameter.ActualValue.Select(x => x.Value).ToArray();

      double r = alglib.spearmancorr2(trainingQuality, validationQuality);

      TrainingValidationQualityCorrelationParameter.ActualValue = new DoubleValue(r);

      if (TrainingValidationQualityCorrelationTableParameter.ActualValue == null) {
        var dataTable = new DataTable("Training and validation fitness correlation table", "Data table of training and validation fitness correlation values over the whole run.");
        dataTable.Rows.Add(new DataRow("Training and validation fitness correlation", "Training and validation fitness correlation values"));
        TrainingValidationQualityCorrelationTableParameter.ActualValue = dataTable;
        ResultsParameter.ActualValue.Add(new Result(TrainingValidationCorrelationTableParameterName, dataTable));
      }

      TrainingValidationQualityCorrelationTableParameter.ActualValue.Rows["Training and validation fitness correlation"].Values.Add(r);

      if (OverfittingParameter.ActualValue != null && OverfittingParameter.ActualValue.Value) {
        // overfitting == true
        // => r must reach the upper threshold to switch back to non-overfitting state
        OverfittingParameter.ActualValue = new BoolValue(r < UpperCorrelationThresholdParameter.ActualValue.Value);
      } else {
        // overfitting == false 
        // => r must drop below lower threshold to switch to overfitting state
        OverfittingParameter.ActualValue = new BoolValue(r < LowerCorrelationThresholdParameter.ActualValue.Value);
      }
    }
  }
}
