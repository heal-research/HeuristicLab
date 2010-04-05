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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators creating symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCreator", "A base class for operators creating symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCreator : SingleSuccessorOperator, ISolutionCreator, IStochasticOperator, ISymbolicExpressionTreeOperator {
    private const string RandomParameterName = "Random";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string MaxTreeSizeParameterName = "MaxTreeSize";
    private const string MaxTreeHeightParameterName = "MaxTreeHeight";
    private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionGrammar";

    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxTreeSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxTreeSizeParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaxTreeHeightParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaxTreeHeightParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionGrammarParameter {
      get { return (ILookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionGrammarParameterName]; }
    }

    protected SymbolicExpressionTreeCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The pseudo random number generator which should be used for stochastic solution creation operators."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree that should be initialized."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeSizeParameterName, "The maximal size (number of nodes) of the symbolic expression tree that should be initialized."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeHeightParameterName, "The maximal height of the symbolic expression tree that should be initialized (a tree with one node has height = 0)."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The grammar that defines the allowed symbols and syntax of the symbolic expression trees."));
    }

    public sealed override IOperation Apply() {
      SymbolicExpressionTreeParameter.ActualValue = Create(RandomParameter.ActualValue, SymbolicExpressionGrammarParameter.ActualValue,
        MaxTreeSizeParameter.ActualValue, MaxTreeHeightParameter.ActualValue);

      foreach (var node in SymbolicExpressionTreeParameter.ActualValue.IterateNodesPostfix()) {
        node.ResetLocalParameters(RandomParameter.ActualValue);
      }
      return null;
    }

    protected abstract SymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, IntValue maxTreeSize, IntValue maxTreeHeight);
  }
}
