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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General.Crossovers {
  [Item("RandomParentCloneCrossover", "An operator which randomly chooses one parent and returns a clone.")]
  [StorableClass]
  public sealed class RandomParentCloneCrossover : VRPCrossover, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private RandomParentCloneCrossover(bool deserializing) : base(deserializing) { }
    private RandomParentCloneCrossover(RandomParentCloneCrossover original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomParentCloneCrossover(this, cloner);
    }


    public RandomParentCloneCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

      Parameters.Remove("Coordinates");
      Parameters.Remove("DistanceMatrix");
      Parameters.Remove("UseDistanceMatrix");
      Parameters.Remove("Vehicles");
      Parameters.Remove("Capacity");
      Parameters.Remove("Demand");
      Parameters.Remove("ReadyTime");
      Parameters.Remove("DueTime");
      Parameters.Remove("ServiceTime");
    }

    public override IOperation Apply() {
      if (RandomParameter.ActualValue.Next() < 0.5)
        ChildParameter.ActualValue = ParentsParameter.ActualValue[0].Clone() as IVRPEncoding;
      else
        ChildParameter.ActualValue = ParentsParameter.ActualValue[1].Clone() as IVRPEncoding;

      return base.Apply();
    }
  }
}
