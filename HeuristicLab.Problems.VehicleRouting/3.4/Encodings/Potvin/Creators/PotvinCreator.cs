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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCreator", "A VRP creator.")]
  [StorableType("9007B8EE-61F5-45D7-A853-951E2435E308")]
  public abstract class PotvinCreator : VRPCreator, IPotvinOperator, IVRPCreator {
    public override bool CanChangeName {
      get { return false; }
    }

    [StorableConstructor]
    protected PotvinCreator(StorableConstructorFlag _) : base(_) { }

    public PotvinCreator()
      : base() {
    }

    protected PotvinCreator(PotvinCreator original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
