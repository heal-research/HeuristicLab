#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for symbolic data analysis models
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisModel : DataAnalysisModel, ISymbolicDataAnalysisModel {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    #region properties
    [Storable]
    private double lowerEstimationLimit;
    public double LowerEstimationLimit { get { return lowerEstimationLimit; } }
    [Storable]
    private double upperEstimationLimit;
    public double UpperEstimationLimit { get { return upperEstimationLimit; } }

    [Storable]
    private ISymbolicExpressionTree symbolicExpressionTree;
    public ISymbolicExpressionTree SymbolicExpressionTree {
      get { return symbolicExpressionTree; }
    }

    [Storable]
    private ISymbolicDataAnalysisExpressionTreeInterpreter interpreter;
    public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter {
      get { return interpreter; }
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get {
        var variables =
          SymbolicExpressionTree.IterateNodesPrefix()
            .OfType<IVariableTreeNode>()
            .Select(x => x.VariableName)
            .Distinct();

        return variables.OrderBy(x => x);
      }
    }

    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisModel(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisModel(SymbolicDataAnalysisModel original, Cloner cloner)
      : base(original, cloner) {
      this.symbolicExpressionTree = cloner.Clone(original.symbolicExpressionTree);
      this.interpreter = cloner.Clone(original.interpreter);
      this.lowerEstimationLimit = original.lowerEstimationLimit;
      this.upperEstimationLimit = original.upperEstimationLimit;
    }
    protected SymbolicDataAnalysisModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
       double lowerEstimationLimit, double upperEstimationLimit)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.symbolicExpressionTree = tree;
      this.interpreter = interpreter;
      this.lowerEstimationLimit = lowerEstimationLimit;
      this.upperEstimationLimit = upperEstimationLimit;
    }

    #region Scaling
    protected void Scale(IDataAnalysisProblemData problemData, string targetVariable) {
      var dataset = problemData.Dataset;
      var rows = problemData.TrainingIndices;
      var estimatedValues = Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows);
      var targetValues = dataset.GetDoubleValues(targetVariable, rows);

      var linearScalingCalculator = new OnlineLinearScalingParameterCalculator();
      var targetValuesEnumerator = targetValues.GetEnumerator();
      var estimatedValuesEnumerator = estimatedValues.GetEnumerator();
      while (targetValuesEnumerator.MoveNext() & estimatedValuesEnumerator.MoveNext()) {
        double target = targetValuesEnumerator.Current;
        double estimated = estimatedValuesEnumerator.Current;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated))
          linearScalingCalculator.Add(estimated, target);
      }
      if (linearScalingCalculator.ErrorState == OnlineCalculatorError.None && (targetValuesEnumerator.MoveNext() || estimatedValuesEnumerator.MoveNext()))
        throw new ArgumentException("Number of elements in target and estimated values enumeration do not match.");

      double alpha = linearScalingCalculator.Alpha;
      double beta = linearScalingCalculator.Beta;
      if (linearScalingCalculator.ErrorState != OnlineCalculatorError.None) return;

      ConstantTreeNode alphaTreeNode = null;
      ConstantTreeNode betaTreeNode = null;
      // check if model has been scaled previously by analyzing the structure of the tree
      var startNode = SymbolicExpressionTree.Root.GetSubtree(0);
      if (startNode.GetSubtree(0).Symbol is Addition) {
        var addNode = startNode.GetSubtree(0);
        if (addNode.SubtreeCount == 2 && addNode.GetSubtree(0).Symbol is Multiplication && addNode.GetSubtree(1).Symbol is Constant) {
          alphaTreeNode = addNode.GetSubtree(1) as ConstantTreeNode;
          var mulNode = addNode.GetSubtree(0);
          if (mulNode.SubtreeCount == 2 && mulNode.GetSubtree(1).Symbol is Constant) {
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
    #endregion

  }
}
