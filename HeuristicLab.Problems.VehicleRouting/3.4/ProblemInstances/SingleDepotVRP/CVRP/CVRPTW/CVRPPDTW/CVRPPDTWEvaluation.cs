#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  public class CVRPPDTWInsertionInfo : CVRPTWInsertionInfo {
    private List<int> visited;

    public List<int> Visited {
      get { return visited; }
    }

    private double arrivalSpareCapacity;

    public double ArrivalSpareCapacity {
      get { return arrivalSpareCapacity; }
    }

    public CVRPPDTWInsertionInfo(int start, int end, double spareCapacity, double tourStartTime,
      double arrivalTime, double leaveTime, double spareTime, double waitingTime, List<int> visited, double arrivalSpareCapacity)
      : base(start, end, spareCapacity, tourStartTime, arrivalTime, leaveTime, spareTime, waitingTime) {
      this.visited = visited;
      this.arrivalSpareCapacity = arrivalSpareCapacity;
    }
  }

  public class CVRPPDTWEvaluation : CVRPTWEvaluation {
    public int PickupViolations { get; set; }
  }
}
