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
 * 
 * Author: Sabine Winkler
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  /// <summary>
  /// Abstract base class for GenotypeToPhenotypeMappers
  /// </summary>
  [StorableType("427C4EB7-7888-4AB2-824A-E1F2EB1DE2FA")]
  public abstract class GenotypeToPhenotypeMapper : IntegerVectorOperator, IGenotypeToPhenotypeMapper {

    [StorableConstructor]
    protected GenotypeToPhenotypeMapper(StorableConstructorFlag _) : base(_) { }
    protected GenotypeToPhenotypeMapper(GenotypeToPhenotypeMapper original, Cloner cloner) : base(original, cloner) { }
    protected GenotypeToPhenotypeMapper() : base() { }

    public abstract ISymbolicExpressionTree Map(IRandom random, IntMatrix bounds, int length,
                                               ISymbolicExpressionGrammar grammar,
                                               IntegerVector genotype);

    /// <summary>
    /// Randomly returns a terminal node for the given <paramref name="parentNode"/>.
    /// (A terminal has got a minimum and maximum arity of 0.)
    /// </summary>
    /// <param name="parentNode">parent node for which a child node is returned randomly</param>
    /// <param name="grammar">grammar to determine the allowed child symbols for parentNode</param>
    /// <param name="random">random number generator</param>
    /// <returns>randomly chosen terminal node with arity 0 or null, if no terminal node exists</returns>
    protected ISymbolicExpressionTreeNode GetRandomTerminalNode(ISymbolicExpressionTreeNode parentNode,
                                                                ISymbolicExpressionGrammar grammar,
                                                                IRandom random) {
      // only select specific symbols, which can be interpreted ...
      var possibleSymbolsList = (from s in grammar.GetAllowedChildSymbols(parentNode.Symbol)
                                 where s.InitialFrequency > 0.0
                                 where s.MaximumArity == 0
                                 where s.MinimumArity == 0
                                 select s).ToList();

      // no terminal node exists for the given parent node
      if (!possibleSymbolsList.Any()) return null;

      var newNode = possibleSymbolsList.SampleRandom(random).CreateTreeNode();
      if (newNode.HasLocalParameters) newNode.ResetLocalParameters(random);
      return newNode;
    }


    /// <summary>
    /// Returns a randomly chosen child node for the given <paramref name="parentNode"/>.
    /// </summary>
    /// <param name="parentNode">parent node to find a child node randomly for</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <param name="grammar">grammar used to define the allowed child symbols</param>
    /// <param name="genotypeIndex">index in the integer vector; can be greater than vector length</param>
    /// <param name="random">random number generator</param>
    /// <returns>randomly chosen child node or null, if no child node exits</returns>
    protected ISymbolicExpressionTreeNode GetNewChildNode(ISymbolicExpressionTreeNode parentNode,
                                                          IntegerVector genotype,
                                                          ISymbolicExpressionGrammar grammar,
                                                          int genotypeIndex,
                                                          IRandom random) {

      // only select specific symbols, which can be interpreted ...
      IEnumerable<ISymbol> symbolList = (from s in grammar.GetAllowedChildSymbols(parentNode.Symbol)
                                         where s.InitialFrequency > 0.0
                                         select s).ToList();

      int prodRuleCount = symbolList.Count();

      // no child node exists for the given parent node
      if (prodRuleCount < 1) return null;

      // genotypeIndex % genotype.Length, if wrapping is allowed
      int prodRuleIndex = genotype[genotypeIndex] % prodRuleCount;

      var newNode = symbolList.ElementAt(prodRuleIndex).CreateTreeNode();
      if (newNode.HasLocalParameters) newNode.ResetLocalParameters(random);
      return newNode;
    }


    /// <summary>
    /// Randomly determines an arity for the given node.
    /// </summary>
    /// <param name="random">random number generator</param>
    /// <param name="node">node, for which a random arity is determined</param>
    /// <param name="grammar">symbolic expression grammar to use</param>
    /// <returns>random arity in the interval [minArity, maxArity]</returns>
    protected int SampleArity(IRandom random,
                              ISymbolicExpressionTreeNode node,
                              ISymbolicExpressionGrammar grammar) {

      int minArity = grammar.GetMinimumSubtreeCount(node.Symbol);
      int maxArity = grammar.GetMaximumSubtreeCount(node.Symbol);

      if (minArity == maxArity) {
        return minArity;
      }

      return random.Next(minArity, maxArity);
    }
  }
}
