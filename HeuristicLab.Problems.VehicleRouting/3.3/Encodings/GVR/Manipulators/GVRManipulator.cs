#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRManipulator", "A VRP manipulation operation.")]
  [StorableClass]
  public abstract class GVRManipulator : VRPManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected GVRManipulator(bool deserializing) : base(deserializing) { }
    protected GVRManipulator(GVRManipulator original, Cloner cloner) : base(original, cloner) { }
    public GVRManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

      //remove unused parameters
      Parameters.Remove("Coordinates");
      Parameters.Remove("UseDistanceMatrix");
      Parameters.Remove("ReadyTime");
      Parameters.Remove("DueTime");
      Parameters.Remove("ServiceTime");
    }

    protected abstract void Manipulate(IRandom random, GVREncoding individual);
   
    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is GVREncoding)) {
        VRPToursParameter.ActualValue = GVREncoding.ConvertFrom(solution, CapacityParameter.ActualValue, DemandParameter.ActualValue, 
          DistanceMatrixParameter);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as GVREncoding);

      return base.Apply();
    }
  }
}
