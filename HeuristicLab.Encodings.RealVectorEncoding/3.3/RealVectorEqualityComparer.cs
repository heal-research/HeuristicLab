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

using System;
using System.Collections.Generic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [NonDiscoverableType]
  public class RealVectorEqualityComparer : EqualityComparer<RealVector> {
    public override bool Equals(RealVector x, RealVector y) {
      if (ReferenceEquals(x, y)) return true;
      if (x == null || y == null) return false;
      if (x.Length != y.Length) return false;
      for (var i = 0; i < x.Length; i++)
        if (x[i] != y[i]) return false;
      return true;
    }

    public override int GetHashCode(RealVector obj) {
      if (obj == null) throw new ArgumentNullException("obj", "RealVectorEqualityComparer: Cannot compute hash value of null.");
      unchecked {
        int hash = 17;
        foreach (var o in obj)
          hash = hash * 31 + o.GetHashCode();
        return hash;
      }
    }
  }
}
