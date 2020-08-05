#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General.Crossovers {
  [Item("RandomParentCloneCrossover", "An operator which randomly chooses one parent and returns a clone.")]
  [StorableType("418EA2DE-C098-4A88-82B9-CB68732DB2AC")]
  public sealed class RandomParentCloneCrossover : VRPOperator, IStochasticOperator, IGeneralVRPOperator, IVRPCrossover {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<ItemArray<IVRPEncodedSolution>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncodedSolution>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncodedSolution> ChildParameter {
      get { return (ILookupParameter<IVRPEncodedSolution>)Parameters["Child"]; }
    }

    [StorableConstructor]
    private RandomParentCloneCrossover(StorableConstructorFlag _) : base(_) { }

    public RandomParentCloneCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncodedSolution>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPTours";
      Parameters.Add(new LookupParameter<IVRPEncodedSolution>("Child", "The child permutation resulting from the crossover."));
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
        ChildParameter.ActualValue = ParentsParameter.ActualValue[0].Clone() as IVRPEncodedSolution;
      else
        ChildParameter.ActualValue = ParentsParameter.ActualValue[1].Clone() as IVRPEncodedSolution;

      return base.InstrumentedApply();
    }
  }
}
