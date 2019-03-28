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
using HEAL.Attic;
using HeuristicLab.Problems.TestFunctions.MultiObjective;
namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [Item("HypervolumeIndicator", "Selection of Offspring based on contributing Hypervolume")]
  [StorableType("ADF439D6-64E4-4C92-A4D3-E8C05B050406")]
  internal class HypervolumeIndicator : Item, IIndicator {
    #region Constructors and Cloning
    [StorableConstructor]
    protected HypervolumeIndicator(StorableConstructorFlag _) : base(_) { }
    protected HypervolumeIndicator(HypervolumeIndicator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new HypervolumeIndicator(this, cloner); }
    public HypervolumeIndicator() { }
    #endregion

    public int LeastContributer(IReadOnlyList<Individual> front, MultiObjectiveBasicProblem<RealVectorEncoding> problem) {
      var frontCopy = front.Select(x => x.PenalizedFitness).ToList();
      if (frontCopy.Count <= 1) return 0;
      var p = problem as MultiObjectiveTestFunctionProblem;
      var refPoint = BuildReferencePoint(p != null ? frontCopy.Concat(new[] { p.ReferencePoint.CloneAsArray() }) : frontCopy, problem.Maximization);
      var contributions = Enumerable.Range(0, frontCopy.Count).Select(i => Contribution(frontCopy, i, problem.Maximization, refPoint));
      return contributions.Select((value, index) => new { value, index }).OrderBy(x => x.value).First().index;
    }

    #region Helpers
    private static double Contribution(IList<double[]> front, int idx, bool[] maximization, double[] refPoint) {
      var point = front[idx];
      front.RemoveAt(idx);
      var contribution = -Hypervolume.Calculate(front.ToArray(), refPoint, maximization);
      front.Insert(idx, point);
      return contribution;
    }
    private static double[] BuildReferencePoint(IEnumerable<double[]> front, IReadOnlyList<bool> maximization) {
      var refPoint = new double[maximization.Count];
      foreach (var point in front)
        for (var i = 0; i < maximization.Count; i++)
          refPoint[i] = maximization[i] ? Math.Min(refPoint[i], point[i]) : Math.Max(refPoint[i], point[i]);
      return refPoint;
    }
    #endregion
  }
}
