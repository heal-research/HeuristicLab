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
  /// Represents the type of a variable (input, output,...).
  /// </summary>
  [FlagsAttribute]
  public enum VariableKind {
    /// <summary>
    /// Flag for doing nothing with the variable.
    /// </summary>
    None = 0,
    /// <summary>
    /// Flag for creating a new variable.
    /// </summary>
    New = 1,
    /// <summary>
    /// Flag for changing value of a variable.
    /// </summary>
    Out = 2,
    /// <summary>
    /// Flag for reading value of a variable.
    /// </summary>
    In = 4,
    /// <summary>
    /// Flag for deleting a variable.
    /// </summary>
    Deleted = 8
  }
}
