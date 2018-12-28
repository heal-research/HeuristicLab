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
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPMoveOperator", "A VRP move operator.")]
  [StorableType("B2A37CCE-3EF6-4648-8A9B-2BCBD4EC93F1")]
  public abstract class VRPMoveOperator : VRPOperator, IVRPMoveOperator {
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public abstract ILookupParameter VRPMoveParameter { get; }

    [StorableConstructor]
    protected VRPMoveOperator(StorableConstructorFlag _) : base(_) { }

    public VRPMoveOperator() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
    }

    protected VRPMoveOperator(VRPMoveOperator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
