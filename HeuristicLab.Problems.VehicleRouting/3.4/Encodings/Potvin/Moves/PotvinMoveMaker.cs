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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinMoveMaker", "Makes a Potvin VRP move.")]
  [StorableType("DF3016B2-53BE-4992-A00F-549E0546591D")]
  public abstract class PotvinMoveMaker : VRPMoveMaker, IPotvinOperator {
    [StorableConstructor]
    protected PotvinMoveMaker(StorableConstructorFlag _) : base(_) { }

    public PotvinMoveMaker()
      : base() {
    }

    protected PotvinMoveMaker(PotvinMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
