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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableClass]
  [Item("SymbolicRegressionPruningOperator", "An operator which prunes symbolic regression trees.")]
  public class SymbolicRegressionPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string EvaluatorParameterName = "Evaluator";

    #region parameter properties
    public ILookupParameter<ISymbolicRegressionSingleObjectiveEvaluator> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicRegressionSingleObjectiveEvaluator>)Parameters[EvaluatorParameterName]; }
    }
    #endregion

    protected SymbolicRegressionPruningOperator(SymbolicRegressionPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicRegressionPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicRegressionPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator)
      : base(impactValuesCalculator) {
      Parameters.Add(new LookupParameter<ISymbolicRegressionSingleObjectiveEvaluator>(EvaluatorParameterName));
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, DoubleLimit estimationLimits) {
      return new SymbolicRegressionModel(tree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)ProblemDataParameter.ActualValue;
      var evaluator = EvaluatorParameter.ActualValue;
      var fitnessEvaluationPartition = FitnessCalculationPartitionParameter.ActualValue;
      var rows = Enumerable.Range(fitnessEvaluationPartition.Start, fitnessEvaluationPartition.Size);
      return evaluator.Evaluate(this.ExecutionContext, regressionModel.SymbolicExpressionTree, regressionProblemData, rows);
    }

    public static ISymbolicExpressionTree Prune(ISymbolicExpressionTree tree, SymbolicRegressionSolutionImpactValuesCalculator impactValuesCalculator, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IRegressionProblemData problemData, DoubleLimit estimationLimits, IEnumerable<int> rows, double nodeImpactThreshold = 0.0, bool pruneOnlyZeroImpactNodes = false) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();
      var model = new SymbolicRegressionModel(clonedTree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
      var nodes = clonedTree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList(); // skip the nodes corresponding to the ProgramRootSymbol and the StartSymbol

      double qualityForImpactsCalculation = double.NaN; // pass a NaN value initially so the impact calculator will calculate the quality

      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is ConstantTreeNode) continue;

        double impactValue, replacementValue;
        double newQualityForImpactsCalculation;
        impactValuesCalculator.CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, out newQualityForImpactsCalculation, qualityForImpactsCalculation);

        if (pruneOnlyZeroImpactNodes && !impactValue.IsAlmost(0.0)) continue;
        if (!pruneOnlyZeroImpactNodes && impactValue > nodeImpactThreshold) continue;

        var constantNode = (ConstantTreeNode)node.Grammar.GetSymbol("Constant").CreateTreeNode();
        constantNode.Value = replacementValue;

        ReplaceWithConstant(node, constantNode);
        i += node.GetLength() - 1; // skip subtrees under the node that was folded

        qualityForImpactsCalculation = newQualityForImpactsCalculation;
      }
      return model.SymbolicExpressionTree;
    }
  }
}
