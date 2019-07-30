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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPManipulator", "Manipulates a VRP solution.")]
  [StorableType("6A7616B7-0BA2-45DA-B89F-C4ECE93080BD")]
  public abstract class VRPManipulator : VRPOperator, IVRPManipulator {
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    [StorableConstructor]
    protected VRPManipulator(StorableConstructorFlag _) : base(_) { }

    public VRPManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours to be manipulated."));
    }

    protected VRPManipulator(VRPManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
