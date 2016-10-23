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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("SymbolicRegressionPhenotypicDiversityAnalyzer", "An analyzer which calculates diversity based on the phenotypic distance between trees")]
  [StorableClass]
  public class SymbolicRegressionPhenotypicDiversityAnalyzer : PopulationSimilarityAnalyzer,
    ISymbolicDataAnalysisBoundedOperator, ISymbolicDataAnalysisInterpreterOperator, ISymbolicExpressionTreeAnalyzer {
    #region parameter names
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string EvaluatedValuesParameterName = "EstimatedValues";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    #endregion

    #region parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    private IScopeTreeLookupParameter<DoubleArray> EvaluatedValuesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters[EvaluatedValuesParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueLookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion

    public SymbolicRegressionPhenotypicDiversityAnalyzer(IEnumerable<ISolutionSimilarityCalculator> validSimilarityCalculators)
      : base(validSimilarityCalculators) {
      #region add parameters
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(EvaluatedValuesParameterName, "Intermediate estimated values to be saved in the scopes."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new ValueLookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The upper and lower limit that should be used as cut off value for the output values of symbolic data analysis trees."));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Whether or not to apply linear scaling to the estimated values"));
      #endregion

      UpdateCounterParameter.ActualName = "PhenotypicDiversityAnalyzerUpdateCounter";
      DiversityResultName = "Phenotypic Diversity";
    }

    [StorableConstructor]
    protected SymbolicRegressionPhenotypicDiversityAnalyzer(bool deserializing)
      : base(deserializing) {
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName))
        Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Whether or not to apply linear scaling to the estimated values"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPhenotypicDiversityAnalyzer(this, cloner);
    }

    protected SymbolicRegressionPhenotypicDiversityAnalyzer(SymbolicRegressionPhenotypicDiversityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;

      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      }

      if (updateCounter.Value != updateInterval) return base.Apply();

      var scopes = ExecutionContext.Scope.SubScopes;
      var applyLinearScaling = ApplyLinearScalingParameter.ActualValue.Value;

      foreach (var scope in scopes.Where(x => !x.Variables.ContainsKey("EstimatedValues"))) {
        var tree = (ISymbolicExpressionTree)scope.Variables["SymbolicExpressionTree"].Value;
        var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
        var ds = ProblemDataParameter.ActualValue.Dataset;
        var rows = ProblemDataParameter.ActualValue.TrainingIndices;
        var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(tree, ds, rows).ToArray();

        var estimationLimits = EstimationLimitsParameter.ActualValue;

        if (applyLinearScaling) {
          var linearScalingCalculator = new OnlineLinearScalingParameterCalculator();
          var targetValues = ds.GetDoubleValues(ProblemDataParameter.ActualValue.TargetVariable, rows);
          int i = 0;
          foreach (var target in targetValues) {
            var estimated = estimatedValues[i];
            if (!double.IsNaN(estimated) && !double.IsInfinity(estimated))
              linearScalingCalculator.Add(estimated, target);
            i++;
          }
          if (linearScalingCalculator.ErrorState == OnlineCalculatorError.None) {
            var alpha = linearScalingCalculator.Alpha;
            var beta = linearScalingCalculator.Beta;
            for (i = 0; i < estimatedValues.Length; ++i) {
              estimatedValues[i] = estimatedValues[i] * beta + alpha;
            }
          }
        }
        // add estimated values to escope
        scope.Variables.Add(new Core.Variable("EstimatedValues", new DoubleArray(estimatedValues.LimitToRange(estimationLimits.Lower, estimationLimits.Upper).ToArray())));
      }
      return base.Apply();
    }
  }
}
