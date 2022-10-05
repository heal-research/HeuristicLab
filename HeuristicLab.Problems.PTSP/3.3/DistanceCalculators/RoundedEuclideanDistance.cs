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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.PTSP {
  [Item("Rounded Euclidean Distance", "Calculates the euclidean distance and rounds it to the nearest integer.")]
  [StorableType("5AE6854F-505E-438A-8668-2758FEF20861")]
  public sealed class RoundedEuclideanDistance : DistanceCalculator {

    [StorableConstructor]
    private RoundedEuclideanDistance(StorableConstructorFlag _) : base(_) { }
    private RoundedEuclideanDistance(RoundedEuclideanDistance original, Cloner cloner) : base(original, cloner) { }
    public RoundedEuclideanDistance() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedEuclideanDistance(this, cloner);
    }

    public override double Calculate(int from, int to, DoubleMatrix coordinates) {
      return DistanceHelper.GetDistance(DistanceMeasure.RoundedEuclidean, coordinates[from, 0], coordinates[from, 1], coordinates[to, 0], coordinates[to, 1]);
    }
  }
}
