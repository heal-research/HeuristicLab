#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.DebugString {

  /// <summary>
  /// Simple write-only format for debugging purposes.
  /// </summary>
  [StorableType("11A7D76E-AB88-43E1-97E8-D195959C34A2")]
  public class DebugString : ISerialData {

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    [Storable]
    public string Data { get; set; }

    [StorableConstructor]
    protected DebugString(StorableConstructorFlag _) { }
    private DebugString() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugString"/> class.
    /// </summary>
    /// <param name="s">The string.</param>
    public DebugString(string s) {
      Data = s;
    }

  }

}
