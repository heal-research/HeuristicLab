#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [NonDiscoverableType]
  public class PermutationEqualityComparer : EqualityComparer<Permutation> {
    public override bool Equals(Permutation x, Permutation y) {
      if (x.PermutationType != y.PermutationType) return false;
      if (x.Length != y.Length) return false;
      switch (x.PermutationType) {
        case PermutationTypes.Absolute:
          return EqualsAbsolute(x, y);
        case PermutationTypes.RelativeDirected:
          return EqualsRelative(x, y, true);
        case PermutationTypes.RelativeUndirected:
          return EqualsRelative(x, y, false);
        default:
          throw new InvalidOperationException("unknown permutation type");
      }
    }

    private bool EqualsAbsolute(Permutation x, Permutation y) {
      return x.SequenceEqual(y);
    }

    private bool EqualsRelative(Permutation x, Permutation y, bool directed) {
      int[] edgesX = CalculateEdgesVector(x);
      int[] edgesY = CalculateEdgesVector(y);
      for (int i = 0; i < x.Length; i++)
        if ((edgesX[i] != edgesY[i]) && (directed || edgesX[edgesY[i]] != i))
          return false;
      return true;
    }

    private int[] CalculateEdgesVector(Permutation permutation) {
      // transform path representation into adjacency representation
      int[] edgesVector = new int[permutation.Length];
      for (int i = 0; i < permutation.Length - 1; i++)
        edgesVector[permutation[i]] = permutation[i + 1];
      edgesVector[permutation[permutation.Length - 1]] = permutation[0];
      return edgesVector;
    }

    public override int GetHashCode(Permutation obj) {
      if (obj == null) return 0;
      return GenerateHashString(obj).GetHashCode();
    }

    private string GenerateHashString(Permutation p) {
      StringBuilder sb = new StringBuilder();
      if (p.PermutationType == PermutationTypes.Absolute) {
        for (int i = 0; i < p.Length; i++)
          sb.Append(p[i].ToString() + ";");
      } else {
        int i = 0;
        while (p[i] != 0) i++; // always start at element 0
        if (p.PermutationType == PermutationTypes.RelativeDirected) {
          for (int j = 0; j < p.Length; j++)
            sb.Append(p.GetCircular(i + j).ToString() + ";");
        } else {
          bool goLeft = p.GetCircular(i - 1) < p.GetCircular(i + 1); // go in direction of the lowest edge so that the total inversion and its original return the same hash code
          for (int j = 0; j < p.Length; j++) {
            if (goLeft) sb.Append(p.GetCircular(i - j).ToString() + ";");
            else sb.Append(p.GetCircular(i + j).ToString() + ";");
          }
        }
      }
      return sb.ToString();
    }
  }
}
