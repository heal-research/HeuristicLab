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
using System.Drawing;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("TourEncoding", "Represents a base class for tour encodings of VRP solutions.")]
  [StorableClass]
  public abstract class TourEncoding : Item, IVRPEncoding {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    #region IVRPEncoding Members
    public virtual List<Tour> GetTours(ILookupParameter<DoubleMatrix> distanceMatrix = null, int maxVehicles = int.MaxValue) {
      List<Tour> result = new List<Tour>(Tours);

      while (result.Count > maxVehicles) {
        Tour tour = result[result.Count - 1];
        result[result.Count - 2].Cities.AddRange(tour.Cities);

        result.Remove(tour);
      }

      return result;
    }

    public int Cities {
      get {
        int cities = 0;

        foreach (Tour tour in Tours) {
          cities += tour.Cities.Count;
        }

        return cities;
      }
    }
    #endregion

    [Storable]
    public ItemList<Tour> Tours { get; set; }

    public TourEncoding() {
      Tours = new ItemList<Tour>();

      Tours.ToStringChanged += new System.EventHandler(Tours_ToStringChanged);
    }

    void Tours_ToStringChanged(object sender, System.EventArgs e) {
      this.OnToStringChanged();
    }

    [StorableConstructor]
    protected TourEncoding(bool serializing)
      : base() {
    }
    protected TourEncoding(TourEncoding original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void ConvertFrom(IVRPEncoding encoding, TourEncoding solution, ILookupParameter<DoubleMatrix> distanceMatrix) {
      solution.Tours = new ItemList<Tour>(encoding.GetTours(distanceMatrix));
    }

    public static void ConvertFrom(List<int> route, TourEncoding solution) {
      solution.Tours = new ItemList<Tour>();

      Tour tour = new Tour();
      for (int i = 0; i < route.Count; i++) {
        if (route[i] == 0) {
          if (tour.Cities.Count > 0) {
            solution.Tours.Add(tour);
            tour = new Tour();
          }
        } else {
          tour.Cities.Add(route[i]);
        }
      }
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();

      foreach (Tour tour in Tours) {
        foreach (int city in tour.Cities) {
          sb.Append(city);
          sb.Append(" ");
        }

        sb.AppendLine();
      }

      return sb.ToString();
    }
  }
}
