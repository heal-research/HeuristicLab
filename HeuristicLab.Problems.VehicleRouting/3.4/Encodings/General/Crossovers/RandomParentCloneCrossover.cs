#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General.Crossovers {
  [Item("RandomParentCloneCrossover", "An operator which randomly chooses one parent and returns a clone.")]
  [StorableClass]
  public sealed class RandomParentCloneCrossover : VRPOperator, IStochasticOperator, IGeneralVRPOperator, IVRPCrossover {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<ItemArray<IVRPEncoding>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncoding> ChildParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["Child"]; }
    }

    [StorableConstructor]
    private RandomParentCloneCrossover(bool deserializing) : base(deserializing) { }

    public RandomParentCloneCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPTours";
      Parameters.Add(new LookupParameter<IVRPEncoding>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "VRPTours";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomParentCloneCrossover(this, cloner);
    }

    private RandomParentCloneCrossover(RandomParentCloneCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      if (RandomParameter.ActualValue.Next() < 0.5)
        ChildParameter.ActualValue = ParentsParameter.ActualValue[0].Clone() as IVRPEncoding;
      else
        ChildParameter.ActualValue = ParentsParameter.ActualValue[1].Clone() as IVRPEncoding;

      return base.InstrumentedApply();
    }
  }
}
