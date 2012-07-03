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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuManipulator", "An operator which manipulates a VRP representation.")]
  [StorableClass]
  public abstract class ZhuManipulator : VRPManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    [StorableConstructor]
    protected ZhuManipulator(bool deserializing) : base(deserializing) { }
    protected ZhuManipulator(ZhuManipulator original, Cloner cloner)
      : base(original, cloner) {
    }


    public ZhuManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected abstract void Manipulate(IRandom random, ZhuEncoding individual);

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is ZhuEncoding)) {
        VRPToursParameter.ActualValue = ZhuEncoding.ConvertFrom(solution,
            Cities,
            DueTimeParameter.ActualValue,
            ServiceTimeParameter.ActualValue,
            ReadyTimeParameter.ActualValue,
            DemandParameter.ActualValue,
            CapacityParameter.ActualValue,
            CoordinatesParameter.ActualValue,
            DistanceMatrixParameter,
            UseDistanceMatrixParameter.ActualValue);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as ZhuEncoding);

      return base.Apply();
    }
  }
}
