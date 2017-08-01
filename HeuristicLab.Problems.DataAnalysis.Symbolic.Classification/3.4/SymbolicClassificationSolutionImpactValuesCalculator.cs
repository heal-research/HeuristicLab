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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for classification problems.")]
  public class SymbolicClassificationSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public SymbolicClassificationSolutionImpactValuesCalculator() { }
    protected SymbolicClassificationSolutionImpactValuesCalculator(SymbolicClassificationSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSolutionImpactValuesCalculator(this, cloner);
    }
    [StorableConstructor]
    protected SymbolicClassificationSolutionImpactValuesCalculator(bool deserializing) : base(deserializing) { }

    public override void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model,
      ISymbolicExpressionTreeNode node,
      IDataAnalysisProblemData problemData, IEnumerable<int> rows, out double impactValue, out double replacementValue,
      out double newQualityForImpactsCalculation,
      double qualityForImpactsCalculation = Double.NaN) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;

      if (double.IsNaN(qualityForImpactsCalculation))
        qualityForImpactsCalculation = CalculateQualityForImpacts(classificationModel, classificationProblemData, rows);


      var cloner = new Cloner();
      var tempModel = cloner.Clone(classificationModel);
      var tempModelNode = (ISymbolicExpressionTreeNode)cloner.GetClone(node);

      var tempModelParentNode = tempModelNode.Parent;
      int i = tempModelParentNode.IndexOfSubtree(tempModelNode);
      double bestReplacementValue = 0.0;
      double bestImpactValue = double.PositiveInfinity;
      newQualityForImpactsCalculation = qualityForImpactsCalculation; // initialize
      // try the potentially reasonable replacement values and use the best one
      foreach (var repValue in CalculateReplacementValues(node, classificationModel.SymbolicExpressionTree, classificationModel.Interpreter, classificationProblemData.Dataset, classificationProblemData.TrainingIndices)) {
        tempModelParentNode.RemoveSubtree(i);

        var constantNode = new ConstantTreeNode(new Constant()) { Value = repValue };
        tempModelParentNode.InsertSubtree(i, constantNode);

        var dataset = classificationProblemData.Dataset;
        var targetClassValues = dataset.GetDoubleValues(classificationProblemData.TargetVariable, rows);
        var estimatedClassValues = tempModel.GetEstimatedClassValues(dataset, rows);
        OnlineCalculatorError errorState;
        newQualityForImpactsCalculation = OnlineAccuracyCalculator.Calculate(targetClassValues, estimatedClassValues,
          out errorState);
        if (errorState != OnlineCalculatorError.None) newQualityForImpactsCalculation = 0.0;

        impactValue = qualityForImpactsCalculation - newQualityForImpactsCalculation;

        if (impactValue < bestImpactValue) {
          bestImpactValue = impactValue;
          bestReplacementValue = repValue;
        }
      }
      replacementValue = bestReplacementValue;
      impactValue = bestImpactValue;
    }

    public static double CalculateQualityForImpacts(ISymbolicClassificationModel model, IClassificationProblemData problemData, IEnumerable<int> rows) {
      OnlineCalculatorError errorState;
      var dataset = problemData.Dataset;
      var targetClassValues = dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var originalClassValues = model.GetEstimatedClassValues(dataset, rows);
      var qualityForImpactsCalculation = OnlineAccuracyCalculator.Calculate(targetClassValues, originalClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) qualityForImpactsCalculation = 0.0;

      return qualityForImpactsCalculation;
    }
  }
}
