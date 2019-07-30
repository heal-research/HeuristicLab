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
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("VRPMoveGenerator", "Generates moves for a VRP solution.")]
  [StorableType("B76BD2D8-2D37-4006-86AF-64A1D28DC26D")]
  public abstract class
    VRPMoveGenerator : VRPMoveOperator, IMoveGenerator {
    [StorableConstructor]
    protected VRPMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public VRPMoveGenerator()
      : base() {
    }

    protected VRPMoveGenerator(VRPMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
