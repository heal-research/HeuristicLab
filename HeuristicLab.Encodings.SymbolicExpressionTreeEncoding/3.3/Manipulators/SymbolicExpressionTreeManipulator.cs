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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators {
  /// <summary>
  /// A base class for operators that manipulate real-valued vectors.
  /// </summary>
  [Item("SymbolicExpressionTreeManipulator", "A base class for operators that manipulate symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeManipulator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeManipulator {
    private const string FailedManipulationEventsParameterName = "FailedManipulationEvents";
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";

    #region Parameter Properties
    public IValueParameter<IntValue> FailedManipulationEventsParameter {
      get { return (IValueParameter<IntValue>)Parameters[FailedManipulationEventsParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    #endregion

    #region Properties
    public IntValue FailedManipulationEvents {
      get { return FailedManipulationEventsParameter.Value; }
    }
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionTreeManipulator(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeManipulator(SymbolicExpressionTreeManipulator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeManipulator()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>(FailedManipulationEventsParameterName, "The number of failed manipulation events.", new IntValue()));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree on which the operator should be applied."));
    }

    public sealed override IOperation Apply() {
      SymbolicExpressionTree tree = SymbolicExpressionTreeParameter.ActualValue;
      bool success;
      Manipulate(RandomParameter.ActualValue, tree, SymbolicExpressionGrammarParameter.ActualValue,
        MaxTreeSizeParameter.ActualValue, MaxTreeHeightParameter.ActualValue, out success);

      if (!success) FailedManipulationEvents.Value++;
      return base.Apply();
    }

    protected abstract void Manipulate(IRandom random, SymbolicExpressionTree symbolicExpressionTree, ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight, out bool success);
  }
}