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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinMoveGenerator", "Generates Potvin VRP moves.")]
  [StorableType("A9832740-B65F-43CA-8B14-A0C19F4B977D")]
  public abstract class PotvinMoveGenerator : VRPMoveGenerator, IPotvinOperator {
    [StorableConstructor]
    protected PotvinMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinMoveGenerator()
      : base() {
    }

    protected PotvinMoveGenerator(PotvinMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PotvinEncoding)) {
        VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(solution, ProblemInstance);
      }

      return base.InstrumentedApply();
    }
  }
}
