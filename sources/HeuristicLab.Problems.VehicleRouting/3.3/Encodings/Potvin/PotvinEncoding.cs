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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Drawing;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinEncoding", "Represents a potvin encoding of VRP solutions.")]
  [StorableClass]
  class PotvinEncoding : Item, IVRPEncoding {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }
    
    #region IVRPEncoding Members
    [Storable]
    public ItemList<Tour> Tours { get; set; }

    public int Cities {
      get 
      {
        int cities = 0;

        foreach (Tour tour in Tours) {
          cities += tour.Count;
        }

        return cities;
      }
    }
    #endregion

    [Storable]
    public ItemList<IntValue> Unrouted { get; set; }

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      PotvinEncoding clone = new PotvinEncoding();
      cloner.RegisterClonedObject(this, clone);
      clone.Tours = (ItemList<Tour>)cloner.Clone(this.Tours);
      clone.Unrouted = (ItemList<IntValue>)cloner.Clone(this.Unrouted);
      return clone;
    }

    public PotvinEncoding() {
      Tours = new ItemList<Tour>();
      Unrouted = new ItemList<IntValue>();
    }
    
    public static PotvinEncoding ConvertFrom(IVRPEncoding encoding) {
      PotvinEncoding solution = new PotvinEncoding();

      solution.Tours.AddRange(
        encoding.Tours);

      return solution;
    }

    public static PotvinEncoding ConvertFrom(List<int> route) {
      PotvinEncoding solution = new PotvinEncoding();

      Tour tour = new Tour();
      for (int i = 0; i < route.Count; i++) {
        if (route[i] == 0) {
          if (tour.Count > 0) {
            solution.Tours.Add(tour);
            tour = new Tour();
          }
        } else {
          tour.Add(new IntValue(route[i]));
        }
      }

      return solution;
    }
  }
}
