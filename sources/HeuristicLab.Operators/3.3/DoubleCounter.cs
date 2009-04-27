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
  /// Class to add a specified interval to a double value.
  /// </summary>
  public class DoubleCounter : OperatorBase {
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    public override string Description {
      get { return @"Adds a given interval to a double value"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleCounter"/>, including two variable infos 
    /// (<c>Value</c> and <c>Interval</c>).
    /// </summary>
    public DoubleCounter()
      : base() {
      AddVariableInfo(new VariableInfo("Value", "Counter value", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Interval", "Interval value", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Adds to a double value of the given <paramref name="scope"/> a specified interval.
    /// </summary>
    /// <param name="scope">The scope whose variable should be incremented.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      DoubleData value = GetVariableValue<DoubleData>("Value", scope, true);
      double interval = GetVariableValue<DoubleData>("Interval", scope, true).Data;
      value.Data += interval;
      return null;
    }
  }
}
