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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [Item("MinimalDistanceIndicator", "Selection of Offspring based on distance to nearest neighbour")]
  [StorableType("FBBD4517-164C-4DEE-B87D-49B99172EDF4")]
  internal class MinimalDistanceIndicator : Item, IIndicator {

    #region Constructor and Cloning
    [StorableConstructor]
    protected MinimalDistanceIndicator(StorableConstructorFlag _) : base(_) { }
    protected MinimalDistanceIndicator(MinimalDistanceIndicator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new MinimalDistanceIndicator(this, cloner); }
    public MinimalDistanceIndicator() { }
    #endregion

    public int LeastContributer(IReadOnlyList<Individual> front, MultiObjectiveBasicProblem<RealVectorEncoding> problem) {
      var extracted = front.Select(x => x.PenalizedFitness).ToArray();
      if (extracted.Length <= 2) return 0;
      var distances = CalcDistances(extracted);
      var mindexI = 0;
      var mindexJ = 0;
      var min = double.MaxValue;
      for (var i = 0; i < extracted.Length; i++) {
        var d = double.MaxValue;
        var minj = 0;
        for (var j = 0; j < extracted.Length; j++) {
          if (i == j) continue;
          var d1 = distances[i, j];
          if (!(d1 < d)) continue;
          minj = j;
          d = d1;
        }
        if (!(d < min)) continue;
        min = d;
        mindexI = i;
        mindexJ = minj;
      }

      //break tie with distance to second nearest
      var minI = double.MaxValue;
      var minJ = double.MaxValue;

      for (var i = 0; i < extracted.Length; i++) {
        double d;
        if (mindexI != i) {
          d = distances[mindexI, i];
          if (!d.IsAlmost(min)) minI = Math.Min(minI, d);
        }
        if (mindexJ == i) continue;
        d = distances[mindexJ, i];
        if (!d.IsAlmost(min)) minJ = Math.Min(minJ, d);
      }

      //find min
      return minI < minJ ? mindexI : mindexJ;
    }

    #region Helpers
    private static double[,] CalcDistances(IReadOnlyList<double[]> extracted) {
      var res = new double[extracted.Count, extracted.Count];
      for (var i = 0; i < extracted.Count; i++)
        for (var j = 0; j < i; j++)
          res[i, j] = res[j, i] = Dist(extracted[i], extracted[j]);
      return res;
    }

    private static double Dist(IEnumerable<double> a, IEnumerable<double> b) {
      return Math.Sqrt(a.Zip(b, (x, y) => (x - y) * (x - y)).Sum());
    }
    #endregion
  }
}
