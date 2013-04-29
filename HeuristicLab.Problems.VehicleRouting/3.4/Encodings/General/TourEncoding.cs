#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("TourEncoding", "Represents a base class for tour encodings of VRP solutions.")]
  [StorableClass]
  public abstract class TourEncoding : Item, IVRPEncoding {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }

    #region IVRPEncoding Members
    public virtual void Repair() {
      List<Tour> toBeRemoved = new List<Tour>();
      foreach (Tour tour in Tours) {
        if (tour.Stops.Count == 0)
          toBeRemoved.Add(tour);
      }

      foreach (Tour tour in toBeRemoved)
        Tours.Remove(tour);

      while (Tours.Count > ProblemInstance.Vehicles.Value) {
        Tour tour = Tours[Tours.Count - 1];
        Tours[Tours.Count - 2].Stops.AddRange(tour.Stops);

        Tours.Remove(tour);
      }
    }

    public virtual List<Tour> GetTours() {
      List<Tour> result = new List<Tour>();
      foreach (Tour tour in Tours)
        result.Add(tour.Clone() as Tour);

      return result;
    }

    public int GetTourIndex(Tour tour) {
      int index = -1;

      for (int i = 0; i < Tours.Count; i++) {
        if (Tours[i].IsEqual(tour)) {
          index = i;
          break;
        }
      }

      return index;
    }

    public virtual int GetVehicleAssignment(int tour) {
      return tour;
    }

    public int Cities {
      get {
        int cities = 0;

        foreach (Tour tour in Tours) {
          cities += tour.Stops.Count;
        }

        return cities;
      }
    }
    #endregion

    [Storable]
    public ItemList<Tour> Tours { get; set; }

    [Storable]
    protected IVRPProblemInstance ProblemInstance { get; set; }

    public TourEncoding(IVRPProblemInstance problemInstance) {
      Tours = new ItemList<Tour>();

      ProblemInstance = problemInstance;
    }

    [StorableConstructor]
    protected TourEncoding(bool serializing)
      : base(serializing) {
    }

    protected TourEncoding(TourEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.Tours = (ItemList<Tour>)cloner.Clone(original.Tours);
      if (original.ProblemInstance != null && cloner.ClonedObjectRegistered(original.ProblemInstance))
        this.ProblemInstance = (IVRPProblemInstance)cloner.Clone(original.ProblemInstance);
      else
        this.ProblemInstance = original.ProblemInstance;
    }

    public static void ConvertFrom(IVRPEncoding encoding, TourEncoding solution, IVRPProblemInstance problemInstance) {
      solution.Tours = new ItemList<Tour>(encoding.GetTours());
      solution.Repair();
    }

    public static void ConvertFrom(List<int> route, TourEncoding solution) {
      solution.Tours = new ItemList<Tour>();

      Tour tour = new Tour();
      for (int i = 0; i < route.Count; i++) {
        if (route[i] <= 0) {
          if (tour.Stops.Count > 0) {
            solution.Tours.Add(tour);
            tour = new Tour();
          }
        } else {
          tour.Stops.Add(route[i]);
        }
      }

      solution.Repair();
    }
  }
}
