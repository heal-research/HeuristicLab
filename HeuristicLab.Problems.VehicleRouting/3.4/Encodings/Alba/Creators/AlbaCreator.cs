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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCreator", "A VRP creator.")]
  [StorableType("95DC1B24-02C9-4AF0-8A00-21A47AFB9E39")]
  public abstract class AlbaCreator : VRPCreator, IAlbaOperator, IVRPCreator {
    public override bool CanChangeName {
      get { return false; }
    }

    [StorableConstructor]
    protected AlbaCreator(StorableConstructorFlag _) : base(_) { }

    public AlbaCreator()
      : base() {
    }

    protected AlbaCreator(AlbaCreator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      (VRPToursParameter.ActualValue as AlbaEncoding).Repair();

      return base.InstrumentedApply();
    }
  }
}
