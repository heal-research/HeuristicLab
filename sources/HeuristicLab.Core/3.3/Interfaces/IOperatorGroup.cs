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
  /// Interface to represent a group of operators.
  /// </summary>
  public interface IOperatorGroup : IItem {
    /// <summary>
    /// Gets or sets the name of the current instance.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Gets all sub groups of operators of the current instance.
    /// </summary>
    ICollection<IOperatorGroup> SubGroups { get; }
    /// <summary>
    /// Gets all operators of the current operator group.
    /// </summary>
    ICollection<IOperator> Operators { get; }

    /// <summary>
    /// Adds the specified sub group of operators to the current operator group.
    /// </summary>
    /// <param name="group">The operator group to add.</param>
    void AddSubGroup(IOperatorGroup group);
    /// <summary>
    /// Deletes the specified sub group of operators from the current instance.
    /// </summary>
    /// <param name="group">The sub group to delete.</param>
    void RemoveSubGroup(IOperatorGroup group);
    /// <summary>
    /// Adds the specified operator to the current instance.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    void AddOperator(IOperator op);
    /// <summary>
    /// Deletes the specified operator from the current instance.
    /// </summary>
    /// <param name="op">The operator to remove.</param>
    void RemoveOperator(IOperator op);

    /// <summary>
    /// Occurs when the name of the operator group has been changed.
    /// </summary>
    event EventHandler NameChanged;
  }
}
