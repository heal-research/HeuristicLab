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
  /// Event arguments to be able to specify the affected scope at the specified index.
  /// </summary>
  public class ScopeIndexEventArgs : ItemIndexEventArgs {
    /// <summary>
    /// Gets the affected scope.
    /// </summary>
    /// <remarks>Uses property <see cref="ItemEventArgs.Item"/> of base class 
    /// <see cref="ItemIndexEventArgs"/>. No own data storage present.</remarks>
    public IScope Scope {
      get { return (IScope)Item; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ScopeIndexEventArgs"/> with the specified 
    /// <paramref name="scope"/> and <paramref name="index"/>.
    /// </summary>
    /// <remarks>Calls constructor of base class <see cref="ItemIndexEventArgs"/>.</remarks>
    /// <param name="scope">The affected scope.</param>
    /// <param name="index">The affected index.</param>
    public ScopeIndexEventArgs(IScope scope, int index)
      : base(scope, index) {
    }
  }
}
