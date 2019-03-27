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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("B53649DF-A760-4087-A496-BFD3F13DA31C")]
  public abstract class SymbolicDataAnalysisExpressionCrossover<T> : SymbolicExpressionTreeCrossover, ISymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string ProblemDataParameterName = "ProblemData";
    private const string EvaluatorParameterName = "Evaluator";
    private const string EvaluationPartitionParameterName = "EvaluationPartition";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";

    #region Parameter properties
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<T> ProblemDataParameter {
      get { return (IValueLookupParameter<T>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>> EvaluatorParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>)Parameters[EvaluatorParameterName]; }
    }
    public IValueLookupParameter<IntRange> EvaluationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[EvaluationPartitionParameterName]; }
    }
    public IValueLookupParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    #endregion

    #region Properties
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionCrossover(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisExpressionCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisExpressionCrossover()
      : base() {
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The interpreter that should be used to calculate the output values of the symbolic data analysis tree."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisSingleObjectiveEvaluator<T>>(EvaluatorParameterName, "The single objective solution evaluator"));
      Parameters.Add(new ValueLookupParameter<T>(ProblemDataParameterName, "The problem data on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<IntRange>(EvaluationPartitionParameterName, "The start index of the dataset partition on which the symbolic data analysis solution should be evaluated."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation between the start and end index."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximum tree depth."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximum tree length."));

      EvaluatorParameter.Hidden = true;
      EvaluationPartitionParameter.Hidden = true;
      SymbolicDataAnalysisTreeInterpreterParameter.Hidden = true;
      ProblemDataParameter.Hidden = true;
      RelativeNumberOfEvaluatedSamplesParameter.Hidden = true;
    }

    /// <summary>
    /// Creates a SymbolicExpressionTreeNode reusing the root and start symbols (since they are expensive to create).
    /// </summary>
    /// <param name="random"></param>
    /// <param name="node"></param>
    /// <param name="rootSymbol"></param>
    /// <param name="startSymbol"></param>
    /// <returns></returns>
    protected static ISymbolicExpressionTree CreateTreeFromNode(IRandom random, ISymbolicExpressionTreeNode node, ISymbol rootSymbol, ISymbol startSymbol) {
      var rootNode = new SymbolicExpressionTreeTopLevelNode(rootSymbol);
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);

      var startNode = new SymbolicExpressionTreeTopLevelNode(startSymbol);
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);

      startNode.AddSubtree(node);
      rootNode.AddSubtree(startNode);

      return new SymbolicExpressionTree(rootNode);
    }

    protected IEnumerable<int> GenerateRowsToEvaluate() {
      return GenerateRowsToEvaluate(RelativeNumberOfEvaluatedSamplesParameter.ActualValue.Value);
    }

    protected IEnumerable<int> GenerateRowsToEvaluate(double percentageOfRows) {
      IEnumerable<int> rows;
      int samplesStart = EvaluationPartitionParameter.ActualValue.Start;
      int samplesEnd = EvaluationPartitionParameter.ActualValue.End;
      int testPartitionStart = ProblemDataParameter.ActualValue.TestPartition.Start;
      int testPartitionEnd = ProblemDataParameter.ActualValue.TestPartition.End;

      if (samplesEnd < samplesStart) throw new ArgumentException("Start value is larger than end value.");

      if (percentageOfRows.IsAlmost(1.0))
        rows = Enumerable.Range(samplesStart, samplesEnd - samplesStart);
      else {
        int seed = RandomParameter.ActualValue.Next();
        int count = (int)((samplesEnd - samplesStart) * percentageOfRows);
        if (count == 0) count = 1;
        rows = RandomEnumerable.SampleRandomNumbers(seed, samplesStart, samplesEnd, count);
      }

      return rows.Where(i => i < testPartitionStart || testPartitionEnd <= i);
    }

    protected static void Swap(CutPoint crossoverPoint, ISymbolicExpressionTreeNode selectedBranch) {
      if (crossoverPoint.Child != null) {
        // manipulate the tree of parent0 in place
        // replace the branch in tree0 with the selected branch from tree1
        crossoverPoint.Parent.RemoveSubtree(crossoverPoint.ChildIndex);
        if (selectedBranch != null) {
          crossoverPoint.Parent.InsertSubtree(crossoverPoint.ChildIndex, selectedBranch);
        }
      } else {
        // child is null (additional child should be added under the parent)
        if (selectedBranch != null) {
          crossoverPoint.Parent.AddSubtree(selectedBranch);
        }
      }
    }
  }
}
