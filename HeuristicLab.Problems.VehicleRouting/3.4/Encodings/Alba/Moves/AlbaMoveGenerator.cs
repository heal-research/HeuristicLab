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
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaMoveGenerator", "Generates Alba VRP moves.")]
  [StorableType("97A1A0D2-BC7F-4F68-A9FD-2CC29F487AF2")]
  public abstract class AlbaMoveGenerator : VRPMoveGenerator, IAlbaOperator {
    [StorableConstructor]
    protected AlbaMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public AlbaMoveGenerator()
      : base() {
    }

    protected AlbaMoveGenerator(AlbaMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is AlbaEncoding)) {
        VRPToursParameter.ActualValue = AlbaEncoding.ConvertFrom(solution, ProblemInstance);
      }

      return base.InstrumentedApply();
    }
  }
}
