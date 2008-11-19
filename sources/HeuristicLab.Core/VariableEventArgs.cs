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

namespace HeuristicLab.Core {
  /// <summary>
  /// Event arguments to be able to specify the affected variable.
  /// </summary>
  public class VariableEventArgs : ItemEventArgs {
    /// <summary>
    /// Gets the affected variable.
    /// </summary>
    /// <remarks>Uses property <see cref="ItemEventArgs.Item"/> of base class <see cref="ItemEventArgs"/>.
    /// No own data storage present.</remarks>
    public IVariable Variable {
      get { return (IVariable)Item; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableEventArgs"/> with the specified 
    /// <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls constructor of base class <see cref="ItemEventArgs"/>.</remarks>
    /// <param name="variable">The affected variable.</param>
    public VariableEventArgs(IVariable variable)
      : base(variable) {
    }
  }
}
