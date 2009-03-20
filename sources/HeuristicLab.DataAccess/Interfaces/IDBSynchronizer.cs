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

namespace HeuristicLab.DataAccess.Interfaces {
  /// <summary>
  /// Transaction manager for the DB access layer
  /// </summary>
  public interface IDBSynchronizer {
    /// <summary>
    /// This event is fired when an update occurs
    /// </summary>
    event EventHandler OnUpdate; 

    /// <summary>
    /// Enables the auto update of the DB
    /// </summary>
    /// <param name="interval"></param>
    void EnableAutoUpdate(TimeSpan interval);

    /// <summary>
    /// Disables the auto update of the DB
    /// </summary>
    void DisableAutoUpdate();

    /// <summary>
    /// Update the DB from the cache
    /// </summary>
    void UpdateDB();
  }
}
