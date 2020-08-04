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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPPDTWInsertionInfo", "")]
  [StorableType("acb4ecf2-70a5-42ec-a2a5-68b11bd033e2")]
  public class CVRPPDTWInsertionInfo : CVRPTWInsertionInfo {
    [Storable] public List<int> Visited { get; private set; }
    [Storable] public double ArrivalSpareCapacity { get; private set; }

    [StorableConstructor]
    protected CVRPPDTWInsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected CVRPPDTWInsertionInfo(CVRPPDTWInsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      Visited = original.Visited.ToList();
      ArrivalSpareCapacity = original.ArrivalSpareCapacity;
    }
    public CVRPPDTWInsertionInfo(int start, int end, double spareCapacity, double tourStartTime,
      double arrivalTime, double leaveTime, double spareTime, double waitingTime, List<int> visited, double arrivalSpareCapacity)
      : base(start, end, spareCapacity, tourStartTime, arrivalTime, leaveTime, spareTime, waitingTime) {
      Visited = visited;
      ArrivalSpareCapacity = arrivalSpareCapacity;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPPDTWInsertionInfo(this, cloner);
    }
  }

  [Item("CVRPPDTWEvaluation", "")]
  [StorableType("877ae91a-708f-4b32-95f3-d790e0efc018")]
  public class CVRPPDTWEvaluation : CVRPTWEvaluation {
    [Storable] public int PickupViolations { get; set; }

    [StorableConstructor]
    protected CVRPPDTWEvaluation(StorableConstructorFlag _) : base(_) { }
    protected CVRPPDTWEvaluation(CVRPPDTWEvaluation original, Cloner cloner)
      : base(original, cloner) {
      PickupViolations = original.PickupViolations;
    }
    public CVRPPDTWEvaluation() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPPDTWEvaluation(this, cloner);
    }
  }
}
