#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [StorableClass]
  public class Tour : Item {
    [Storable]
    public List<int> Cities { get; private set; }

    [StorableConstructor]
    protected Tour(bool deserializing) : base(deserializing) { }
    protected Tour(Tour original, Cloner cloner)
      : base(original, cloner) {
      Cities = new List<int>(original.Cities);
    }
    public Tour() {
      Cities = new List<int>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Tour(this, cloner);
    }

    public bool Feasible(DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DistanceMatrix distMatrix) {
      TourEvaluation eval = VRPEvaluator.EvaluateTour(this,
        dueTimeArray,
        serviceTimeArray,
        readyTimeArray,
        demandArray,
        capacity,
        new DoubleValue(0),
        new DoubleValue(0),
        new DoubleValue(0),
        new DoubleValue(1),
        new DoubleValue(1),
        distMatrix);

      return eval.Overload < double.Epsilon && eval.Tardiness < double.Epsilon;
    }

    public double GetLength(DistanceMatrix distMatrix) {
      double length = 0;

      if (Cities.Count > 0) {
        List<int> cities = new List<int>();
        cities.Add(0);
        foreach (int city in Cities) {
          cities.Add(city);
        }
        cities.Add(0);

        for (int i = 1; i < cities.Count; i++) {
          length += VRPUtilities.GetDistance(
            cities[i - 1], cities[i], distMatrix);
        }
      }

      return length;
    }
  }
}
