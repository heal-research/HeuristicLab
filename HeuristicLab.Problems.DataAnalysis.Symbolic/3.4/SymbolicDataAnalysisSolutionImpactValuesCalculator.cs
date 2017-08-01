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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicDataAnalysisSolutionImpactValuesCalculator", "Calculates the impact values and replacements values for symbolic expression tree nodes.")]
  public abstract class SymbolicDataAnalysisSolutionImpactValuesCalculator : Item, ISymbolicDataAnalysisSolutionImpactValuesCalculator {
    protected SymbolicDataAnalysisSolutionImpactValuesCalculator() { }

    protected SymbolicDataAnalysisSolutionImpactValuesCalculator(SymbolicDataAnalysisSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    [StorableConstructor]
    protected SymbolicDataAnalysisSolutionImpactValuesCalculator(bool deserializing) : base(deserializing) { }
    public abstract void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, out double impactValue, out double replacementValue, out double newQualityForImpactsCalculation, double qualityForImpactsCalculation = double.NaN);

    protected IEnumerable<double> CalculateReplacementValues(ISymbolicExpressionTreeNode node, ISymbolicExpressionTree sourceTree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      IDataset dataset, IEnumerable<int> rows) {
      //optimization: constant nodes return always the same value
      ConstantTreeNode constantNode = node as ConstantTreeNode;
      BinaryFactorVariableTreeNode binaryFactorNode = node as BinaryFactorVariableTreeNode;
      FactorVariableTreeNode factorNode = node as FactorVariableTreeNode;
      if (constantNode != null) {
        yield return constantNode.Value;
      } else if (binaryFactorNode != null) {
        // valid replacements are either all off or all on
        yield return 0;
        yield return 1;
      } else if (factorNode != null) {
        foreach (var w in factorNode.Weights) yield return w;
        yield return 0.0;
      } else {
        var rootSymbol = new ProgramRootSymbol().CreateTreeNode();
        var startSymbol = new StartSymbol().CreateTreeNode();
        rootSymbol.AddSubtree(startSymbol);
        startSymbol.AddSubtree((ISymbolicExpressionTreeNode)node.Clone());

        var tempTree = new SymbolicExpressionTree(rootSymbol);
        // clone ADFs of source tree
        for (int i = 1; i < sourceTree.Root.SubtreeCount; i++) {
          tempTree.Root.AddSubtree((ISymbolicExpressionTreeNode)sourceTree.Root.GetSubtree(i).Clone());
        }
        yield return interpreter.GetSymbolicExpressionTreeValues(tempTree, dataset, rows).Median();
        yield return interpreter.GetSymbolicExpressionTreeValues(tempTree, dataset, rows).Average(); // TODO perf
      }
    }
  }
}
