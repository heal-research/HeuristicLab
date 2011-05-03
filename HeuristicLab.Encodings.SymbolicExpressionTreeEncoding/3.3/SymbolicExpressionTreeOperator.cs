#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators for symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeOperator", "A base class for operators for symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeOperator : SingleSuccessorOperator, IStochasticOperator, ISymbolicExpressionTreeOperator {
    private const string RandomParameterName = "Random";
    private const string MaxTreeSizeParameterName = "MaxTreeSize";
    private const string MaxTreeHeightParameterName = "MaxTreeHeight";
    private const string SymbolicExpressionGrammarParameterName = "SymbolicExpressionGrammar";

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameter Properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters[RandomParameterName]; }
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
    #endregion

    #region Properties
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public IntValue MaxTreeSize {
      get { return MaxTreeSizeParameter.ActualValue; }
    }
    public IntValue MaxTreeHeight {
      get { return MaxTreeHeightParameter.ActualValue; }
    }
    public ISymbolicExpressionGrammar SymbolicExpressionGrammar {
      get { return SymbolicExpressionGrammarParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionTreeOperator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeOperator(SymbolicExpressionTreeOperator original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeOperator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The pseudo random number generator which should be used for symbolic expression tree operators."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeSizeParameterName, "The maximal size (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaxTreeHeightParameterName, "The maximal height of the symbolic expression tree (a tree with one node has height = 0)."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionGrammarParameterName, "The grammar that defines the allowed symbols and syntax of the symbolic expression trees."));
    }
  }
}
