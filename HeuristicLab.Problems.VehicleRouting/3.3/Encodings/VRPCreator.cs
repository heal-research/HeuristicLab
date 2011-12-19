#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("VRPCreator", "A VRP creator.")]
  [StorableClass]
  public abstract class VRPCreator : VRPOperator, IVRPCreator {
    public override bool CanChangeName {
      get { return false; }
    }

    #region IVRPCreator Members
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    
    [StorableConstructor]
    protected VRPCreator(bool deserializing) : base(deserializing) { }
    protected VRPCreator(VRPCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public VRPCreator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The new VRP tours."));
    }
    #endregion
  }
}
