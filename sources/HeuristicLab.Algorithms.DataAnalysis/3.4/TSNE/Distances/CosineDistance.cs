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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {

  /// <summary>
  /// The angluar distance as defined as a normalized distance measure dependent on the angle between two vectors.
  /// It is designed for vectors with all positive coordinates.
  /// </summary>
  [StorableClass]
  [Item("CosineDistance", "The angluar distance as defined as a normalized distance measure dependent on the angle between two vectors.\nIt is designed for vectors with all positive coordinates")]
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
    public static double GetDistance(IReadOnlyList<double> point1, IReadOnlyList<double> point2) {
      if (point1.Count != point2.Count) throw new ArgumentException("Cosine distance not defined on vectors of different length");
      var innerprod = 0.0;
      var length1 = 0.0;
      var length2 = 0.0;

      for (var i = 0; i < point1.Count; i++) {
        double d1 = point1[i], d2 = point2[i];
        innerprod += d1 * d2;
        length1 += d1 * d1;
        length2 += d2 * d2;
      }
      var l = Math.Sqrt(length1 * length2);
      if (l.IsAlmost(0)) throw new ArgumentException("Cosine distance is not defined on vectors of length 0");
      return 1 - innerprod / l;
    }
    #endregion
    public override double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a.ToArray(), b.ToArray());
    }
  }
}
