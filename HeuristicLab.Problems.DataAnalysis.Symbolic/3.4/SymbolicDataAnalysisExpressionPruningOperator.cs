#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("DBE27385-2E8C-4314-88B4-F96A5240FC9D")]
  [Item("SymbolicExpressionTreePruningOperator", "An operator that replaces introns with constant values in a symbolic expression tree.")]
  public abstract class SymbolicDataAnalysisExpressionPruningOperator : SingleSuccessorOperator, ISymbolicExpressionTreeOperator {
    #region parameter names
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisModelParameterName = "SymbolicDataAnalysisModel";
    private const string ImpactValuesCalculatorParameterName = "ImpactValuesCalculator";
    private const string PrunedSubtreesParameterName = "PrunedSubtrees";
    private const string PrunedTreesParameterName = "PrunedTrees";
    private const string PrunedNodesParameterName = "PrunedNodes";
    private const string FitnessCalculationPartitionParameterName = "FitnessCalculationPartition";
    private const string NodeImpactThresholdParameterName = "ImpactThreshold";
    private const string PruneOnlyZeroImpactNodesParameterName = "PruneOnlyZeroImpactNodes";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree"; // the tree to be pruned
    private const string QualityParameterName = "Quality"; // the quality 
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string InterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    #endregion

    #region parameter properties
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<IDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator> ImpactValuesCalculatorParameter {
      get { return (IValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>)Parameters[ImpactValuesCalculatorParameterName]; }
    }
    public ILookupParameter<IntRange> FitnessCalculationPartitionParameter {
      get { return (ILookupParameter<IntRange>)Parameters[FitnessCalculationPartitionParameterName]; }
    }
    public ILookupParameter<IntValue> PrunedSubtreesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[PrunedSubtreesParameterName]; }
    }
    public ILookupParameter<IntValue> PrunedTreesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[PrunedTreesParameterName]; }
    }
    public ILookupParameter<IntValue> PrunedNodesParameter {
      get { return (ILookupParameter<IntValue>)Parameters[PrunedNodesParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> NodeImpactThresholdParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NodeImpactThresholdParameterName]; }
    }
    public IFixedValueParameter<BoolValue> PruneOnlyZeroImpactNodesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[PruneOnlyZeroImpactNodesParameterName]; }
    }
    public ILookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (ILookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName]; }
    }
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion

    #region properties
    public ISymbolicDataAnalysisSolutionImpactValuesCalculator ImpactValuesCalculator {
      get { return ImpactValuesCalculatorParameter.Value; }
      set { ImpactValuesCalculatorParameter.Value = value; }
    }
    public bool PruneOnlyZeroImpactNodes {
      get { return PruneOnlyZeroImpactNodesParameter.Value.Value; }
      set { PruneOnlyZeroImpactNodesParameter.Value.Value = value; }
    }
    public double NodeImpactThreshold {
      get { return NodeImpactThresholdParameter.Value.Value; }
      set { NodeImpactThresholdParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionPruningOperator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisExpressionPruningOperator(SymbolicDataAnalysisExpressionPruningOperator original, Cloner cloner)
      : base(original, cloner) { }

    protected SymbolicDataAnalysisExpressionPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator) {
      #region add parameters
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisModel>(SymbolicDataAnalysisModelParameterName));
      Parameters.Add(new LookupParameter<IntRange>(FitnessCalculationPartitionParameterName));
      Parameters.Add(new LookupParameter<IntValue>(PrunedNodesParameterName, "A counter of how many nodes were pruned."));
      Parameters.Add(new LookupParameter<IntValue>(PrunedSubtreesParameterName, "A counter of how many subtrees were replaced."));
      Parameters.Add(new LookupParameter<IntValue>(PrunedTreesParameterName, "A counter of how many trees were pruned."));
      Parameters.Add(new FixedValueParameter<BoolValue>(PruneOnlyZeroImpactNodesParameterName, "Specify whether or not only zero impact nodes should be pruned."));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NodeImpactThresholdParameterName, "Specifies an impact value threshold below which nodes should be pruned."));
      Parameters.Add(new LookupParameter<DoubleLimit>(EstimationLimitsParameterName));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(InterpreterParameterName));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, impactValuesCalculator));
      #endregion
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(PrunedNodesParameterName)) {
        Parameters.Add(new LookupParameter<IntValue>(PrunedNodesParameterName, "A counter of how many nodes were pruned."));
      }
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName));
      }
      if (!Parameters.ContainsKey(ImpactValuesCalculatorParameterName)) {
        // value must be set by derived operators (regression/classification)
        Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName));
      }
      #endregion
    }

    protected abstract ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, DoubleLimit estimationLimits);

    protected abstract double Evaluate(IDataAnalysisModel model);

    public override IOperation Apply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var fitnessCalculationPartition = FitnessCalculationPartitionParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var interpreter = InterpreterParameter.ActualValue;

      var model = CreateModel(tree, interpreter, problemData, estimationLimits);
      var nodes = tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList();
      var rows = Enumerable.Range(fitnessCalculationPartition.Start, fitnessCalculationPartition.Size).ToList();
      var prunedSubtrees = 0;
      var prunedTrees = 0;
      var prunedNodes = 0;

      double qualityForImpactsCalculation = double.NaN;

      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is ConstantTreeNode) continue;

        double impactValue, replacementValue;
        double newQualityForImpacts;
        ImpactValuesCalculator.CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, out newQualityForImpacts, qualityForImpactsCalculation);

        if (PruneOnlyZeroImpactNodes && !impactValue.IsAlmost(0.0)) continue;
        if (!PruneOnlyZeroImpactNodes && impactValue > NodeImpactThreshold) continue;

        var constantNode = (ConstantTreeNode)node.Grammar.GetSymbol("Constant").CreateTreeNode();
        constantNode.Value = replacementValue;

        var length = node.GetLength();
        ReplaceWithConstant(node, constantNode);
        i += length - 1; // skip subtrees under the node that was folded

        prunedSubtrees++;
        prunedNodes += length;

        qualityForImpactsCalculation = newQualityForImpacts;
      }

      if (prunedSubtrees > 0) prunedTrees = 1;
      PrunedSubtreesParameter.ActualValue = new IntValue(prunedSubtrees);
      PrunedTreesParameter.ActualValue = new IntValue(prunedTrees);
      PrunedNodesParameter.ActualValue = new IntValue(prunedNodes);

      if (prunedSubtrees > 0) // if nothing was pruned then there's no need to re-evaluate the tree
        QualityParameter.ActualValue.Value = Evaluate(model);

      return base.Apply();
    }

    protected static void ReplaceWithConstant(ISymbolicExpressionTreeNode original, ISymbolicExpressionTreeNode replacement) {
      var parent = original.Parent;
      var i = parent.IndexOfSubtree(original);
      parent.RemoveSubtree(i);
      parent.InsertSubtree(i, replacement);
    }
  }
}
