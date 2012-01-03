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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaStochasticIntraRouteInversionSingleMoveGenerator", "Generates one random intra route inversion move from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaStochasticIntraRouteInversionSingleMoveGenerator : AlbaIntraRouteInversionMoveGenerator,
    IStochasticOperator, ISingleMoveGenerator, IAlbaIntraRouteInversionMoveOperator, IMultiVRPMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["AlbaIntraRouteInversionMove"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private AlbaStochasticIntraRouteInversionSingleMoveGenerator(bool deserializing) : base(deserializing) { }
    private AlbaStochasticIntraRouteInversionSingleMoveGenerator(AlbaStochasticIntraRouteInversionSingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public AlbaStochasticIntraRouteInversionSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaStochasticIntraRouteInversionSingleMoveGenerator(this, cloner);
    }

    public static AlbaIntraRouteInversionMove Apply(AlbaEncoding individual, int cities, IRandom rand) {
      int index1 = -1;
      int index2 = -1;

      List<Tour> validTours = new List<Tour>();
      foreach (Tour tour in individual.GetTours()) {
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
        while (currentTourEnd < individual.Length &&
          individual[currentTourEnd] < individual.Cities) {
          currentTourEnd++;
        }

        int tourLength = currentTourEnd - currentTourStart;
        int a = rand.Next(tourLength - 3);
        index1 = currentTourStart + a;
        index2 = currentTourStart + rand.Next(a + 2, tourLength - 1);
      }

      return new AlbaIntraRouteInversionMove(index1, index2, individual);
    }

    protected override AlbaIntraRouteInversionMove[] GenerateMoves(AlbaEncoding individual) {
      List<AlbaIntraRouteInversionMove> moves = new List<AlbaIntraRouteInversionMove>();

      AlbaIntraRouteInversionMove move = Apply(individual, Cities, RandomParameter.ActualValue);
      if (move != null)
        moves.Add(move);

      return moves.ToArray();
    }
  }
}
