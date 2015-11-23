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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("SemanticSimilarityCrossover", "An operator which performs subtree swapping based on the notion semantic similarity between subtrees\n" +
                                       "(criteria: mean of the absolute differences between the estimated output values of the two subtrees, falling into a user-defined range)\n" +
                                       "- Take two parent individuals P0 and P1\n" +
                                       "- Randomly choose a node N from the P0\n" +
                                       "- Find the first node M that satisfies the semantic similarity criteria\n" +
                                       "- Swap N for M and return P0")]
  public sealed class SymbolicDataAnalysisExpressionSemanticSimilarityCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {
    private const string SemanticSimilarityRangeParameterName = "SemanticSimilarityRange";

    #region Parameter properties
    public IValueParameter<DoubleRange> SemanticSimilarityRangeParameter {
      get { return (IValueParameter<DoubleRange>)Parameters[SemanticSimilarityRangeParameterName]; }
    }
    #endregion

    #region Properties
    public DoubleRange SemanticSimilarityRange {
      get { return SemanticSimilarityRangeParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionSemanticSimilarityCrossover(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisExpressionSemanticSimilarityCrossover(SymbolicDataAnalysisExpressionCrossover<T> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisExpressionSemanticSimilarityCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleRange>(SemanticSimilarityRangeParameterName, "Semantic similarity interval.", new DoubleRange(0.0001, 10)));
      name = "SemanticSimilarityCrossover";
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionSemanticSimilarityCrossover<T>(this, cloner);
    }

    public override ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
      List<int> rows = GenerateRowsToEvaluate().ToList();
      T problemData = ProblemDataParameter.ActualValue;
      return Cross(random, parent0, parent1, interpreter, problemData, rows, MaximumSymbolicExpressionTreeDepth.Value, MaximumSymbolicExpressionTreeLength.Value, SemanticSimilarityRange);
    }

    /// <summary>
    /// Takes two parent individuals P0 and P1. 
    /// Randomly choose a node i from the first parent, then get a node j from the second parent that matches the semantic similarity criteria.
    /// </summary>
    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
                                                T problemData, List<int> rows, int maxDepth, int maxLength, DoubleRange range) {
      var crossoverPoints0 = new List<CutPoint>();
      parent0.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent0.Root)
          crossoverPoints0.Add(new CutPoint(n.Parent, n));
      });

      var crossoverPoint0 = crossoverPoints0.SampleRandom(random);
      int level = parent0.Root.GetBranchLevel(crossoverPoint0.Child);
      int length = parent0.Root.GetLength() - crossoverPoint0.Child.GetLength();

      var allowedBranches = new List<ISymbolicExpressionTreeNode>();
      parent1.Root.ForEachNodePostfix((n) => {
        if (n.Parent != null && n.Parent != parent1.Root) {
          if (n.GetDepth() + level <= maxDepth && n.GetLength() + length <= maxLength && crossoverPoint0.IsMatchingPointType(n))
            allowedBranches.Add(n);
        }
      });

      if (allowedBranches.Count == 0)
        return parent0;

      var dataset = problemData.Dataset;

      // create symbols in order to improvize an ad-hoc tree so that the child can be evaluated
      var rootSymbol = new ProgramRootSymbol();
      var startSymbol = new StartSymbol();
      var tree0 = CreateTreeFromNode(random, crossoverPoint0.Child, rootSymbol, startSymbol);
      List<double> estimatedValues0 = interpreter.GetSymbolicExpressionTreeValues(tree0, dataset, rows).ToList();
      crossoverPoint0.Child.Parent = crossoverPoint0.Parent; // restore parent
      ISymbolicExpressionTreeNode selectedBranch = null;

      // pick the first node that fulfills the semantic similarity conditions
      foreach (var node in allowedBranches) {
        var parent = node.Parent;
        var tree1 = CreateTreeFromNode(random, node, startSymbol, rootSymbol); // this will affect node.Parent 
        List<double> estimatedValues1 = interpreter.GetSymbolicExpressionTreeValues(tree1, dataset, rows).ToList();
        node.Parent = parent; // restore parent

        OnlineCalculatorError errorState;
        double ssd = OnlineMeanAbsoluteErrorCalculator.Calculate(estimatedValues0, estimatedValues1, out errorState);

        if (range.Start <= ssd && ssd <= range.End) {
          selectedBranch = node;
          break;
        }
      }

      // perform the actual swap
      if (selectedBranch != null)
        Swap(crossoverPoint0, selectedBranch);
      return parent0;
    }
  }
}
