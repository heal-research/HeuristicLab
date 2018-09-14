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

using System.Collections.Generic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  public class StandardEdgeEqualityComparer : EqualityComparer<Edge> {
    public override bool Equals(Edge x, Edge y) {
      bool inDirection = x.Source == y.Source && x.Target == y.Target;
      if (!x.Bidirectional && !y.Bidirectional) return inDirection;
      else return inDirection || x.Source == y.Target && x.Target == y.Source;
    }

    public override int GetHashCode(Edge edge) {
      return (int)((long)edge.Source.GetHashCode() + (long)edge.Target.GetHashCode());
    }
  }
}
