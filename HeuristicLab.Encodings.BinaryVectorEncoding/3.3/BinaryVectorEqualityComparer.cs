#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  public class BinaryVectorEqualityComparer : EqualityComparer<BinaryVector> {
    public override bool Equals(BinaryVector first, BinaryVector second) {
      return first.SequenceEqual(second);
    }
    public override int GetHashCode(BinaryVector obj) {
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
