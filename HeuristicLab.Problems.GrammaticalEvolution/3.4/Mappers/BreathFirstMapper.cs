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
  /// BreathFirstMapper
  /// </summary>
  [Item("BreathFirstMapper", "Resolves the non-terminal symbols of the resulting phenotypic syntax tree in a breath-first manner.")]
  [StorableClass]
  public class BreathFirstMapper : GenotypeToPhenotypeMapper {

    [StorableConstructor]
    protected BreathFirstMapper(bool deserializing) : base(deserializing) { }
    protected BreathFirstMapper(BreathFirstMapper original, Cloner cloner) : base(original, cloner) { }
    public BreathFirstMapper() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BreathFirstMapper(this, cloner);
    }


    /// <summary>
    /// Maps a genotype (an integer vector) to a phenotype (a symbolic expression tree).
    /// Breath-first approach.
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
      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      rootNode.AddSubtree(startNode);
      tree.Root = rootNode;

      MapBreathFirstIteratively(startNode, genotype, grammar,
                                genotype.Length, random);

      return tree;
    }

    /// <summary>
    /// Genotype-to-Phenotype mapper (iterative breath-first approach, by using a queue -> FIFO).
    /// </summary>
    /// <param name="startNode">first node of the tree with arity 1</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <param name="grammar">grammar to determine the allowed child symbols for each node</param>
    /// <param name="maxSubtreeCount">maximum allowed subtrees (= number of used genomes)</param>
    /// <param name="random">random number generator</param>
    private void MapBreathFirstIteratively(ISymbolicExpressionTreeNode startNode,
                                          IntegerVector genotype,
                                          ISymbolicExpressionGrammar grammar,
                                          int maxSubtreeCount, IRandom random) {

      Queue<Tuple<ISymbolicExpressionTreeNode, int>> queue
        = new Queue<Tuple<ISymbolicExpressionTreeNode, int>>(); // tuples of <node, arity>

      int genotypeIndex = 0;
      queue.Enqueue(new Tuple<ISymbolicExpressionTreeNode, int>(startNode, 1));

      while (queue.Count > 0) {

        Tuple<ISymbolicExpressionTreeNode, int> current = queue.Dequeue();

        // foreach subtree of the current node, create a new node and enqueue it, if it is no terminal node
        for (int i = 0; i < current.Item2; ++i) {

          if (genotypeIndex >= maxSubtreeCount) {
            // if all genomes were used, only add terminal nodes to the remaining subtrees
            current.Item1.AddSubtree(GetRandomTerminalNode(current.Item1, grammar, random));
          } else {
            var newNode = GetNewChildNode(current.Item1, genotype, grammar, genotypeIndex, random);
            int arity = SampleArity(random, newNode, grammar);

            current.Item1.AddSubtree(newNode);
            genotypeIndex++;
            if (arity > 0) {
              // new node has subtrees so enqueue the node 
              queue.Enqueue(new Tuple<ISymbolicExpressionTreeNode, int>(newNode, arity));
            }
          }
        }
      }
    }
  }
}