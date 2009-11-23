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
  /// Interface to represent an operator graph.
  /// </summary>
  public interface IOperatorGraph : IItem {
    /// <summary>
    /// Gets all operators of the current instance.
    /// </summary>
    ICollection<IOperator> Operators { get; }
    /// <summary>
    /// Gets or sets the initial operator (the starting one) of the current instance.
    /// </summary>
    IOperator InitialOperator { get; set; }

    /// <summary>
    /// Adds the given operator to the current instance.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    void AddOperator(IOperator op);
    /// <summary>
    /// Removes an operator with the specified <paramref name="guid"/> from the current instance.
    /// </summary>
    /// <param name="guid">The unique id of the operator to remove.</param>
    void RemoveOperator(IOperator op);
    /// <summary>
    /// Clears the current instance.
    /// </summary>
    void Clear();

    /// <summary>
    /// Occurs when a new operator has been added to the current instance.
    /// </summary>
    event EventHandler<EventArgs<IOperator>> OperatorAdded;
    /// <summary>
    /// Occurs when an operator has been deleted from the current instance.
    /// </summary>
    event EventHandler<EventArgs<IOperator>> OperatorRemoved;
    /// <summary>
    /// Occurs when the initial operator (the starting one) has been changed.
    /// </summary>
    event EventHandler InitialOperatorChanged;
  }
}
