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
  public abstract class StochasticSelectorBase : SelectorBase {
    public StochasticSelectorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Selected", "Number of selected sub-scopes", typeof(IntData), VariableKind.In));
    }

    protected sealed override void Select(IScope source, IScope target, bool copySelected) {
      IRandom random = GetVariableValue<IRandom>("Random", source, true);
      IntData selected = GetVariableValue<IntData>("Selected", source, true);

      Select(random, source, selected.Data, target, copySelected);
    }

    protected abstract void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected);
  }
}
