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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [Item("VRPCreator", "A VRP crossover operation.")]
  [StorableClass]
  public abstract class VRPCrossover : VRPOperator, IVRPCrossover {
    #region IVRPCrossover Members

    public ILookupParameter<ItemArray<IVRPEncoding>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncoding> ChildParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["Child"]; }
    }

    #endregion

    [StorableConstructor]
    protected VRPCrossover(bool deserializing) : base(deserializing) { }
    protected VRPCrossover(VRPCrossover original, Cloner cloner)
      : base(original, cloner) {
    }
    public VRPCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPTours";
      Parameters.Add(new LookupParameter<IVRPEncoding>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "VRPTours";
    }
  }
}
