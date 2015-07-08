#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public override double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;

      return CalculateReplacementValue(node, regressionModel.SymbolicExpressionTree, regressionModel.Interpreter, regressionProblemData.Dataset, rows);
    }

    public override double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double originalQuality = double.NaN) {
      double impactValue, replacementValue;
      CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, originalQuality);
      return impactValue;
    }

    public override void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node,
      IDataAnalysisProblemData problemData, IEnumerable<int> rows, out double impactValue, out double replacementValue,
      double originalQuality = Double.NaN) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;

      var dataset = regressionProblemData.Dataset;
      var targetValues = dataset.GetDoubleValues(regressionProblemData.TargetVariable, rows);

      OnlineCalculatorError errorState;
      if (double.IsNaN(originalQuality)) {
        var originalValues = regressionModel.GetEstimatedValues(dataset, rows);
        originalQuality = OnlinePearsonsRCalculator.Calculate(targetValues, originalValues, out errorState);
        if (errorState != OnlineCalculatorError.None) originalQuality = 0.0;
      }

      replacementValue = CalculateReplacementValue(regressionModel, node, regressionProblemData, rows);
      var constantNode = new ConstantTreeNode(new Constant()) { Value = replacementValue };

      var cloner = new Cloner();
      var tempModel = cloner.Clone(regressionModel);
      var tempModelNode = (ISymbolicExpressionTreeNode)cloner.GetClone(node);

      var tempModelParentNode = tempModelNode.Parent;
      int i = tempModelParentNode.IndexOfSubtree(tempModelNode);
      tempModelParentNode.RemoveSubtree(i);
      tempModelParentNode.InsertSubtree(i, constantNode);

      var estimatedValues = tempModel.GetEstimatedValues(dataset, rows);
      double newQuality = OnlinePearsonsRCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) newQuality = 0.0;

      impactValue = (originalQuality*originalQuality) - (newQuality*newQuality);
    }
  }
}
