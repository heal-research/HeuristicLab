#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using System;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers {
  public class SymbolicRegressionTournamentPruning : SingleSuccessorOperator, ISymbolicRegressionAnalyzer {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string MaxPruningRatioParameterName = "MaxPruningRatio";
    private const string TournamentSizeParameterName = "TournamentSize";
    private const string PopulationPercentileStartParameterName = "PopulationPercentileStart";
    private const string PopulationPercentileEndParameterName = "PopulationPercentileEnd";
    private const string QualityGainWeightParameterName = "QualityGainWeight";
    private const string IterationsParameterName = "Iterations";
    private const string FirstPruningGenerationParameterName = "FirstPruningGeneration";
    private const string PruningFrequencyParameterName = "PruningFrequency";
    private const string GenerationParameterName = "Generations";
    private const string ResultsParameterName = "Results";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> MaxPruningRatioParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[MaxPruningRatioParameterName]; }
    }
    public IValueLookupParameter<IntValue> TournamentSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[TournamentSizeParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> PopulationPercentileStartParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[PopulationPercentileStartParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> PopulationPercentileEndParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[PopulationPercentileEndParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> QualityGainWeightParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[QualityGainWeightParameterName]; }
    }
    public IValueLookupParameter<IntValue> IterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IValueLookupParameter<IntValue> FirstPruningGenerationParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[FirstPruningGenerationParameterName]; }
    }
    public IValueLookupParameter<IntValue> PruningFrequencyParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[PruningFrequencyParameterName]; }
    }
    public ILookupParameter<IntValue> GenerationParameter {
      get { return (ILookupParameter<IntValue>)Parameters[GenerationParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.ActualValue; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.ActualValue; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.ActualValue; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public DoubleValue MaxPruningRatio {
      get { return MaxPruningRatioParameter.ActualValue; }
    }
    public IntValue TournamentSize {
      get { return TournamentSizeParameter.ActualValue; }
    }
    public DoubleValue PopulationPercentileStart {
      get { return PopulationPercentileStartParameter.ActualValue; }
    }
    public DoubleValue PopulationPercentileEnd {
      get { return PopulationPercentileEndParameter.ActualValue; }
    }
    public DoubleValue QualityGainWeight {
      get { return QualityGainWeightParameter.ActualValue; }
    }
    public IntValue Iterations {
      get { return IterationsParameter.ActualValue; }
    }
    public IntValue PruningFrequency {
      get { return PruningFrequencyParameter.ActualValue; }
    }
    public IntValue FirstPruningGeneration {
      get { return FirstPruningGenerationParameter.ActualValue; }
    }
    public IntValue Generation {
      get { return GenerationParameter.ActualValue; }
    }
    #endregion
    public SymbolicRegressionTournamentPruning()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "A random number generator."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to prune."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for branch impact evaluation."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter to use for node impact evaluation"));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first row index of the dataset partition to use for branch impact evaluation."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last row index of the dataset partition to use for branch impact evaluation."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(MaxPruningRatioParameterName, "The maximal relative size of the pruned branch.", new DoubleValue(0.5)));
      Parameters.Add(new ValueLookupParameter<IntValue>(TournamentSizeParameterName, "The number of branches to compare for pruning", new IntValue(10)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(PopulationPercentileStartParameterName, "The start of the population percentile to consider for pruning.", new DoubleValue(0.25)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(PopulationPercentileEndParameterName, "The end of the population percentile to consider for pruning.", new DoubleValue(0.75)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(QualityGainWeightParameterName, "The weight of the quality gain relative to the size gain.", new DoubleValue(1.0)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper estimation limit to use for evaluation."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower estimation limit to use for evaluation."));
      Parameters.Add(new ValueLookupParameter<IntValue>(IterationsParameterName, "The number of pruning iterations to apply for each tree.", new IntValue(1)));
      Parameters.Add(new ValueLookupParameter<IntValue>(FirstPruningGenerationParameterName, "The first generation when pruning should be applied.", new IntValue(1)));
      Parameters.Add(new ValueLookupParameter<IntValue>(PruningFrequencyParameterName, "The frequency of pruning operations (1: every generation, 2: every second generation...)", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>(GenerationParameterName, "The current generation."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The results collection."));
    }

    public override IOperation Apply() {
      bool pruningCondition =
        (Generation.Value >= FirstPruningGeneration.Value) &&
        ((Generation.Value - FirstPruningGeneration.Value) % PruningFrequency.Value == 0);
      if (pruningCondition) {
        int n = SymbolicExpressionTree.Length;
        double percentileStart = PopulationPercentileStart.Value;
        double percentileEnd = PopulationPercentileEnd.Value;
        // for each tree in the given percentile
        var trees = SymbolicExpressionTree
          .Skip((int)(n * percentileStart))
          .Take((int)(n * (percentileEnd - percentileStart)));
        foreach (var tree in trees) {
          Prune(Random, tree, Iterations.Value, TournamentSize.Value,
            DataAnalysisProblemData, SamplesStart.Value, SamplesEnd.Value,
            SymbolicExpressionTreeInterpreter,
            LowerEstimationLimit.Value, UpperEstimationLimit.Value,
            MaxPruningRatio.Value, QualityGainWeight.Value);
        }
      }
      return base.Apply();
    }

    public static void Prune(IRandom random, SymbolicExpressionTree tree, int iterations, int tournamentSize,
      DataAnalysisProblemData problemData, int samplesStart, int samplesEnd,
      ISymbolicExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit, double upperEstimationLimit,
      double maxPruningRatio, double qualityGainWeight) {
      IEnumerable<int> rows = Enumerable.Range(samplesStart, samplesEnd - samplesStart);
      int originalSize = tree.Size;
      double originalMse = SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(interpreter, tree,
        lowerEstimationLimit, upperEstimationLimit, problemData.Dataset, problemData.TargetVariable.Value, samplesStart, samplesEnd);

      int minPrunedSize = (int)(originalSize * (1 - maxPruningRatio));

      // tree for branch evaluation
      SymbolicExpressionTree templateTree = (SymbolicExpressionTree)tree.Clone();
      while (templateTree.Root.SubTrees[0].SubTrees.Count > 0) templateTree.Root.SubTrees[0].RemoveSubTree(0);

      SymbolicExpressionTree prunedTree = tree;
      for (int iteration = 0; iteration < iterations; iteration++) {
        SymbolicExpressionTree iterationBestTree = prunedTree;
        double bestGain = double.PositiveInfinity;
        int maxPrunedBranchSize = (int)(prunedTree.Size * maxPruningRatio);

        for (int i = 0; i < tournamentSize; i++) {
          var clonedTree = (SymbolicExpressionTree)prunedTree.Clone();
          int clonedTreeSize = clonedTree.Size;
          var prunePoints = (from node in clonedTree.IterateNodesPostfix()
                             from subTree in node.SubTrees
                             let subTreeSize = subTree.GetSize()
                             where subTreeSize <= maxPrunedBranchSize
                             where clonedTreeSize - subTreeSize >= minPrunedSize
                             select new { Parent = node, Branch = subTree, SubTreeIndex = node.SubTrees.IndexOf(subTree) })
                 .ToList();
          if (prunePoints.Count > 0) {
            var selectedPrunePoint = prunePoints.SelectRandom(random);
            templateTree.Root.SubTrees[0].AddSubTree(selectedPrunePoint.Branch);
            IEnumerable<double> branchValues = interpreter.GetSymbolicExpressionTreeValues(templateTree, problemData.Dataset, rows);
            double branchMean = branchValues.Average();
            templateTree.Root.SubTrees[0].RemoveSubTree(0);

            selectedPrunePoint.Parent.RemoveSubTree(selectedPrunePoint.SubTreeIndex);
            var constNode = CreateConstant(branchMean);
            selectedPrunePoint.Parent.InsertSubTree(selectedPrunePoint.SubTreeIndex, constNode);

            double prunedMse = SymbolicRegressionScaledMeanSquaredErrorEvaluator.Calculate(interpreter, clonedTree,
        lowerEstimationLimit, upperEstimationLimit, problemData.Dataset, problemData.TargetVariable.Value, samplesStart, samplesEnd);
            double prunedSize = clonedTree.Size;
            // MSE of the pruned tree is larger than the original tree in most cases
            // size of the pruned tree is always smaller than the size of the original tree
            // same change in quality => prefer pruning operation that removes a larger tree
            double gain = ((prunedMse / originalMse) * qualityGainWeight) /
                           (originalSize / prunedSize);
            if (gain < bestGain) {
              bestGain = gain;
              iterationBestTree = clonedTree;
            }
          }
        }
        prunedTree = iterationBestTree;
      }
      tree.Root = prunedTree.Root;
    }

    private static SymbolicExpressionTreeNode CreateConstant(double constantValue) {
      var node = (ConstantTreeNode)(new Constant()).CreateTreeNode();
      node.Value = constantValue;
      return node;
    }
  }
}
