#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [StorableClass]
  public class Tour : Item {
    [Storable]
    public List<int> Cities { get; private set; }

    public Tour() {
      Cities = new List<int>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Tour clone = base.Clone(cloner) as Tour;
      clone.Cities = new List<int>(Cities);

      return clone;
    }

    public bool Feasible(DoubleArray dueTimeArray,
      DoubleArray serviceTimeArray, DoubleArray readyTimeArray, DoubleArray demandArray, DoubleValue capacity,
      DoubleMatrix coordinates, ILookupParameter<DoubleMatrix> distanceMatrix, BoolValue useDistanceMatrix) {
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
        coordinates,
        distanceMatrix,
        useDistanceMatrix);

      return eval.Overload < double.Epsilon && eval.Tardiness < double.Epsilon;
    }
    
    public double GetLength(DoubleMatrix coordinates, 
      ILookupParameter<DoubleMatrix> distanceMatrix, 
      BoolValue useDistanceMatrix) {
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
            cities[i - 1], cities[i], coordinates, distanceMatrix, useDistanceMatrix); 
        }
      }

      return length;
    }
  }
}
