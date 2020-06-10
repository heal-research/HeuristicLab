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

using System;
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("62D2A0C2-52A0-4788-8666-E575790B29DA")]
  /// <summary>
  /// Represents the definition of a result with name, description and data type
  /// </summary>
  public interface IResultDefinition : INamedItem {
    Type DataType { get; }
    //TODO implement enabled property for deactivation of result calculation
    //bool Enabled { get; set; }
  }

  [StorableType("e05050f3-5f92-4245-b733-7097d496e781")]
  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  public interface IResult : IResultDefinition {
    IItem Value { get; set; }
    bool HasValue { get; }

    void Reset();
    event EventHandler ValueChanged;
  }

  [StorableType("3B7B9DF0-0BEB-4FF5-8430-A07749FF2EB4")]
  /// <summary>
  /// Represents a typed result which has a name and a data type.
  /// </summary>
  public interface IResult<T> : IResult {
    new T Value { get; set; }
  }


}
