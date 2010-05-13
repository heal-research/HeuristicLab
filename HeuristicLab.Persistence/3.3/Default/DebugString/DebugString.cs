#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.DebugString {

  /// <summary>
  /// Simple write-only format for debugging purposes.
  /// </summary>
  [StorableClass]  
  public class DebugString : ISerialData {

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    [Storable]
    public string Data { get; set; }

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
