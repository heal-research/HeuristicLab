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
  [Item("CVRPTWInsertionInfo", "")]
  [StorableType("487635b5-2682-42a2-9ec2-1a210e6ce88c")]
  public class CVRPTWInsertionInfo : CVRPInsertionInfo {

    [Storable] public double TourStartTime { get; private set; }
    [Storable] public double ArrivalTime { get; private set; }
    [Storable] public double LeaveTime { get; private set; }
    [Storable] public double SpareTime { get; private set; }
    [Storable] public double WaitingTime { get; private set; }


    [StorableConstructor]
    protected CVRPTWInsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected CVRPTWInsertionInfo(CVRPTWInsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      TourStartTime = original.TourStartTime;
      ArrivalTime = original.ArrivalTime;
      LeaveTime = original.LeaveTime;
      SpareTime = original.SpareTime;
      WaitingTime = original.WaitingTime;
    }
    public CVRPTWInsertionInfo(int start, int end, double spareCapacity, double tourStartTime, double arrivalTime, double leaveTime, double spareTime, double waitingTime)
      : base(start, end, spareCapacity) {
      TourStartTime = tourStartTime;
      ArrivalTime = arrivalTime;
      LeaveTime = leaveTime;
      SpareTime = spareTime;
      WaitingTime = waitingTime;
    }
  }

  [Item("CVRPTWEvaluation", "")]
  [StorableType("b3b31244-5ac8-4007-9ae6-249e2a7d5321")]
  public class CVRPTWEvaluation : CVRPEvaluation {
    [Storable] public double Tardiness { get; set; }
    [Storable] public double TravelTime { get; set; }

    [StorableConstructor]
    protected CVRPTWEvaluation(StorableConstructorFlag _) : base(_) { }
    protected CVRPTWEvaluation(CVRPTWEvaluation original, Cloner cloner)
      : base(original, cloner) {
      Tardiness = original.Tardiness;
      TravelTime = original.TravelTime;
    }
    public CVRPTWEvaluation() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPTWEvaluation(this, cloner);
    }
  }
}
