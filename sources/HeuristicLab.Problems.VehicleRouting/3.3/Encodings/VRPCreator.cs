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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [StorableClass]
  public abstract class VRPCreator : VRPOperator, IVRPCreator {
    public override bool CanChangeName {
      get { return false; }
    }

    #region IVRPCreator Members
    public ILookupParameter<IVRPEncoding> VRPSolutionParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPSolution"]; }
    }
    
      public IValueLookupParameter<IntValue> CitiesParameter {
	      get { return (IValueLookupParameter<IntValue>)Parameters["Cities"]; }
	    }

    public VRPCreator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPSolution", "The new VRP solution."));
    }

    #endregion
  }
}
