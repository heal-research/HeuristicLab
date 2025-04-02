#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using System.Runtime.InteropServices;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("5A5292D5-712D-448C-8A1C-0CBCD8DD1F14")]
  [Item("SymbolicExpressionTreePruningOperator", "An operator that replaces sub-trees with small impacts with numbers in a symbolic expression tree.")]
  public abstract class SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator : SingleSuccessorOperator, ISymbolicExpressionTreeOperator {
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
    private const string QualityParameterName = "Qualities"; // the quality 
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string InterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string SimplifyParameterName = "Simplify";
    #endregion

    #region parameter properties
    public ILookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter => (ILookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName];
    public ILookupParameter<DoubleArray> QualityParameter => (ILookupParameter<DoubleArray>)Parameters[QualityParameterName];
    public ILookupParameter<IDataAnalysisProblemData> ProblemDataParameter => (ILookupParameter<IDataAnalysisProblemData>)Parameters[ProblemDataParameterName];
    public IValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator> ImpactValuesCalculatorParameter => (IValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>)Parameters[ImpactValuesCalculatorParameterName];
    public ILookupParameter<IntRange> FitnessCalculationPartitionParameter => (ILookupParameter<IntRange>)Parameters[FitnessCalculationPartitionParameterName];
    public ILookupParameter<IntValue> PrunedSubtreesParameter => (ILookupParameter<IntValue>)Parameters[PrunedSubtreesParameterName];
    public ILookupParameter<IntValue> PrunedTreesParameter => (ILookupParameter<IntValue>)Parameters[PrunedTreesParameterName];
    public ILookupParameter<IntValue> PrunedNodesParameter => (ILookupParameter<IntValue>)Parameters[PrunedNodesParameterName];
    public IFixedValueParameter<DoubleValue> NodeImpactThresholdParameter => (IFixedValueParameter<DoubleValue>)Parameters[NodeImpactThresholdParameterName];
    public IFixedValueParameter<BoolValue> PruneOnlyZeroImpactNodesParameter => (IFixedValueParameter<BoolValue>)Parameters[PruneOnlyZeroImpactNodesParameterName];
    public ILookupParameter<DoubleLimit> EstimationLimitsParameter => (ILookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName];
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter => (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName];
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter => (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName];
    public IValueParameter<BoolValue> SimplifyParameter => (IValueParameter<BoolValue>)Parameters[SimplifyParameterName];
    #endregion

    #region properties
    public ISymbolicDataAnalysisSolutionImpactValuesCalculator ImpactValuesCalculator {
      get => ImpactValuesCalculatorParameter.Value;
      set => ImpactValuesCalculatorParameter.Value = value;
    }
    public bool PruneOnlyZeroImpactNodes {
      get => PruneOnlyZeroImpactNodesParameter.Value.Value;
      set => PruneOnlyZeroImpactNodesParameter.Value.Value = value;
    }
    public double NodeImpactThreshold {
      get => NodeImpactThresholdParameter.Value.Value;
      set => NodeImpactThresholdParameter.Value.Value = value;
    }

    public bool Simplify {
      get => SimplifyParameter.Value.Value;
      set => SimplifyParameter.Value.Value = value;
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator(SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator original, Cloner cloner)
      : base(original, cloner) { }

    protected SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator) {
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
      Parameters.Add(new LookupParameter<DoubleArray>(QualityParameterName));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, impactValuesCalculator));
      Parameters.Add(new ValueParameter<BoolValue>(SimplifyParameterName, new BoolValue(false)));
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

    protected abstract double[] Evaluate(IDataAnalysisModel model);

    public override IOperation Apply() {
      var tree = SymbolicExpressionTreeParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      var fitnessCalculationPartition = FitnessCalculationPartitionParameter.ActualValue;
      var estimationLimits = EstimationLimitsParameter.ActualValue;
      var interpreter = InterpreterParameter.ActualValue;


      var simplify = Simplify;
      if (simplify) SymbolicExpressionTreeParameter.ActualValue = tree = TreeSimplifier.Simplify(tree);

      var model = CreateModel(tree, interpreter, problemData, estimationLimits);
      var nodes = tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix().ToList();
      var rows = Enumerable.Range(fitnessCalculationPartition.Start, fitnessCalculationPartition.Size).ToList();
      var prunedSubtrees = 0;
      var prunedTrees = 0;
      var prunedNodes = 0;


      //var q1= ImpactValuesCalculator.CalculateQualityForImpacts(model, problemData, rows);
      //var unused = Enumerable.Range(0, nodes.Count).Where(i => !(nodes[i] is INumericTreeNode)).ToHashSet();
      //while (unused.Count>0) {
      //  var (node, impactValue, replacementValue, index, nextQuality) = unused.Select(n=> {
      //    ImpactValuesCalculator.CalculateImpactAndReplacementValues(model, nodes[n], problemData, rows,
      //      out var res, out var rep , out var nq, q1);
      //    return (nodes[n],res, rep, n, nq);
      //  }).OrderBy(x=>x.res).First();

      //  switch (PruneOnlyZeroImpactNodes) {
      //    case true when !impactValue.IsAlmost(0.0):
      //    case false when impactValue > NodeImpactThreshold:
      //      break;
      //  }
      //  var numberNode = (NumberTreeNode)node.Grammar.GetSymbol("Number").CreateTreeNode();
      //  numberNode.Value = replacementValue;
      //  var length = node.GetLength();
      //  ReplaceWithNumber(node, numberNode);
      //  unused.ExceptWith(Enumerable.Range(index, length)); // skip subtrees under the node that was folded
      //  prunedSubtrees++;
      //  prunedNodes += length;
      //  q1 = nextQuality;
      //}


      var qualityForImpactsCalculation = ImpactValuesCalculator.CalculateQualityForImpacts(model, problemData, rows);
      for (var i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is INumericTreeNode) continue;

        ImpactValuesCalculator.CalculateImpactAndReplacementValues(model, node, problemData, rows,
          out var impactValue, out var replacementValue, out var newQualityForImpacts, qualityForImpactsCalculation);

        switch (PruneOnlyZeroImpactNodes) {
          case true when !impactValue.IsAlmost(0.0):
          case false when impactValue > NodeImpactThreshold:
            continue;
        }

        var numberNode = (NumberTreeNode)node.Grammar.GetSymbol("Number").CreateTreeNode();
        numberNode.Value = replacementValue;

        var length = node.GetLength();
        ReplaceWithNumber(node, numberNode);
        i += length - 1; // skip subtrees under the node that was folded

        prunedSubtrees++;
        prunedNodes += length;

        qualityForImpactsCalculation = newQualityForImpacts;
      }

      if (prunedSubtrees > 0) prunedTrees = 1;
      PrunedSubtreesParameter.ActualValue = new IntValue(prunedSubtrees);
      PrunedTreesParameter.ActualValue = new IntValue(prunedTrees);
      PrunedNodesParameter.ActualValue = new IntValue(prunedNodes);

      if (prunedSubtrees <= 0 && !simplify) return base.Apply();
      // if nothing was pruned then there's no need to re-evaluate the tree
      var q = Evaluate(model);
      var q2 = QualityParameter.ActualValue;
      if (q.Length != q2.Length) throw new InvalidOperationException();
      for (var i = 0; i < q.Length; i++) q2[i] = q[i] ;
      return base.Apply();
    }

    protected static void ReplaceWithNumber(ISymbolicExpressionTreeNode original, ISymbolicExpressionTreeNode replacement) {
      var parent = original.Parent;
      var i = parent.IndexOfSubtree(original);
      parent.RemoveSubtree(i);
      parent.InsertSubtree(i, replacement);
    }
  }
}
