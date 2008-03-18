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

namespace HeuristicLab.Operators {
  public abstract class ComparatorBase : OperatorBase {
    protected ComparatorBase() {
      AddVariableInfo(new VariableInfo("LeftSide", "Variable on the left side of the comparison", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("RightSide", "Variable on the right side of the comparison", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("Result", "Result of the comparison", typeof(BoolData), VariableKind.Out | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      BoolData result = GetVariableValue<BoolData>("Result", scope, false, false);
      if (result == null) {
        result = new BoolData(true);
        IVariableInfo info = GetVariableInfo("Result");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, result));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), result));
      }
      IItem leftSide = GetVariableValue<IItem>("LeftSide", scope, true);
      if (!(leftSide is IComparable)) throw new InvalidOperationException("Comparator: Left side needs to be of type IComparable");
      IComparable left = (IComparable)leftSide;
      IItem rightSide = GetVariableValue<IItem>("RightSide", scope, true);
      result.Data = Compare(left, rightSide);
      return null;
    }

    protected abstract bool Compare(IComparable left, IItem right);
  }
}
