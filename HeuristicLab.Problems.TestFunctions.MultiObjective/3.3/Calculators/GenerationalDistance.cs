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

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  /// <summary>
  /// The generational Distance is defined as the pth-root of the sum of all d[i]^(p) divided by the size of the front
  ///  where d[i] is the minimal distance the ith point of the evaluated front has to any point in the optimal pareto front.   
  ///  p is a dampening factor and is normally set to 1. 
  ///  http://shodhganga.inflibnet.ac.in/bitstream/10603/15070/28/28_appendix_h.pdf
  /// </summary>
  public static class GenerationalDistance {

    public static double Calculate(IEnumerable<double[]> front, IEnumerable<double[]> optimalFront, double p) {
      if (front == null || optimalFront == null) throw new ArgumentNullException("Fronts must not be null.");
      if (!front.Any()) throw new ArgumentException("Front must not be empty.");
      if (p == 0.0) throw new ArgumentException("p must not be 0.0.");


      double sum = front.Select(r => Math.Pow(Utilities.MinimumDistance(r, optimalFront), p)).Sum();
      return Math.Pow(sum, 1 / p) / front.Count();
    }

  }
}
