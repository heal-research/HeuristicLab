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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationPruningOperator", "An operator which prunes symbolic classificaton trees.")]
  public class SymbolicClassificationPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ModelCreatorParameterName = "ModelCreator";
    private const string EvaluatorParameterName = "Evaluator";

    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }

    public ILookupParameter<ISymbolicClassificationSingleObjectiveEvaluator> EvaluatorParameter {
      get {
        return (ILookupParameter<ISymbolicClassificationSingleObjectiveEvaluator>)Parameters[EvaluatorParameterName];
      }
    }
    #endregion

    protected SymbolicClassificationPruningOperator(SymbolicClassificationPruningOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicClassificationPruningOperator(this, cloner); }

    [StorableConstructor]
    protected SymbolicClassificationPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicClassificationPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator)
      : base(impactValuesCalculator) {
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
      Parameters.Add(new LookupParameter<ISymbolicClassificationSingleObjectiveEvaluator>(EvaluatorParameterName));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      base.ImpactValuesCalculator = new SymbolicClassificationSolutionImpactValuesCalculator();
      if (!Parameters.ContainsKey(EvaluatorParameterName)) {
        Parameters.Add(new LookupParameter<ISymbolicClassificationSingleObjectiveEvaluator>(EvaluatorParameterName));
      }
      #endregion
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, DoubleLimit estimationLimits) {
      var classificationProblemData = (IClassificationProblemData)problemData;
      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel(classificationProblemData.TargetVariable, tree, interpreter, estimationLimits.Lower, estimationLimits.Upper);

      var rows = classificationProblemData.TrainingIndices;
      model.RecalculateModelParameters(classificationProblemData, rows);
      return model;
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var evaluator = EvaluatorParameter.ActualValue;
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)ProblemDataParameter.ActualValue;
      var rows = Enumerable.Range(FitnessCalculationPartitionParameter.ActualValue.Start, FitnessCalculationPartitionParameter.ActualValue.Size);
      return evaluator.Evaluate(this.ExecutionContext, classificationModel.SymbolicExpressionTree, classificationProblemData, rows);
    }

    public static ISymbolicExpressionTree Prune(ISymbolicExpressionTree tree, ISymbolicClassificationModelCreator modelCreator,
      SymbolicClassificationSolutionImpactValuesCalculator impactValuesCalculator, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      IClassificationProblemData problemData, DoubleLimit estimationLimits, IEnumerable<int> rows,
      double nodeImpactThreshold = 0.0, bool pruneOnlyZeroImpactNodes = false) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();
      var model = modelCreator.CreateSymbolicClassificationModel(problemData.TargetVariable, clonedTree, interpreter, estimationLimits.Lower, estimationLimits.Upper);

      var nodes = clonedTree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList();
      double qualityForImpactsCalculation = double.NaN;

      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is ConstantTreeNode) continue;

        double impactValue, replacementValue, newQualityForImpactsCalculation;
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
