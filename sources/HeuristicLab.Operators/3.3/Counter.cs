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
  /// Class to increment an int variable by 1.
  /// </summary>
  public class Counter : OperatorBase {
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Counter"/> with one variable info (<c>Value</c>).
    /// </summary>
    public Counter()
      : base() {
      AddVariableInfo(new VariableInfo("Value", "Counter value", typeof(IntData), VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Increments an int value of the specified <paramref name="scope"/> by one.
    /// </summary>
    /// <param name="scope">The scope whose variable should be incremented.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IntData value = GetVariableValue<IntData>("Value", scope, true);
      value.Data++;
      return null;
    }
  }
}
