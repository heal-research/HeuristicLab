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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.SimOpt {
  public abstract class SimOptManipulationOperatorBase : OperatorBase {

    public SimOptManipulationOperatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "A (uniform distributed) pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "The parameter vector to manipulate", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Index", "Which index in the parameter vector to manipulate", typeof(IntData), VariableKind.In));
      GetVariableInfo("Index").Local = true;
      AddVariable(new Variable("Index", new IntData(-1)));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      ConstrainedItemList parameterVector = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      IntData index = GetVariableValue<IntData>("Index", scope, false);
      int i = index.Data;
      if (i < 0 || i >= parameterVector.Count) throw new IndexOutOfRangeException("ERROR: Index is out of range of the parameter vector");
      IItem item = parameterVector[i];
      if (item is Variable) {
        item = ((Variable)item).Value;
      }
      Apply(scope, random, item);
      return null;
    }

    protected abstract void Apply(IScope scope, IRandom random, IItem item);

    #region helper functions
    protected bool IsIntegerConstrained(ConstrainedDoubleData data) {
      foreach (IConstraint constraint in data.Constraints) {
        if (constraint is IsIntegerConstraint) {
          return true;
        }
      }
      return false;
    }
    #endregion
  }
}
