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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  /// <summary>
  /// Spacing is defined as the standarddeviation of all d[i] 
  /// where d[i] is the minimum eukildean distance any point has 
  /// to all OTHER points in the same front 
  /// </summary>
  public static class Spacing {

    public static double Calculate(IEnumerable<double[]> front) {
      if (front == null) throw new ArgumentException("Front must not be null.");
      if (!front.Any()) throw new ArgumentException("Front must  not be empty.");

      var points = front.ToList();
      var d = new List<double>();

      foreach (double[] r in points) {
        var point = r;
        var otherPoints = points.Where(p => p != point).DefaultIfEmpty(point);
        double dist = Utilities.MinimumDistance(point, otherPoints);
        d.Add(dist);
      }

      return d.StandardDeviationPop();
    }

  }
}
