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
  [StorableClass]
  [Item("EuclideanDistance", "A norm function that uses Euclidean distance")]
  public class EuclideanDistance : DistanceBase<IEnumerable<double>> {

    #region HLConstructors & Boilerplate
    [StorableConstructor]
    protected EuclideanDistance(bool deserializing) : base(deserializing) { }
    protected EuclideanDistance(EuclideanDistance original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new EuclideanDistance(this, cloner); }
    public EuclideanDistance() { }
    #endregion

    public static double GetDistance(double[] point1, double[] point2) {
      if (point1.Length != point2.Length) throw new ArgumentException("Euclidean distance not defined on vectors of different length");
      return Math.Sqrt(point1.Zip(point2, (a1, b1) => (a1 - b1) * (a1 - b1)).Sum());
    }

    public override double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a.ToArray(), b.ToArray());
    }
  }
}
