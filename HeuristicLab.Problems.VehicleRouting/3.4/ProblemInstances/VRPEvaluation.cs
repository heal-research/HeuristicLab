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

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  public class StopInsertionInfo {
    private int start;

    public int Start {
      get { return start; }
    }

    private int end;

    public int End {
      get { return end; }
    }

    public StopInsertionInfo(int start, int end)
      : base() {
      this.start = start;
      this.end = end;
    }
  }

  public class TourInsertionInfo {
    public double Penalty { get; set; }
    public double Quality { get; set; }

    public int Vehicle { get; set; }

    private List<StopInsertionInfo> stopInsertionInfos;

    public TourInsertionInfo(int vehicle)
      : base() {
      stopInsertionInfos = new List<StopInsertionInfo>();
      Vehicle = vehicle;
    }

    public void AddStopInsertionInfo(StopInsertionInfo info) {
      stopInsertionInfos.Add(info);
    }

    public StopInsertionInfo GetStopInsertionInfo(int stop) {
      return stopInsertionInfos[stop];
    }

    public int GetStopCount() {
      return stopInsertionInfos.Count;
    }
  }

  public class InsertionInfo {
    private List<TourInsertionInfo> tourInsertionInfos;

    public InsertionInfo()
      : base() {
      tourInsertionInfos = new List<TourInsertionInfo>();
    }

    public void AddTourInsertionInfo(TourInsertionInfo info) {
      tourInsertionInfos.Add(info);
    }

    public TourInsertionInfo GetTourInsertionInfo(int tour) {
      return tourInsertionInfos[tour];
    }
  }

  public class VRPEvaluation {
    public double Quality { get; set; }
    public double Distance { get; set; }
    public int VehicleUtilization { get; set; }
    public InsertionInfo InsertionInfo { get; set; }

    public double Penalty { get; set; }

    public VRPEvaluation() {
      InsertionInfo = new InsertionInfo();
    }
  }
}
