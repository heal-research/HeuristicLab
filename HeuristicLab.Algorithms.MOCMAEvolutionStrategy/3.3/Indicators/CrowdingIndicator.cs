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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [Item("CrowdingIndicator", "Selection of Offspring based on CrowdingDistance")]
  [StorableClass]
  internal class CrowdingIndicator : Item, IIndicator {
    #region Constructors and Cloning
    [StorableConstructor]
    protected CrowdingIndicator(bool deserializing) : base(deserializing) { }
    protected CrowdingIndicator(CrowdingIndicator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new CrowdingIndicator(this, cloner); }
    public CrowdingIndicator() { }
    #endregion

    public int LeastContributer(IReadOnlyList<Individual> front, MultiObjectiveBasicProblem<RealVectorEncoding> problem) {
      var bounds = problem.Encoding.Bounds;
      var extracted = front.Select(x => x.PenalizedFitness).ToArray();
      if (extracted.Length <= 2) return 0;
      var pointsums = new double[extracted.Length];

      for (var dim = 0; dim < problem.Maximization.Length; dim++) {
        var arr = extracted.Select(x => x[dim]).ToArray();
        Array.Sort(arr);
        var fmax = problem.Encoding.Bounds[dim % bounds.Rows, 1];
        var fmin = bounds[dim % bounds.Rows, 0];
        var pointIdx = 0;
        foreach (var point in extracted) {
          var pos = Array.BinarySearch(arr, point[dim]);
          var d = pos != 0 && pos != arr.Length - 1 ? (arr[pos + 1] - arr[pos - 1]) / (fmax - fmin) : double.PositiveInfinity;
          pointsums[pointIdx] += d;
          pointIdx++;
        }
      }
      return pointsums.Select((value, index) => new { value, index }).OrderBy(x => x.value).First().index;
    }
  }
}
