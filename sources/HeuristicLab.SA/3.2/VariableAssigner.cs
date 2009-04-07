#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;

namespace HeuristicLab.SA {
  public class VariableAssigner : OperatorBase {

    public override string Description {
      get { return @"Assigns a value to a variable (performs a clone of the value during assignment)."; }
    }

    public VariableAssigner()
      : base() {
      AddVariableInfo(new VariableInfo("Variable", "The variable which should be assigned a value to", typeof(IItem), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Value", "The value that is to be assigned", typeof(IItem), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IVariable var = scope.GetVariable(scope.TranslateName("Variable"));
      if (var == null) throw new InvalidOperationException("ERROR in VariableAssigner: Variable not found in current scope");
      IItem val = GetVariableValue<IItem>("Value", scope, true);
      var.Value = (IItem)val.Clone();
      return null;
    }
  }
}
