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
 * 
 * Author: Sabine Winkler
 */

#endregion

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  /// <summary>
  /// DepthFirstMapper
  /// </summary>
  [Item("DepthFirstMapper", "Resolves the non-terminal symbols of the resulting phenotypic syntax tree in a depth-first manner.")]
  [StorableClass]
  public class DepthFirstMapper : GenotypeToPhenotypeMapper {

    [StorableConstructor]
    protected DepthFirstMapper(bool deserializing) : base(deserializing) { }
    protected DepthFirstMapper(DepthFirstMapper original, Cloner cloner) : base(original, cloner) { }
    public DepthFirstMapper() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DepthFirstMapper(this, cloner);
    }


    /// <summary>
    /// Maps a genotype (an integer vector) to a phenotype (a symbolic expression tree).
    /// Depth-first approach.
    /// </summary>
    /// <param name="random">random number generator</param>
    /// <param name="bounds">only used for PIGEMapper (ignore here)</param>
    /// <param name="length">only used for PIGEMapper (ignore here)</param>
    /// <param name="grammar">grammar definition</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <returns>phenotype (a symbolic expression tree)</returns>
    public override ISymbolicExpressionTree Map(IRandom random, IntMatrix bounds, int length,
                                               ISymbolicExpressionGrammar grammar,
                                               IntegerVector genotype) {

      SymbolicExpressionTree tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      rootNode.AddSubtree(startNode);
      tree.Root = rootNode;

      MapDepthFirstIteratively(startNode, genotype, grammar,
                               genotype.Length, random);
      return tree;
    }


    /// <summary>
    /// Genotype-to-Phenotype mapper (iterative depth-first approach, by using a stack -> LIFO).
    /// </summary>
    /// <param name="startNode">first node of the tree with arity 1</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <param name="grammar">grammar to determine the allowed child symbols for each node</param>
    /// <param name="maxSubtreeCount">maximum allowed subtrees (= number of used genomes)</param>
    /// <param name="random">random number generator</param>
    private void MapDepthFirstIteratively(ISymbolicExpressionTreeNode startNode,
                                          IntegerVector genotype,
                                          ISymbolicExpressionGrammar grammar,
                                          int maxSubtreeCount, IRandom random) {

      Stack<Tuple<ISymbolicExpressionTreeNode, int>> stack
        = new Stack<Tuple<ISymbolicExpressionTreeNode, int>>(); // tuples of <node, arity>

      int genotypeIndex = 0;
      stack.Push(new Tuple<ISymbolicExpressionTreeNode, int>(startNode, 1));

      while (stack.Count > 0) {

        // get next node from stack and re-push it, if this node still has unhandled subtrees ...
        Tuple<ISymbolicExpressionTreeNode, int> current = stack.Pop();
        if (current.Item2 > 1) {
          stack.Push(new Tuple<ISymbolicExpressionTreeNode, int>(current.Item1, current.Item2 - 1));
        }

        if (genotypeIndex >= maxSubtreeCount) {
          // if all genomes were used, only add terminal nodes to the remaining subtrees
          current.Item1.AddSubtree(GetRandomTerminalNode(current.Item1, grammar, random));
        } else {
          var newNode = GetNewChildNode(current.Item1, genotype, grammar, genotypeIndex, random);
          int arity = SampleArity(random, newNode, grammar);

          current.Item1.AddSubtree(newNode);
          genotypeIndex++;
          if (arity > 0) {
            // new node has subtrees so push it onto the stack 
            stack.Push(new Tuple<ISymbolicExpressionTreeNode, int>(newNode, arity));
          }
        }
      }
    }
  }
}