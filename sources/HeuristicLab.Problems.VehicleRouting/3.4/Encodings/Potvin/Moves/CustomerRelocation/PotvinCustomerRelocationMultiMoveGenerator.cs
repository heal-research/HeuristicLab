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
using HeuristicLab.Parameters;
using System.Collections.Generic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMultiMoveGenerator", "Generates customer relocation moves from a given VRP encoding.")]
  [StorableClass]
  public sealed class PotvinCustomerRelocationMultiMoveGenerator : PotvinCustomerRelocationMoveGenerator, IMultiMoveGenerator, IMultiVRPMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationMultiMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinCustomerRelocationMultiMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinCustomerRelocationMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    private PotvinCustomerRelocationMultiMoveGenerator(PotvinCustomerRelocationMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinCustomerRelocationMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinCustomerRelocationMove> result = new List<PotvinCustomerRelocationMove>();

      for (int i = 0; i < SampleSizeParameter.ActualValue.Value; i++) {
        result.Add(PotvinCustomerRelocationSingleMoveGenerator.Apply(individual, ProblemInstance, RandomParameter.ActualValue));
      }

      return result.ToArray();
    }
  }
}
