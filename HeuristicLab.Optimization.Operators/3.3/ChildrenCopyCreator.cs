#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Creates a copy of every sub-scope in the current scope and adds it as a child to the sub-scope.
  /// </summary>
  /// <remarks>
  /// Creates a copy of every sub-scope in the current scope and adds it as a child to the sub-scope.
  /// </remarks>
  [Item("ChildrenCopyCreator", "Creates a copy of every sub-scope in the current scope and adds it as a child to the sub-scope.")]
  [StorableClass]
  public sealed class ChildrenCopyCreator : SingleSuccessorOperator {
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    private ChildrenCopyCreator(bool deserializing) : base(deserializing) { }
    private ChildrenCopyCreator(ChildrenCopyCreator original, Cloner cloner) : base(original, cloner) { }
    public ChildrenCopyCreator()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes represent the parents."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ChildrenCopyCreator(this, cloner);
    }

    public override IOperation Apply() {
      int parents = CurrentScope.SubScopes.Count;

      for (int i = 0; i < parents; i++) {
        IScope parent = CurrentScope.SubScopes[i];
        parent.SubScopes.Clear();

        //copy parent
        IScope child = new Scope(i.ToString());
        foreach (IVariable var in parent.Variables)
          child.Variables.Add((IVariable)var.Clone());

        parent.SubScopes.Add(child);
      }
      return base.Apply();
    }
  }
}
