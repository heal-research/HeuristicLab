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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRManipulator", "A VRP manipulation operation.")]
  [StorableClass]
  public abstract class GVRManipulator : VRPManipulator, IStochasticOperator, IGVROperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected GVRManipulator(bool deserializing) : base(deserializing) { }

    public GVRManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected GVRManipulator(GVRManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract void Manipulate(IRandom random, GVREncoding individual);
   
    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is GVREncoding)) {
        VRPToursParameter.ActualValue = GVREncoding.ConvertFrom(solution, ProblemInstance);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as GVREncoding);

      return base.Apply();
    }
  }
}
