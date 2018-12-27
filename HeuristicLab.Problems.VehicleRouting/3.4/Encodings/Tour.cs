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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting {
  [StorableClass]
  public class Tour : Item {
    [Storable]
    public List<int> Stops { get; private set; }

    public Tour() {
      Stops = new List<int>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Tour(this, cloner);
    }

    protected Tour(Tour original, Cloner cloner)
      : base(original, cloner) {
      this.Stops = new List<int>(original.Stops);
    }

    [StorableConstructor]
    protected Tour(bool deserializing) : base(deserializing) { }

    public double GetTourLength(IVRPProblemInstance instance, IVRPEncoding solution) {
      double length = 0;

      if (Stops.Count > 0) {
        List<int> cities = new List<int>();
        cities.Add(0);
        foreach (int city in Stops) {
          cities.Add(city);
        }
        cities.Add(0);

        for (int i = 1; i < cities.Count; i++) {
          length += instance.GetDistance(cities[i - 1], cities[i], solution);
        }
      }

      return length;
    }

    public bool IsEqual(Tour tour) {
      bool equal = (tour != null) && (tour.Stops.Count == Stops.Count);
      int index = 0;

      while (equal && index < Stops.Count) {
        equal = equal && tour.Stops[index] == Stops[index];
        index++;
      }

      return equal;
    }
  }
}
