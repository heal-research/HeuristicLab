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
  [Item("PotvinCustomerRelocationSingleMoveGenerator", "Generates a single customer relocation move from a given VRP encoding.")]
  [StorableClass]
  public sealed class PotvinCustomerRelocationSingleMoveGenerator : PotvinCustomerRelocationMoveGenerator,
    ISingleMoveGenerator {
    #region IMultiVRPMoveOperator Members

    public override ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["PotvinCustomerRelocationMove"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationSingleMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinCustomerRelocationSingleMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinCustomerRelocationSingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    private PotvinCustomerRelocationSingleMoveGenerator(PotvinCustomerRelocationSingleMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static PotvinCustomerRelocationMove Apply(PotvinEncoding individual, IVRPProblemInstance problemInstance, IRandom rand) {
      int city = 1 + rand.Next(individual.Cities);
      Tour oldTour = individual.Tours.Find(t => t.Stops.Contains(city));
      int oldTourIndex = individual.Tours.IndexOf(oldTour);

      int max = individual.Tours.Count;
      if (individual.Tours.Count < problemInstance.Vehicles.Value)
        max = max - 1;

      int newTourIndex = rand.Next(max);
      if (newTourIndex >= oldTourIndex)
        newTourIndex++;

      return new PotvinCustomerRelocationMove(city, oldTourIndex, newTourIndex, individual);
    }

    protected override PotvinCustomerRelocationMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinCustomerRelocationMove> result = new List<PotvinCustomerRelocationMove>();

      PotvinCustomerRelocationMove move = Apply(individual, ProblemInstance, RandomParameter.ActualValue);
      if (move != null)
        result.Add(move);

      return result.ToArray();
    }
  }
}
