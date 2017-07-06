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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  [Item("SymbolicRegressionSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for regression problems.")]
  public class SymbolicRegressionSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public SymbolicRegressionSolutionImpactValuesCalculator() { }

    protected SymbolicRegressionSolutionImpactValuesCalculator(SymbolicRegressionSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolutionImpactValuesCalculator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicRegressionSolutionImpactValuesCalculator(bool deserializing) : base(deserializing) { }

    public override void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node,
      IDataAnalysisProblemData problemData, IEnumerable<int> rows, out double impactValue, out double replacementValue, out double newQualityForImpactsCalculation,
      double qualityForImpactsCalculation = double.NaN) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;

      var dataset = regressionProblemData.Dataset;
      var targetValues = dataset.GetDoubleValues(regressionProblemData.TargetVariable, rows);

      if (double.IsNaN(qualityForImpactsCalculation))
        qualityForImpactsCalculation = CalculateQualityForImpacts(regressionModel, regressionProblemData, rows);

      var cloner = new Cloner();
      var tempModel = cloner.Clone(regressionModel);
      var tempModelNode = (ISymbolicExpressionTreeNode)cloner.GetClone(node);

      var tempModelParentNode = tempModelNode.Parent;
      int i = tempModelParentNode.IndexOfSubtree(tempModelNode);

      double bestReplacementValue = 0.0;
      double bestImpactValue = double.PositiveInfinity;
      newQualityForImpactsCalculation = qualityForImpactsCalculation; // initialize
      // try the potentially reasonable replacement values and use the best one
      foreach (var repValue in CalculateReplacementValues(node, regressionModel.SymbolicExpressionTree, regressionModel.Interpreter, regressionProblemData.Dataset, regressionProblemData.TrainingIndices)) {

        tempModelParentNode.RemoveSubtree(i);

        var constantNode = new ConstantTreeNode(new Constant()) { Value = repValue };

        tempModelParentNode.InsertSubtree(i, constantNode);

        var estimatedValues = tempModel.GetEstimatedValues(dataset, rows);
        OnlineCalculatorError errorState;
        double r = OnlinePearsonsRCalculator.Calculate(targetValues, estimatedValues, out errorState);
        if (errorState != OnlineCalculatorError.None) r = 0.0;
        newQualityForImpactsCalculation = r * r;

        impactValue = qualityForImpactsCalculation - newQualityForImpactsCalculation;
        if (impactValue < bestImpactValue) {
          bestImpactValue = impactValue;
          bestReplacementValue = repValue;
        }
      }
      replacementValue = bestReplacementValue;
      impactValue = bestImpactValue;
    }

    public static double CalculateQualityForImpacts(ISymbolicRegressionModel model, IRegressionProblemData problemData, IEnumerable<int> rows) {
      var estimatedValues = model.GetEstimatedValues(problemData.Dataset, rows); // also bounds the values
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;
      var r = OnlinePearsonsRCalculator.Calculate(targetValues, estimatedValues, out errorState);
      var quality = r * r;
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
