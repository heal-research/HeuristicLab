#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarMoveMaker", "Peforms the two opt star move on a given VRP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinTwoOptStarMoveMaker : PotvinMoveMaker, IPotvinTwoOptStarMoveOperator, IMoveMaker {
    public ILookupParameter<PotvinTwoOptStarMove> TwoOptStarMoveParameter {
      get { return (ILookupParameter<PotvinTwoOptStarMove>)Parameters["PotvinTwoOptStarMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return TwoOptStarMoveParameter; }
    }

    [StorableConstructor]
    protected PotvinTwoOptStarMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinTwoOptStarMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinTwoOptStarMove>("PotvinTwoOptStarMove", "The moves that should be made."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarMoveMaker(this, cloner);
    }

    protected PotvinTwoOptStarMoveMaker(PotvinTwoOptStarMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void GetSegments(PotvinTwoOptStarMove move, out List<int> segmentX1, out List<int> segmentX2) {
      PotvinEncoding solution = move.Individual as PotvinEncoding;

      Tour route1 = solution.Tours[move.Tour1];
      Tour route2 = solution.Tours[move.Tour2];

      int x1 = move.X1;
      int x2 = move.X2;

      int count = route1.Stops.Count - x1;
      segmentX1 = new List<int>();
      if (count > 0) {
        segmentX1 = route1.Stops.GetRange(x1, count);
      }

      count = route2.Stops.Count - x2;
      segmentX2 = new List<int>();
      if (count > 0) {
        segmentX2 = route2.Stops.GetRange(x2, count);
      }
    }

    public static void Apply(PotvinEncoding solution, PotvinTwoOptStarMove move, IVRPProblemInstance problemInstance) {
      List<int> segmentX1;
      List<int> segmentX2;
      GetSegments(move, out segmentX1, out segmentX2);

      Tour route1 = solution.Tours[move.Tour1];
      Tour route2 = solution.Tours[move.Tour2];

      foreach (int stop in segmentX1)
        route1.Stops.Remove(stop);
      route1.Stops.AddRange(segmentX2);

      foreach (int stop in segmentX2)
        route2.Stops.Remove(stop);
      route2.Stops.AddRange(segmentX1);
    }

    protected override void PerformMove() {
      PotvinTwoOptStarMove move = TwoOptStarMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      newSolution.Repair();
      VRPToursParameter.ActualValue = newSolution;
    }
  }
}
