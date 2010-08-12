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
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Parameters;
using System.Collections.Generic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("StochasticSimpleLocalSearchSingleMoveGenerator", "Generates one random simple local search move from a given Alba VRP encoding.")]
  [StorableClass]
  public sealed class StochasticSimpleLocalSearchSingleMoveGenerator : SimpleLocalSearchMoveGenerator, IStochasticOperator, ISingleMoveGenerator, IAlbaSimpleLocalSearchMoveOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    
    [StorableConstructor]
    private StochasticSimpleLocalSearchSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public StochasticSimpleLocalSearchSingleMoveGenerator()
      : base() {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public static SimpleLocalSearchMove Apply(AlbaEncoding individual, int cities, IRandom rand) {
      int index1 = -1;
      int index2 = -1;

      List<Tour> validTours = new List<Tour>();
      foreach (Tour tour in individual.Tours) {
        if (tour.Cities.Count >= 4)
          validTours.Add(tour);
      }

      if (validTours.Count > 0) {
        Tour chosenTour = validTours[rand.Next(validTours.Count)];
        int currentTourStart = -1;
        for (int i = 0; i < individual.Length; i++) {
          if (individual[i] + 1 == chosenTour.Cities[0]) {
            currentTourStart = i;
            break;
          }
        }

        int currentTourEnd = currentTourStart;
        while (individual[currentTourEnd] < individual.Cities &&
            currentTourEnd < individual.Length) {
          currentTourEnd++;
        }

        int tourLength = currentTourEnd - currentTourStart;
        int a = rand.Next(tourLength - 3);
        index1 = currentTourStart + a;
        index2 = currentTourStart + rand.Next(a + 2, tourLength - 1);
      }

      return new SimpleLocalSearchMove(index1, index2, individual);
    }

    protected override SimpleLocalSearchMove[] GenerateMoves(AlbaEncoding individual) {
      List<SimpleLocalSearchMove> moves = new List<SimpleLocalSearchMove>();

      SimpleLocalSearchMove move = Apply(individual, Cities, RandomParameter.ActualValue);
      if(move != null)
        moves.Add(move);

      return moves.ToArray();
    }
  }
}
