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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent a variable (of an operator) having a name and a value.
  /// </summary>
  public interface IVariable : IItem {
    /// <summary>
    /// Gets or sets the name of the variable.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Gets or sets the value of the variable.
    /// </summary>
    IItem Value { get; set; }

    /// <summary>
    /// Gets the value of the variable.
    /// </summary>
    /// <typeparam name="T">The type of the variable's value.</typeparam>
    /// <returns>The value of the current instance.</returns>
    T GetValue<T>() where T : class, IItem;

    /// <summary>
    /// Occurs when the name of the variable is currently changing.
    /// </summary>
    event EventHandler<CancelEventArgs<string>> NameChanging;
    /// <summary>
    /// Occurs when the name of the current instance has been changed.
    /// </summary>
    event EventHandler NameChanged;
    /// <summary>
    /// Occurs when the value of the current instance has been changed.
    /// </summary>
    event EventHandler ValueChanged;
  }
}
