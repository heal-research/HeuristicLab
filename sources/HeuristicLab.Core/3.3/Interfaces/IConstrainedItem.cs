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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent items that are subjects to restrictions. 
  /// </summary>
  public interface IConstrainedItem : IItem {
    /// <summary>
    /// All constraints of the current instance. 
    /// </summary>
    ICollection<IConstraint> Constraints { get; }

    /// <summary>
    /// Adds a constraint to the current instance.
    /// </summary>
    /// <param name="constraint">The constraint to add.</param>
    void AddConstraint(IConstraint constraint);
    /// <summary>
    /// Removes a constraint from the current instance.
    /// </summary>
    /// <param name="constraint">The constraint to remove.</param>
    void RemoveConstraint(IConstraint constraint);

    /// <summary>
    /// Checks whether the current instance fullfills all constraints.
    /// </summary>
    /// <returns><c>true</c> if all constraints are fullfilled, <c>false</c> otherwise.</returns>
    bool IsValid();
    /// <summary>
    /// Checks whether the current instance fullfills all constraints.
    /// </summary>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fullfilled.</param>
    /// <returns><c>true</c> if all constraints are fullfilled, <c>false</c> otherwise.</returns>
    bool IsValid(out ICollection<IConstraint> violatedConstraints);

    /// <summary>
    /// An <see cref="EventHandler"/> for events when a new constraint is added.
    /// </summary>
    event EventHandler<EventArgs<IConstraint>> ConstraintAdded;
    /// <summary>
    /// An <see cref="EventHandler"/> for events when a constraint is deleted.
    /// </summary>
    event EventHandler<EventArgs<IConstraint>> ConstraintRemoved;
  }
}
