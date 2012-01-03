#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Common base class of <c>BeginToken</c> and <c>EndToken</c>
  /// that surround a composite element.
  /// </summary>
  public abstract class CompositeTokenBase : SerializationTokenBase {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int TypeId;


    /// <summary>
    /// The object's id for references in case it is a reference type.
    /// </summary>
    public readonly int? Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeTokenBase"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="typeId">The type id.</param>
    /// <param name="id">The object id.</param>
    public CompositeTokenBase(string name, int typeId, int? id)
      : base(name) {
      TypeId = typeId;
      Id = id;
    }
  }

}