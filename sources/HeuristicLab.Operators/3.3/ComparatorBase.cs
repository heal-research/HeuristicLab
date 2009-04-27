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
  /// <summary>
  /// Base class for operators which perform comparisons between two items.
  /// </summary>
  public abstract class ComparatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="ComparatorBase"/> with three 
    /// variable infos (<c>LeftSide</c>, <c>RightSide</c> and <c>Result</c>).
    /// </summary>
    protected ComparatorBase() {
      AddVariableInfo(new VariableInfo("LeftSide", "Variable on the left side of the comparison", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("RightSide", "Variable on the right side of the comparison", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("Result", "Result of the comparison", typeof(BoolData), VariableKind.Out | VariableKind.New));
    }

    /// <summary>
    /// Compares two items with each other and injects the <c>Result</c> variable - if it is no local one - into
    /// the specified <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Compare"/>.</remarks>
    /// <exception cref="InvalidOperationException">Thrown when left side of the comparison does not
    /// implement <see cref="IComparable"/>.</exception>
    /// <param name="scope">The scope where to apply the compare operation.</param>
    /// <returns><c>null</c>.</returns>
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

    /// <summary>
    /// Compares two variables with each other.
    /// </summary>
    /// <param name="left">The variable on the left side of the comparison.</param>
    /// <param name="right">The variable on the right side of the comparison.</param>
    /// <returns><c>true</c> if the comparison query was successful, <c>false</c> otherwise.</returns>
    protected abstract bool Compare(IComparable left, IItem right);
  }
}
