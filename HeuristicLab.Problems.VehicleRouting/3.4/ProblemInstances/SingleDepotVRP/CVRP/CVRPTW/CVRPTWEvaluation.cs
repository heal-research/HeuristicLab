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


namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  public class CVRPTWInsertionInfo : CVRPInsertionInfo {
    private double tourStartTime;

    public double TourStartTime {
      get { return tourStartTime; }
    }

    private double arrivalTime;

    public double ArrivalTime {
      get { return arrivalTime; }
    }

    private double leaveTime;

    public double LeaveTime {
      get { return leaveTime; }
    }

    private double spareTime;

    public double SpareTime {
      get { return spareTime; }
    }

    private double waitingTime;

    public double WaitingTime {
      get { return waitingTime; }
    }

    public CVRPTWInsertionInfo(int start, int end, double spareCapacity, double tourStartTime, double arrivalTime, double leaveTime, double spareTime, double waitingTime)
      : base(start, end, spareCapacity) {
      this.tourStartTime = tourStartTime;
      this.arrivalTime = arrivalTime;
      this.leaveTime = leaveTime;
      this.spareTime = spareTime;
      this.waitingTime = waitingTime;
    }
  }

  public class CVRPTWEvaluation : CVRPEvaluation {
    public double Tardiness { get; set; }
    public double TravelTime { get; set; }
  }
}
