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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCrossover", "A base class for operators that perform a crossover of symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCrossover : SingleSuccessorOperator, ICrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> ParentsParameter {
      get { return (SubScopesLookupParameter<SymbolicExpressionTree>)Parameters["Parents"]; }
    }
    public ILookupParameter<SymbolicExpressionTree> ChildParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["Child"]; }
    }
    public IValueLookupParameter<IntValue> MaxTreeSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxTreeSize"]; }
    }
    public IValueLookupParameter<IntValue> MaxTreeHeightParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxTreeHeight"]; }
    }
    public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters["SymbolicExpressionGrammar"]; }
    }

    protected SymbolicExpressionTreeCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>("Parents", "The parent symbolic expression trees which should be crossed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxTreeSize", "The maximal size (number of nodes) of the symbolic expression tree that should be initialized."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxTreeHeight", "The maximal height of the symbolic expression tree that should be initialized (a tree with one node has height = 0)."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>("SymbolicExpressionGrammar", "The grammar that defines the allowed symbols and syntax of the symbolic expression trees."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("Child", "The child symbolic expression tree resulting from the crossover."));
    }

    public sealed override IOperation Apply() {
      if (ParentsParameter.ActualValue.Length != 2)
        throw new ArgumentException("Number of parents must be exactly two for symbolic expression tree crossover operators.");

      SymbolicExpressionTree parent0 = ParentsParameter.ActualValue[0];
      SymbolicExpressionTree parent1 = ParentsParameter.ActualValue[1];

      IRandom random = RandomParameter.ActualValue;
      ISymbolicExpressionGrammar grammar = SymbolicExpressionGrammarParameter.ActualValue;

      // randomly swap parents to remove a possible bias from selection (e.g. when using gender-specific selection)
      if (random.NextDouble() < 0.5) {
        var tmp = parent0;
        parent0 = parent1;
        parent1 = tmp;
      }

      SymbolicExpressionTree result = Cross(random, grammar, parent0, parent1,
        MaxTreeSizeParameter.ActualValue, MaxTreeHeightParameter.ActualValue);
      Debug.Assert(result.Size <= MaxTreeSizeParameter.ActualValue.Value);
      Debug.Assert(result.Height <= MaxTreeHeightParameter.ActualValue.Value);
      Debug.Assert(grammar.IsValidExpression(result));
      ChildParameter.ActualValue = result;
      return base.Apply();
    }

    protected abstract SymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionGrammar grammar,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      IntValue maxTreeSize, IntValue maxTreeHeight);
  }
}
