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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which improves VRP solutions.
  /// </summary>
  [Item("VRPIntraRouteImprovementOperator", "An operator which improves VRP solutions.")]
  [StorableClass]
  public sealed class VRPIntraRouteImprovementOperator : VRPImprovementOperator {
    [StorableConstructor]
    private VRPIntraRouteImprovementOperator(bool deserializing) : base(deserializing) { }
    private VRPIntraRouteImprovementOperator(VRPImprovementOperator original, Cloner cloner) : base(original, cloner) { }
    public VRPIntraRouteImprovementOperator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPIntraRouteImprovementOperator(this, cloner);
    }

    protected override int Improve(PotvinEncoding solution) {
      int evaluatedSolutions = 0;

      var rand = RandomParameter.ActualValue;
      var instance = ProblemInstance;
      int sampleSize = SampleSizeParameter.Value.Value;
      int attempts = ImprovementAttemptsParameter.Value.Value;
      int customers = instance.Cities.Value;

      // store city-to-tour assignment and position of the city within the tour
      var tours = new Dictionary<int, Tour>();
      var position = new Dictionary<int, int>();
      foreach (Tour tour in solution.Tours) {
        for (int stop = 0; stop < tour.Stops.Count; stop++) {
          int city = tour.Stops[stop];
          tours[city] = tour;
          position[city] = stop;
        }
      }

      for (int attempt = 0; attempt < attempts; attempt++) {
        for (int sample = 0; sample < sampleSize; sample++) {
          int chosenCust = 1 + rand.Next(customers);
          var custTour = tours[chosenCust];

          double beforeQuality = instance.EvaluateTour(custTour, solution).Quality;
          evaluatedSolutions++;

          custTour.Stops.RemoveAt(position[chosenCust]);
          int place = solution.FindBestInsertionPlace(custTour, chosenCust);
          evaluatedSolutions += custTour.Stops.Count;
          custTour.Stops.Insert(place, chosenCust);

          if (place != position[chosenCust]) {
            double afterQuality = instance.EvaluateTour(custTour, solution).Quality;
            if (afterQuality > beforeQuality) {
              // revert move
              custTour.Stops.RemoveAt(place);
              custTour.Stops.Insert(position[chosenCust], chosenCust);
            } else {
              // accept move and update positions of the cities within the tour
              for (int stop = 0; stop < custTour.Stops.Count; stop++) {
                int city = custTour.Stops[stop];
                position[city] = stop;
              }
              break;
            }
          }
        }
      }

      return evaluatedSolutions;
    }
  }
}
