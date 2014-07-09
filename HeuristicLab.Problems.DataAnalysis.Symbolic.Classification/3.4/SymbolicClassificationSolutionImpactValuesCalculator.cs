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

    public override double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;

      return CalculateReplacementValue(node, classificationModel.SymbolicExpressionTree, classificationModel.Interpreter, classificationProblemData.Dataset, rows);
    }

    public override double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double originalQuality = double.NaN) {
      double impactValue, replacementValue;
      CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, originalQuality);
      return impactValue;
    }

    public override void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node,
      IDataAnalysisProblemData problemData, IEnumerable<int> rows, out double impactValue, out double replacementValue,
      double originalQuality = Double.NaN) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;

      var dataset = classificationProblemData.Dataset;
      var targetClassValues = dataset.GetDoubleValues(classificationProblemData.TargetVariable, rows);

      OnlineCalculatorError errorState;
      if (double.IsNaN(originalQuality)) {
        var originalClassValues = classificationModel.GetEstimatedClassValues(dataset, rows);
        originalQuality = OnlineAccuracyCalculator.Calculate(targetClassValues, originalClassValues, out errorState);
        if (errorState != OnlineCalculatorError.None) originalQuality = 0.0;
      }

      replacementValue = CalculateReplacementValue(classificationModel, node, classificationProblemData, rows);
      var constantNode = new ConstantTreeNode(new Constant()) { Value = replacementValue };

      var cloner = new Cloner();
      var tempModel = cloner.Clone(classificationModel);
      var tempModelNode = (ISymbolicExpressionTreeNode)cloner.GetClone(node);

      var tempModelParentNode = tempModelNode.Parent;
      int i = tempModelParentNode.IndexOfSubtree(tempModelNode);
      tempModelParentNode.RemoveSubtree(i);
      tempModelParentNode.InsertSubtree(i, constantNode);

      var estimatedClassValues = tempModel.GetEstimatedClassValues(dataset, rows);
      double newQuality = OnlineAccuracyCalculator.Calculate(targetClassValues, estimatedClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) newQuality = 0.0;

      impactValue = originalQuality - newQuality;
    }
  }
}
