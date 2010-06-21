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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaEncoding", "Represents an alba encoding of VRP solutions.")]
  [StorableClass]
  class AlbaEncoding: Permutation, IVRPEncoding {
    #region IVRPEncoding Members
    [Storable]
    private int cities;

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      AlbaEncoding clone = new AlbaEncoding(
        new Permutation(this.PermutationType, this.array), cities);
      cloner.RegisterClonedObject(this, clone);
      clone.readOnly = readOnly;
      return clone;
    }

    public AlbaEncoding(Permutation permutation, int cities): base(PermutationTypes.RelativeDirected) {
      this.array = new int[permutation.Length];
      for (int i = 0; i < array.Length; i++)
        this.array[i] = permutation[i];

      this.cities = cities;
    }

    [StorableConstructor]
    private AlbaEncoding(bool serializing)
      : base() {
    }

    public ItemList<Tour> Tours {
      get {
        ItemList<Tour> result = new ItemList<Tour>();
        Tour tour = new Tour();
        tour.Add(new IntValue(0));

        for (int i = 0; i < this.array.Length; i++) {
          if (this.array[i] >= cities) {
            if (tour.Count > 1) {
              tour.Add(new IntValue(0));
              result.Add(tour);

              tour = new Tour();
              tour.Add(new IntValue(0));
            }
          } else {
            tour.Add(new IntValue(this.array[i] + 1));
          }       
        }

        if (tour.Count > 1) {
          tour.Add(new IntValue(0));
          result.Add(tour);
        }

        return result;
      }
    }

    public int Cities {
      get { return cities; }
    }

    #endregion

    public static AlbaEncoding ConvertFrom(IVRPEncoding encoding) {
        ItemList<Tour> tours = encoding.Tours;

        int cities = 0;
        foreach (Tour tour in tours) {
          cities += tour.Count;
        }
        int[] array = new int[cities + tours.Count - 2];
        int delimiter = cities;
        int arrayIndex = 0;

        foreach (Tour tour in tours) {
          foreach(IntValue city in tour) {
            array[arrayIndex] = city.Value;

            arrayIndex++;
          }

          if (arrayIndex != array.Length) {
            array[arrayIndex] = delimiter;
            delimiter++;
            arrayIndex++;
          }
        }

      AlbaEncoding solution = new AlbaEncoding(new Permutation(PermutationTypes.RelativeUndirected), cities);

      return solution;
    }

    

  }
}
