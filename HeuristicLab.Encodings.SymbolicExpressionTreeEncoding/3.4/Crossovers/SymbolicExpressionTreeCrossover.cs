#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCrossover", "A base class for operators that perform a crossover of symbolic expression trees.")]
  [StorableClass]
  public abstract class SymbolicExpressionTreeCrossover : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCrossover {
    private const string ParentsParameterName = "Parents";
    private const string ChildParameterName = "Child";
    #region Parameter Properties
    public ILookupParameter<ItemArray<ISymbolicExpressionTree>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[ParentsParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> ChildParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[ChildParameterName]; }
    }
    #endregion
    #region Properties
    public ItemArray<ISymbolicExpressionTree> Parents {
      get { return ParentsParameter.ActualValue; }
    }
    public ISymbolicExpressionTree Child {
      get { return ChildParameter.ActualValue; }
      set { ChildParameter.ActualValue = value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicExpressionTreeCrossover(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeCrossover(SymbolicExpressionTreeCrossover original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(ParentsParameterName, "The parent symbolic expression trees which should be crossed."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(ChildParameterName, "The child symbolic expression tree resulting from the crossover."));
      ParentsParameter.ActualName = "SymbolicExpressionTree";
      ChildParameter.ActualName = "SymbolicExpressionTree";
    }

    public sealed override IOperation InstrumentedApply() {
      if (Parents.Length != 2)
        throw new ArgumentException("Number of parents must be exactly two for symbolic expression tree crossover operators.");

      ISymbolicExpressionTree result = Crossover(Random, Parents[0], Parents[1]);

      Child = result;
      return base.InstrumentedApply();
    }

    public abstract ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1);
  }
}