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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("65C04216-1441-41EE-91B7-A80B2AD5E332")]
  [Item("ManhattanDistance", "A distance function that uses block distance")]
  public class ManhattanDistance : DistanceBase<IEnumerable<double>> {
    #region HLConstructors & Cloning
    [StorableConstructor]
    protected ManhattanDistance(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }
    protected ManhattanDistance(ManhattanDistance original, Cloner cloner)
      : base(original, cloner) { }
    public ManhattanDistance() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManhattanDistance(this, cloner);
    }
    #endregion

    public static double GetDistance(IEnumerable<double> point1, IEnumerable<double> point2) {
      using (IEnumerator<double> p1Enum = point1.GetEnumerator(), p2Enum = point2.GetEnumerator()) {
        var sum = 0.0;
        while (p1Enum.MoveNext() & p2Enum.MoveNext())
          sum += Math.Abs(p1Enum.Current - p2Enum.Current);
        if (p1Enum.MoveNext() || p2Enum.MoveNext()) throw new ArgumentException("Manhattan distance not defined on vectors of different length");
        return sum;
      }
    }

    public override double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a, b);
    }
  }
}