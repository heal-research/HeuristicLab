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
using System.ComponentModel;

namespace HeuristicLab.Core {
  /// <summary>
  /// Event arguments to be able to specify the affected name.
  /// </summary>
  public class NameChangingEventArgs : CancelEventArgs {
    private string myName;
    /// <summary>
    /// Gets the affected name.
    /// </summary>
    public string Name {
      get { return myName; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NameChangingEventArgs"/> 
    /// with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The affected name.</param>
    public NameChangingEventArgs(string name)
      : base() {
      myName = name;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="NameChangingEventArgs"/> 
    /// with the specified <paramref name="name"/> and a flag whether the event has been canceled.
    /// </summary>
    /// <remarks>Calls constructor of base class <see cref="CancelEventArgs"/>.</remarks> 
    /// <param name="name">The affected name.</param>
    /// <param name="cancel">Flag, whether the event has been canceled.</param>
    public NameChangingEventArgs(string name, bool cancel)
      : base(cancel) {
      myName = name;
    }
  }
}
