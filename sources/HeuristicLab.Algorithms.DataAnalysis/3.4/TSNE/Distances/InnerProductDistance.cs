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
  [Item("InnerProductDistance", "The angluar distance as defined as a normalized distance measure dependent on the angle between two vectors.\nIt is designed for vectors with all positive coordinates")]
  public class InnerProductDistance : DistanceBase<IEnumerable<double>> {

    #region HLConstructors
    [StorableConstructor]
    protected InnerProductDistance(bool deserializing) : base(deserializing) { }
    protected InnerProductDistance(InnerProductDistance original, Cloner cloner)
      : base(original, cloner) { }
    public InnerProductDistance() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new InnerProductDistance(this, cloner);
    }
    #endregion

    #region statics
    public static double GetDistance(IEnumerable<double> point1, IEnumerable<double> point2) {
      var xs = point1.GetEnumerator();
      var ys = point2.GetEnumerator();
      var sum = 0.0;
      while(xs.MoveNext() & ys.MoveNext()) {
        if(xs.Current < 0 || ys.Current < 0) throw new ArgumentException("Inner product distance is only defined for vectors with non-negative elements");
        sum += xs.Current * ys.Current;
      }
      if(xs.MoveNext() || ys.MoveNext()) throw new ArgumentException("Enumerables contain a different number of elements");
      return sum;
    }
    #endregion
    public override double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a, b);
    }
  }
}
