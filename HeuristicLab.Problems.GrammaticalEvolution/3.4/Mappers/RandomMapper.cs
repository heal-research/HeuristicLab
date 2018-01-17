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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  /// <summary>
  /// RandomMapper
  /// </summary>
  [Item("RandomMapper", "Randomly determines the next non-terminal symbol to expand.")]
  [StorableClass]
  public class RandomMapper : GenotypeToPhenotypeMapper {

    [StorableConstructor]
    protected RandomMapper(bool deserializing) : base(deserializing) { }
    protected RandomMapper(RandomMapper original, Cloner cloner) : base(original, cloner) { }
    public RandomMapper() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomMapper(this, cloner);
    }


    /// <summary>
    /// Maps a genotype (an integer vector) to a phenotype (a symbolic expression tree).
    /// Random approach.
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

      MapRandomIteratively(startNode, genotype, grammar,
                           genotype.Length, random);

      return tree;
    }


    /// <summary>
    /// Genotype-to-Phenotype mapper (iterative random approach, where the next non-terminal
    /// symbol to expand is randomly determined).
    /// </summary>
    /// <param name="startNode">first node of the tree with arity 1</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <param name="grammar">grammar to determine the allowed child symbols for each node</param>
    /// <param name="maxSubtreeCount">maximum allowed subtrees (= number of used genomes)</param>
    /// <param name="random">random number generator</param>
    private void MapRandomIteratively(ISymbolicExpressionTreeNode startNode,
                                     IntegerVector genotype,
                                     ISymbolicExpressionGrammar grammar,
                                     int maxSubtreeCount, IRandom random) {

      List<ISymbolicExpressionTreeNode> nonTerminals = new List<ISymbolicExpressionTreeNode>();

      int genotypeIndex = 0;
      nonTerminals.Add(startNode);

      while (nonTerminals.Count > 0) {
        if (genotypeIndex >= maxSubtreeCount) {
          // if all genomes were used, only add terminal nodes to the remaining subtrees
          ISymbolicExpressionTreeNode current = nonTerminals[0];
          nonTerminals.RemoveAt(0);
          current.AddSubtree(GetRandomTerminalNode(current, grammar, random));
        } else {
          // similar to PIGEMapper, but here the current node is determined randomly ...
          ISymbolicExpressionTreeNode current = nonTerminals.SampleRandom(random);
          nonTerminals.Remove(current);

          ISymbolicExpressionTreeNode newNode = GetNewChildNode(current, genotype, grammar, genotypeIndex, random);
          int arity = SampleArity(random, newNode, grammar);

          current.AddSubtree(newNode);
          genotypeIndex++;
          // new node has subtrees, so add "arity" number of copies of this node to the nonTerminals list
          for (int i = 0; i < arity; ++i) {
            nonTerminals.Add(newNode);
          }
        }
      }
    }
  }
}