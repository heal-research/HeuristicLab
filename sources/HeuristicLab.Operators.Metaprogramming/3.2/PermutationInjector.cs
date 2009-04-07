#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Operators.Metaprogramming {
  /// <summary>
  /// Injects a new permutation variable in the given scope. The number of items contained
  /// are in a predifined range.
  /// </summary>
  public class PermutationInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "TASK."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PermutationInjector"/> with five variable infos
    /// (<c>VariableInjector</c>, <c>VariableName</c>, <c>Items</c>, <c>Min</c> and <c>Max</c>).
    /// </summary>
    public PermutationInjector()
      : base() {
      AddVariableInfo(new VariableInfo("VariableInjector", "The combined operator that should hold the generated variable injector", typeof(CombinedOperator), VariableKind.New));
      AddVariableInfo(new VariableInfo("VariableName", "Name of the variable that should be injected", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "Set of items that can be part of the permutation", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("Min", "Minimal number of items to inject", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "Maximal number of items to inject", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Injects a new permutation variable into the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The current scope where to inject the permutation list.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      int min = GetVariableValue<IntData>("Min", scope, true).Data;
      int max = GetVariableValue<IntData>("Max", scope, true).Data;
      string variableName = GetVariableValue<StringData>("VariableName", scope, true).Data;
      ItemList allItems = GetVariableValue<ItemList>("Items", scope, true);
      List<IItem> xs = new List<IItem>(allItems);
      List<List<IItem>> permutations = CalcPermutations(xs, new List<IItem>(), min, max);

      for(int i = 0; i < permutations.Count; i++) {
        string scopeName = "";
        foreach(IItem x in permutations[i]) scopeName += x.ToString() + ";";
        Scope subScope = new Scope(scopeName.TrimEnd(';'));
        CombinedOperator combOp = new CombinedOperator();
        VariableInjector varInjector = new VariableInjector();
        ItemList permutation = new ItemList();
        foreach(IItem item in permutations[i]) permutation.Add(item);
        varInjector.AddVariable(new Variable(variableName, permutation));

        combOp.OperatorGraph.AddOperator(varInjector);
        combOp.OperatorGraph.InitialOperator = varInjector;

        subScope.AddVariable(new Variable(scope.TranslateName("VariableInjector"), combOp));
        scope.AddSubScope(subScope);
      }
      return null;
    }

    private List<List<IItem>> CalcPermutations(List<IItem> allItems, List<IItem> prefix, int min, int max) {
      List<List<IItem>> result = new List<List<IItem>>();
      if(prefix.Count >= max) return result;
      if(prefix.Count >= min) result.Add(new List<IItem>(prefix));
      int count = allItems.Count;
      for(int i = 0; i < count; i++) {
        IItem x = allItems[0];
        allItems.RemoveAt(0);
        prefix.Add(x);
        result.AddRange(CalcPermutations(new List<IItem>(allItems), prefix, min, max));
        prefix.RemoveAt(prefix.Count-1);
      }

      return result;
    }
  }
}
