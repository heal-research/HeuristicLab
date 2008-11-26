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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Base class for all selectors.
  /// </summary>
  public abstract class SelectorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="SelectorBase"/> with one variable infos 
    /// (<c>CopySelected</c>), which is a local one.
    /// </summary>
    public SelectorBase()
      : base() {
      AddVariableInfo(new VariableInfo("CopySelected", "Copy or move selected sub-scopes", typeof(BoolData), VariableKind.In));
      GetVariableInfo("CopySelected").Local = true;
      AddVariable(new Variable("CopySelected", new BoolData(false)));
    }

    /// <summary>
    /// Inserts a new level of sub scopes in the given <paramref name="scope"/> with a scope containing the
    /// remaining sub scopes and another with the selected ones.
    /// </summary>
    /// <remarks>Calls <see cref="Select"/>.</remarks>
    /// <param name="scope">The scope where to select the sub scopes.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      BoolData copySelected = GetVariableValue<BoolData>("CopySelected", scope, true);

      IScope source = new Scope("Remaining");
      while (scope.SubScopes.Count > 0) {
        IScope s = scope.SubScopes[0];
        scope.RemoveSubScope(s);
        source.AddSubScope(s);
      }
      scope.AddSubScope(source);
      IScope target = new Scope("Selected");
      scope.AddSubScope(target);

      Select(source, target, copySelected.Data);

      return null;
    }

    /// <summary>
    /// Selects sub scopes from the specified <paramref name="source"/> and moves or copies it to the
    /// specified <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The source scope where to select the sub scopes.</param>
    /// <param name="target">The target where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether to copy or move the selected sub scopes.</param>
    protected abstract void Select(IScope source, IScope target, bool copySelected);
  }
}
