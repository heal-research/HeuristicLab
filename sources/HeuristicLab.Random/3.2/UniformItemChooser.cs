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
using HeuristicLab.Constraints;

namespace HeuristicLab.Random {
  /// <summary>
  /// UniformItemChooser takes a list of items selects one of the elements randomly and sets the selected element as value of output variable.
  /// </summary>
  public class UniformItemChooser : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Initializes the value of variable 'Value' to a random value chosen uniformly from the a list of values"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformItemChooser"/> with four variable infos 
    /// (<c>Value</c>, <c>Random</c>, <c>Max</c> and <c>Min</c>), being a random number generator 
    /// between 0.0 and 1.0.
    /// </summary>
    public UniformItemChooser() {
      AddVariableInfo(new VariableInfo("Value", "The value to manipulate", typeof(IItem), VariableKind.Out));
      AddVariableInfo(new VariableInfo("Random", "The random generator to use", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("Values", "The list of possible values", typeof(ItemList), VariableKind.In));
    }

    /// <summary>
    /// Selects a random element from 'values' and sets the selected element as value of variable 'Value'
    /// </summary>
    /// <param name="scope">The scope where to apply the random number generator.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IVariable value = scope.GetVariable(scope.TranslateName("Value"));
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      value.Value = values[mt.Next(values.Count)];
      return null;
    }
  }
}
