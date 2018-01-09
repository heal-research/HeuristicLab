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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// The angular distance as defined as a normalized distance measure dependent on the angle between two vectors.
  /// </summary>
  [StorableClass]
  [Item("CosineDistance", "The angular distance as defined as a normalized distance measure dependent on the angle between two vectors.")]
  public class CosineDistance : DistanceBase<IEnumerable<double>> {
    #region HLConstructors & Cloning
    [StorableConstructor]
    protected CosineDistance(bool deserializing) : base(deserializing) { }
    protected CosineDistance(CosineDistance original, Cloner cloner)
      : base(original, cloner) { }
    public CosineDistance() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CosineDistance(this, cloner);
    }
    #endregion

    #region statics
    public static double GetDistance(IEnumerable<double> point1, IEnumerable<double> point2) {
      using (IEnumerator<double> p1Enum = point1.GetEnumerator(), p2Enum = point2.GetEnumerator()) {
        var innerprod = 0.0;
        var length1 = 0.0;
        var length2 = 0.0;
        while (p1Enum.MoveNext() & p2Enum.MoveNext()) {
          double d1 = p1Enum.Current, d2 = p2Enum.Current;
          innerprod += d1 * d2;
          length1 += d1 * d1;
          length2 += d2 * d2;
        }
        var divisor = Math.Sqrt(length1 * length2);
        if (divisor.IsAlmost(0)) throw new ArgumentException("Cosine distance is not defined on vectors of length 0");
        if (p1Enum.MoveNext() || p2Enum.MoveNext()) throw new ArgumentException("Cosine distance not defined on vectors of different length");
        return 1 - innerprod / divisor;
      }
    }
    #endregion
    public override double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a, b);
    }
  }
}