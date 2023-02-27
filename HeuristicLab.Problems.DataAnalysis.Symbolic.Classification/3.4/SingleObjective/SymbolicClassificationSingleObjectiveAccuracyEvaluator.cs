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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Accuracy Evaluator", "Calculates the accuracy of a symbolic classification solution")]
  [StorableType("D8D48403-1EDE-499A-BD1C-D0B8EAC8894E")]
  public class SymbolicClassificationSingleObjectiveAccuracyEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    public SymbolicClassificationSingleObjectiveAccuracyEvaluator() 
      : base() { }
    
    protected SymbolicClassificationSingleObjectiveAccuracyEvaluator(SymbolicClassificationSingleObjectiveAccuracyEvaluator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveAccuracyEvaluator(this, cloner);
    }
    
    public override bool Maximization { get { return true; } }

    public override IOperation InstrumentedApply() {
      double quality = Evaluate(ExecutionContext, SymbolicExpressionTreeParameter.ActualValue, ProblemDataParameter.ActualValue, GenerateRowsToEvaluate());
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicClassificationModel model, IClassificationProblemData problemData, IEnumerable<int> rows) {
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows).ToList();
      var estimatedClassValues = model.GetEstimatedClassValues(problemData.Dataset, rows).ToList();
      
      var accuracy = OnlineAccuracyCalculator.Calculate(targetValues, estimatedClassValues, out var errorState);
      return errorState == OnlineCalculatorError.None ? accuracy : double.NaN;
    }
    
    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;
      
      var modelCreator = new AccuracyMaximizingThresholdsModelCreator();
      var model = modelCreator.CreateSymbolicClassificationModel(problemData.TargetVariable, tree, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(problemData);
      model.RecalculateModelParameters(problemData, rows);
      double accuracy = Calculate(model, problemData, rows);
      
      EstimationLimitsParameter.ExecutionContext = null;
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return accuracy;
    }
    
    public override double Evaluate(
      ISymbolicExpressionTree tree, 
      IClassificationProblemData problemData, 
      IEnumerable<int> rows, 
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true, 
      double lowerEstimationLimit = double.MinValue, 
      double upperEstimationLimit = double.MaxValue) {
      var modelCreator = new AccuracyMaximizingThresholdsModelCreator();
      var model = modelCreator.CreateSymbolicClassificationModel(problemData.TargetVariable, tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
      if (applyLinearScaling) model.Scale(problemData);
      model.RecalculateModelParameters(problemData, rows);
      return Calculate(model, problemData, rows);
    }
  }
}
