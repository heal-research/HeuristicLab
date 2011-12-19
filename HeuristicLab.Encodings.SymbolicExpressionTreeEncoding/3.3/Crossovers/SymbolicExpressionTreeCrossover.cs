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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers {
  /// <summary>
  /// A base class for operators that perform a crossover of symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCrossover", "A base class for operators that perform a crossover of symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCrossover : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCrossover {
    private const string ParentsParameterName = "Parents";
    private const string ChildParameterName = "Child";
    private const string FailedCrossoverEventsParameterName = "FailedCrossoverEvents";
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[ParentsParameterName]; }
    }
    public ILookupParameter<SymbolicExpressionTree> ChildParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[ChildParameterName]; }
    }
    public IValueParameter<IntValue> FailedCrossoverEventsParameter {
      get { return (ValueParameter<IntValue>)Parameters[FailedCrossoverEventsParameterName]; }
    }

    public IntValue FailedCrossoverEvents {
      get { return FailedCrossoverEventsParameter.Value; }
    }
    [StorableConstructor]
    protected SymbolicExpressionTreeCrossover(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeCrossover(SymbolicExpressionTreeCrossover original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(ParentsParameterName, "The parent symbolic expression trees which should be crossed."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(ChildParameterName, "The child symbolic expression tree resulting from the crossover."));
      Parameters.Add(new ValueParameter<IntValue>(FailedCrossoverEventsParameterName, "The number of failed crossover events (child is an exact copy of a parent)", new IntValue()));
    }

    public sealed override IOperation Apply() {
      if (ParentsParameter.ActualValue.Length != 2)
        throw new ArgumentException("Number of parents must be exactly two for symbolic expression tree crossover operators.");

      SymbolicExpressionTree parent0 = ParentsParameter.ActualValue[0];
      SymbolicExpressionTree parent1 = ParentsParameter.ActualValue[1];

      IRandom random = RandomParameter.ActualValue;

      bool success;
      SymbolicExpressionTree result = Cross(random, parent0, parent1,
        MaxTreeSizeParameter.ActualValue, MaxTreeHeightParameter.ActualValue, out success);

      if (!success) FailedCrossoverEvents.Value++;

      ChildParameter.ActualValue = result;
      return base.Apply();
    }

    protected abstract SymbolicExpressionTree Cross(IRandom random,
      SymbolicExpressionTree parent0, SymbolicExpressionTree parent1,
      IntValue maxTreeSize, IntValue maxTreeHeight, out bool success);
  }
}