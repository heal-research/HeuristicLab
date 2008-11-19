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
  /// Event arguments to be able to specify the affected alias. 
  /// </summary>
  public class AliasEventArgs : EventArgs {
    private string myAlias;
    /// <summary>
    /// Gets the affected alias.
    /// </summary>
    public string Alias {
      get { return myAlias; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AliasEventArgs"/> with the given <paramref name="alias"/>.
    /// </summary>
    /// <param name="alias">The affected alias.</param>
    public AliasEventArgs(string alias) {
      myAlias = alias;
    }
  }
}
