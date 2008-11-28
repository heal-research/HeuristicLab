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
using HeuristicLab.Data;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// The base class for a permutation, which is an ordered combination. 
  /// </summary>
  public class Permutation : IntArrayData {
    /// <summary>
    /// Initializes a new instance of <see cref="Permutation"/>.
    /// </summary>
    public Permutation()
      : base() {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Permutation"/> with the given 
    /// <paramref name="permutation"/> array.
    /// </summary>
    /// <param name="permutation">The permutation array to assign.</param>
    public Permutation(int[] permutation)
      : base(permutation) {
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <param name="clonedObjects">A dictionary of all already cloned objects. (Needed to 
    /// avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Permutation"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Permutation clone = new Permutation((int[])Data.Clone());
      clonedObjects.Add(Guid, clone);
      return clone;
    }
  }
}
