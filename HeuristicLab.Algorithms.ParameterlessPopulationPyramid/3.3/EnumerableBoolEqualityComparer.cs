#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  public class EnumerableBoolEqualityComparer : IEqualityComparer<IEnumerable<bool>> {
    public bool Equals(IEnumerable<bool> first, IEnumerable<bool> second) {
      return first.SequenceEqual(second);
    }
    public int GetHashCode(IEnumerable<bool> obj) {
      int hash = 0;
      int word = 1;
      foreach (bool bit in obj) {
        // load bits into an integer
        word <<= 1;
        word |= Convert.ToInt32(bit);
        // only happens when the leading 1 reaches the sign bit
        if (word < 0) {
          // combine word into the hash
          hash ^= word;
          word = 1;
        }
      }
      // combine in any remaining content
      if (word > 1) {
        hash ^= word;
      }
      return hash;
    }
  }
}
