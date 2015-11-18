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

using System;
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
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope whose sub-scopes should be copied."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ChildrenCopyCreator(this, cloner);
    }

    public override IOperation Apply() {
      int nChildren = CurrentScope.SubScopes.Count;

      for (int i = 0; i < nChildren; i++) {
        IScope child = CurrentScope.SubScopes[i];
        if (child.SubScopes.Count > 0) throw new ArgumentException("The sub-scope that should be cloned has further sub-scopes.");

        IScope childCopy = new Scope(i.ToString());
        var cloner = new Cloner();
        foreach (IVariable var in child.Variables)
          childCopy.Variables.Add(cloner.Clone(var));

        child.SubScopes.Add(childCopy);
      }
      return base.Apply();
    }
  }
}
