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
using HEAL.Fossil;

namespace HeuristicLab.Problems.GrammaticalEvolution {

  /// <summary>
  /// Position Independent (PI) Grammatical Evolution Mapper
  /// -----------------------------------------------------------------------------------
  /// Standard GE mappers:
  ///   Rule = Codon Value % Number Of Rules
  ///   
  /// 𝜋GE:
  ///   𝜋GE codons consist of (nont, rule) tuples, where nont may be one value from an "order"
  ///   integer vector and rule may be one value from a "content" integer vector.
  ///   
  ///   Order:   NT   = nont % Num. NT      ... determines, which non-terminal to expand next
  ///   Content: Rule = rule % Num. Rules   ... rule determination as with standard GE mappers
  /// 
  /// Four mutation and crossover strategies possible:
  ///  * Order-only:    only "order" vector is modified, "content" vector is fixed (1:0),
  ///                   worst result according to [2]
  ///  * Content-only:  only "content" vector is modified, "order" vector is fixed (0:1),
  ///                   best result according to [2]
  ///  * 𝜋GE:           genetic operators are applied equally (1:1), 
  ///  * Content:Order: genetic operators are applied unequally (e.g. 2:1 or 1:2),
  /// 
  /// Here, the "content-only" strategy is implemented, as it is the best solution according to [2]
  /// and it does not require much to change in the problem and evaluator classes.
  /// 
  /// </summary>
  /// <remarks>
  /// Described in
  /// 
  /// [1] Michael O’Neill et al. 𝜋Grammatical Evolution. In: GECCO 2004.
  /// Vol. 3103. LNCS. Heidelberg: Springer-Verlag Berlin, 2004, pp. 617–629.
  /// url: http://dynamics.org/Altenberg/UH_ICS/EC_REFS/GP_REFS/GECCO/2004/31030617.pdf.
  /// 
  /// [2] David Fagan et al. Investigating Mapping Order in πGE. IEEE, 2010
  /// url: http://ncra.ucd.ie/papers/pigeWCCI2010.pdf
  /// </remarks>
  [Item("PIGEMapper", "Position Independent (PI) Grammatical Evolution Mapper")]
  [StorableType("AFD85902-C2EA-47F5-8284-BA1759848580")]
  public class PIGEMapper : GenotypeToPhenotypeMapper {

    private object nontVectorLocker = new object();
    private IntegerVector nontVector;

    public IntegerVector NontVector {
      get { return nontVector; }
      set { nontVector = value; }
    }

    private static IntegerVector GetNontVector(IRandom random, IntMatrix bounds, int length) {
      IntegerVector v = new IntegerVector(length);
      v.Randomize(random, bounds);
      return v;
    }

    [StorableConstructor]
    protected PIGEMapper(StorableConstructorFlag _) : base(_) { }
    protected PIGEMapper(PIGEMapper original, Cloner cloner) : base(original, cloner) { }
    public PIGEMapper() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PIGEMapper(this, cloner);
    }


    /// <summary>
    /// Maps a genotype (an integer vector) to a phenotype (a symbolic expression tree).
    /// PIGE approach.
    /// </summary>
    /// <param name="random">random number generator</param>
    /// <param name="bounds">integer number range for genomes (codons) of the nont vector</param>
    /// <param name="length">length of the nont vector to create</param>
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

      // Map can be called simultaniously on multiple threads
      lock (nontVectorLocker) {
        if (NontVector == null) {
          NontVector = GetNontVector(random, bounds, length);
        }
      }

      MapPIGEIteratively(startNode, genotype, grammar,
                         genotype.Length, random);

      return tree;
    }


    /// <summary>
    /// Genotype-to-Phenotype mapper (iterative 𝜋GE approach, using a list of not expanded nonTerminals).
    /// </summary>
    /// <param name="startNode">first node of the tree with arity 1</param>
    /// <param name="genotype">integer vector, which should be mapped to a tree</param>
    /// <param name="grammar">grammar to determine the allowed child symbols for each node</param>
    /// <param name="maxSubtreeCount">maximum allowed subtrees (= number of used genomes)</param>
    /// <param name="random">random number generator</param>
    private void MapPIGEIteratively(ISymbolicExpressionTreeNode startNode,
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
          // Order:   NT   = nont % Num. NT
          int nt = NontVector[genotypeIndex] % nonTerminals.Count;
          ISymbolicExpressionTreeNode current = nonTerminals[nt];
          nonTerminals.RemoveAt(nt);

          // Content: Rule = rule % Num. Rules
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