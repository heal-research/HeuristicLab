#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [Item("VRPOperator", "Represents a VRP operator.")]
  [StorableClass]
  public abstract class VRPOperator : InstrumentedOperator, IVRPOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public IVRPProblemInstance ProblemInstance {
      get { return ProblemInstanceParameter.ActualValue; }
    }

    [StorableConstructor]
    protected VRPOperator(bool deserializing) : base(deserializing) { }

    public VRPOperator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
    }

    protected VRPOperator(VRPOperator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
