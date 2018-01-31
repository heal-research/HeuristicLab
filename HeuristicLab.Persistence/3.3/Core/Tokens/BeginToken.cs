#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Persistence.Core.Tokens {


  /// <summary>
  /// Marks the beginning of a composite element.
  /// </summary>
  public class BeginToken : CompositeTokenBase {

    /// <summary>
    /// Initializes a new instance of the <see cref="BeginToken"/> class.
    /// </summary>
    /// <param name="name">The token name.</param>
    /// <param name="typeId">The type id.</param>
    /// <param name="id">The object id.</param>
    public BeginToken(string name, int typeId, int? id) : base(name, typeId, id) { }
  }

}