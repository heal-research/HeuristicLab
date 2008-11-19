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
using System.Xml;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to store meta-information about variables/parameters of operators.
  /// </summary>
  public interface IVariableInfo : IItem {
    /// <summary>
    /// Gets or sets the actual name of the variable info.
    /// </summary>
    string ActualName { get; set; }
    /// <summary>
    /// Gets or sets the formal name of the current instance.
    /// </summary>
    string FormalName { get; }
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Gets the data type of the current instance.
    /// </summary>
    Type DataType { get; }
    /// <summary>
    /// Gets the kind of the variable info (in, out, new,...).
    /// </summary>
    VariableKind Kind { get; }
    /// <summary>
    /// Gets or sets a boolean value whether the current instance is a local variable info.
    /// </summary>
    bool Local { get; set; }

    /// <summary>
    /// Occurs when the actual name of the current instance has been changed.
    /// </summary>
    event EventHandler ActualNameChanged;
    /// <summary>
    /// Occurs when the local flag has been changed.
    /// </summary>
    event EventHandler LocalChanged;
  }
}
