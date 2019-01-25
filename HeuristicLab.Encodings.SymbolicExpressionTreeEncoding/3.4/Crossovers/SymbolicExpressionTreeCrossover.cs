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
 */
#endregion

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of symbolic expression trees.
  /// </summary>
  [Item("SymbolicExpressionTreeCrossover", "A base class for operators that perform a crossover of symbolic expression trees.")]
  [StorableType("AB6004AE-B6ED-41D7-824E-87ECDA5B0AAF")]
  public abstract class SymbolicExpressionTreeCrossover : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCrossover {
    private const string ParentsParameterName = "Parents";
    #region Parameter Properties
    public ILookupParameter<ItemArray<ISymbolicExpressionTree>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[ParentsParameterName]; }
    }
    #endregion
    #region Properties
    private ItemArray<ISymbolicExpressionTree> Parents {
      get { return ParentsParameter.ActualValue; }
    }
    private ISymbolicExpressionTree Child {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
      set { SymbolicExpressionTreeParameter.ActualValue = value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicExpressionTreeCrossover(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeCrossover(SymbolicExpressionTreeCrossover original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(ParentsParameterName, "The parent symbolic expression trees which should be crossed."));
      ParentsParameter.ActualName = "SymbolicExpressionTree";
    }

    public sealed override IOperation InstrumentedApply() {
      if (Parents.Length != 2)
        throw new ArgumentException("Number of parents must be exactly two for symbolic expression tree crossover operators.");

      ISymbolicExpressionTree result = Crossover(RandomParameter.ActualValue, Parents[0], Parents[1]);

      Child = result;
      return base.InstrumentedApply();
    }

    public abstract ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1);


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (Parameters.ContainsKey("Child")) {
        var oldChildParameter = (ILookupParameter<ISymbolicExpressionTree>)Parameters["Child"];
        Parameters.Remove("Child");
        SymbolicExpressionTreeParameter.ActualName = oldChildParameter.ActualName;
      }
      #endregion
    }
  }
}