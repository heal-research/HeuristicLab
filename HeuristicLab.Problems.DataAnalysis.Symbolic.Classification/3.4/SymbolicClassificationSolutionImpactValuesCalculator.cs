#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  public class SymbolicClassificationSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public override double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;

      return CalculateReplacementValue(node, classificationModel.SymbolicExpressionTree, classificationModel.Interpreter, classificationProblemData.Dataset, rows);
    }

    public override double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double originalQuality = double.NaN) {
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

      var replacementValue = CalculateReplacementValue(classificationModel, node, classificationProblemData, rows);
      var constantNode = new ConstantTreeNode(new Constant()) { Value = replacementValue };
      var cloner = new Cloner();
      cloner.RegisterClonedObject(node, constantNode);
      var tempModel = cloner.Clone(classificationModel);
      tempModel.RecalculateModelParameters(classificationProblemData, rows);

      var estimatedClassValues = tempModel.GetEstimatedClassValues(dataset, rows);
      double newQuality = OnlineAccuracyCalculator.Calculate(targetClassValues, estimatedClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) newQuality = 0.0;

      return originalQuality - newQuality;
    }

  }
}
