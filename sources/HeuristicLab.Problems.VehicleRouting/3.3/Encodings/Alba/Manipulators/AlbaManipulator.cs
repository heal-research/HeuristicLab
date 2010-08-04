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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [StorableClass]
  public abstract class AlbaManipulator : VRPManipulator {
    public ILookupParameter<IntValue> VehiclesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Vehicles"]; }
    }

    public AlbaManipulator()
      : base() {
        Parameters.Add(new LookupParameter<IntValue>("Vehicles", "The vehicles count."));
    }
    
    protected virtual void Manipulate() {
    }
    
    public override IOperation Apply() {
      IVRPEncoding solution = VRPSolutionParameter.ActualValue;
      if (!(solution is AlbaEncoding)) {
        VRPSolutionParameter.ActualValue = AlbaEncoding.ConvertFrom(solution, VehiclesParameter.ActualValue.Value);
      }
      
      Manipulate();

      return base.Apply();
    }
  }
}
