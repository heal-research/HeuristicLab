#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  [StorableType("231ae77d-4352-4a70-8662-5f3d5d44f095")]
  public class EnumerableBoolEqualityComparer : IEqualityComparer<IEnumerable<bool>> {
    public bool Equals(IEnumerable<bool> first, IEnumerable<bool> second) {
      return first.SequenceEqual(second);
    }
    public int GetHashCode(IEnumerable<bool> obj) {
      unchecked {
        int hash = 17;
        foreach (var bit in obj) {
          hash = hash * 29 + (bit ? 1231 : 1237);
        }
        return hash;
      }
    }
  }
}
