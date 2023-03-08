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
  [Item("ROC-AUC Evaluator", "Calculates the area under (AUC) of the Receiver Operating Characteristic (ROC) curve of a symbolic classification solution")]
  [StorableType("F2C51D51-5D76-457F-BECE-96C9A51630EC")]
  public class SymbolicClassificationSingleObjectiveRocAucEvaluator : SymbolicClassificationSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveRocAucEvaluator(StorableConstructorFlag _) : base(_) { }

    protected SymbolicClassificationSingleObjectiveRocAucEvaluator(SymbolicClassificationSingleObjectiveRocAucEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveRocAucEvaluator(this, cloner);
    }

    public SymbolicClassificationSingleObjectiveRocAucEvaluator() : base() { }

    public override bool Maximization { get { return true; } }

    public override IOperation InstrumentedApply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      var solution = SymbolicExpressionTreeParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      double quality = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, solution, ProblemDataParameter.ActualValue, rows, estimationLimits.Lower, estimationLimits.Upper);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, ISymbolicExpressionTree solution, IClassificationProblemData problemData, IEnumerable<int> rows, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      var estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows).LimitToRange(lowerEstimationLimit, upperEstimationLimit).ToList();
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows).ToList();
      
      var auc = RocAucCalculator.CalculateRocAuc(targetValues, estimatedValues, problemData.PositiveClass, problemData.ClassValues.ToList(), problemData.ClassNames.ToList());
      return auc;
    }
    
    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
    
      double mse = Calculate(SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, tree, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;

      return mse;
    }
    
    public override double Evaluate(
      ISymbolicExpressionTree tree, 
      IClassificationProblemData problemData, 
      IEnumerable<int> rows, 
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true, 
      double lowerEstimationLimit = double.MinValue, 
      double upperEstimationLimit = double.MaxValue) {
      return Calculate(interpreter, tree, problemData, rows, lowerEstimationLimit, upperEstimationLimit);
    }
  }
}
