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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  /// <summary>
  /// Represents a symbolic time-series prognosis model
  /// </summary>
  [StorableClass]
  [Item(Name = "Symbolic Time-Series Prognosis Model", Description = "Represents a symbolic time series prognosis model.")]
  public class SymbolicTimeSeriesPrognosisModel : SymbolicDataAnalysisModel, ISymbolicTimeSeriesPrognosisModel {
    [Storable]
    private double lowerEstimationLimit;
    [Storable]
    private double upperEstimationLimit;

    [StorableConstructor]
    protected SymbolicTimeSeriesPrognosisModel(bool deserializing) : base(deserializing) { }
    protected SymbolicTimeSeriesPrognosisModel(SymbolicTimeSeriesPrognosisModel original, Cloner cloner)
      : base(original, cloner) {
      this.lowerEstimationLimit = original.lowerEstimationLimit;
      this.upperEstimationLimit = original.upperEstimationLimit;
    }
    public SymbolicTimeSeriesPrognosisModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter) {
      this.lowerEstimationLimit = lowerEstimationLimit;
      this.upperEstimationLimit = upperEstimationLimit;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisModel(this, cloner);
    }

    public IEnumerable<double> GetPrognosedValues(Dataset dataset, IEnumerable<int> rows) {
      return Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows)
        .LimitToRange(lowerEstimationLimit, upperEstimationLimit);
    }

    public ISymbolicTimeSeriesPrognosisSolution CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData) {
      return new SymbolicTimeSeriesPrognosisSolution(this, problemData);
    }
    ITimeSeriesPrognosisSolution ITimeSeriesPrognosisModel.CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData) {
      return CreateTimeSeriesPrognosisSolution(problemData);
    }

    public static void Scale(SymbolicTimeSeriesPrognosisModel model, ITimeSeriesPrognosisProblemData problemData) {
      var dataset = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;
      var rows = problemData.TrainingIndizes;
      var estimatedValues = model.Interpreter.GetSymbolicExpressionTreeValues(model.SymbolicExpressionTree, dataset, rows);
      var targetValues = dataset.GetDoubleValues(targetVariable, rows);
      double alpha;
      double beta;
      OnlineCalculatorError errorState;
      OnlineLinearScalingParameterCalculator.Calculate(estimatedValues, targetValues, out alpha, out beta, out errorState);
      if (errorState != OnlineCalculatorError.None) return;

      ConstantTreeNode alphaTreeNode = null;
      ConstantTreeNode betaTreeNode = null;
      // check if model has been scaled previously by analyzing the structure of the tree
      var startNode = model.SymbolicExpressionTree.Root.GetSubtree(0);
      if (startNode.GetSubtree(0).Symbol is Addition) {
        var addNode = startNode.GetSubtree(0);
        if (addNode.SubtreesCount == 2 && addNode.GetSubtree(0).Symbol is Multiplication && addNode.GetSubtree(1).Symbol is Constant) {
          alphaTreeNode = addNode.GetSubtree(1) as ConstantTreeNode;
          var mulNode = addNode.GetSubtree(0);
          if (mulNode.SubtreesCount == 2 && mulNode.GetSubtree(1).Symbol is Constant) {
            betaTreeNode = mulNode.GetSubtree(1) as ConstantTreeNode;
          }
        }
      }
      // if tree structure matches the structure necessary for linear scaling then reuse the existing tree nodes
      if (alphaTreeNode != null && betaTreeNode != null) {
        betaTreeNode.Value *= beta;
        alphaTreeNode.Value *= beta;
        alphaTreeNode.Value += alpha;
      } else {
        var mainBranch = startNode.GetSubtree(0);
        startNode.RemoveSubtree(0);
        var scaledMainBranch = MakeSum(MakeProduct(mainBranch, beta), alpha);
        startNode.AddSubtree(scaledMainBranch);
      }
    }

    private static ISymbolicExpressionTreeNode MakeSum(ISymbolicExpressionTreeNode treeNode, double alpha) {
      if (alpha.IsAlmost(0.0)) {
        return treeNode;
      } else {
        var addition = new Addition();
        var node = addition.CreateTreeNode();
        var alphaConst = MakeConstant(alpha);
        node.AddSubtree(treeNode);
        node.AddSubtree(alphaConst);
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode MakeProduct(ISymbolicExpressionTreeNode treeNode, double beta) {
      if (beta.IsAlmost(1.0)) {
        return treeNode;
      } else {
        var multipliciation = new Multiplication();
        var node = multipliciation.CreateTreeNode();
        var betaConst = MakeConstant(beta);
        node.AddSubtree(treeNode);
        node.AddSubtree(betaConst);
        return node;
      }
    }

    private static ISymbolicExpressionTreeNode MakeConstant(double c) {
      var node = (ConstantTreeNode)(new Constant()).CreateTreeNode();
      node.Value = c;
      return node;
    }
  }
}
