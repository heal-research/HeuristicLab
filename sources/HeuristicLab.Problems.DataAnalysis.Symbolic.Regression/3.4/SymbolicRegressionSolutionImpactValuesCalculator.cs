#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  public class SymbolicRegressionSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public override double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;

      return CalculateReplacementValue(node, regressionModel.SymbolicExpressionTree, regressionModel.Interpreter, regressionProblemData.Dataset, rows);
    }

    public override double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double originalQuality = double.NaN) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;

      var dataset = regressionProblemData.Dataset;
      var targetValues = dataset.GetDoubleValues(regressionProblemData.TargetVariable, rows);

      OnlineCalculatorError errorState;
      if (double.IsNaN(originalQuality)) {
        var originalClassValues = regressionModel.GetEstimatedValues(dataset, rows);
        originalQuality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, originalClassValues, out errorState);
        if (errorState != OnlineCalculatorError.None) originalQuality = 0.0;
      }

      var replacementValue = CalculateReplacementValue(regressionModel, node, regressionProblemData, rows);
      var constantNode = new ConstantTreeNode(new Constant()) { Value = replacementValue };
      var cloner = new Cloner();
      cloner.RegisterClonedObject(node, constantNode);
      var tempModel = cloner.Clone(regressionModel);

      var estimatedValues = tempModel.GetEstimatedValues(dataset, rows);
      double newQuality = OnlinePearsonsRSquaredCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) newQuality = 0.0;

      return originalQuality - newQuality;
    }

  }
}
