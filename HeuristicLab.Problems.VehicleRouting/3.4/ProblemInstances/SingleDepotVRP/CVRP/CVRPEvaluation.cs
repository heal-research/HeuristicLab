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


using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPInsertionInfo", "")]
  [StorableType("f951554e-dbe1-4dfb-a6b2-b477b9f79fcd")]
  public class CVRPInsertionInfo : StopInsertionInfo {
    [Storable] public double SpareCapacity { get; private set; }

    [StorableConstructor]
    protected CVRPInsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected CVRPInsertionInfo(CVRPInsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      SpareCapacity = original.SpareCapacity;
    }
    public CVRPInsertionInfo(int start, int end, double spareCapacity)
      : base(start, end) {
      SpareCapacity = spareCapacity;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPInsertionInfo(this, cloner);
    }
  }

  [Item("CVRPEvaluation", "")]
  [StorableType("e3d4c547-3028-4c95-8ea8-1fe234d54ed3")]
  public class CVRPEvaluation : VRPEvaluation {
    [Storable] public double Overload { get; set; }

    [StorableConstructor]
    protected CVRPEvaluation(StorableConstructorFlag _) : base(_) { }
    protected CVRPEvaluation(CVRPEvaluation original, Cloner cloner)
      : base(original, cloner) {
      Overload = original.Overload;
    }
    public CVRPEvaluation() { }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPEvaluation(this, cloner);
    }
  }
}
