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
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("StopInsertionInfo", "")]
  [StorableType("e4e2657a-0af9-4f1c-b396-887ad5b6f459")]
  public class StopInsertionInfo : Item {
    [Storable] public int Start { get; private set; }
    [Storable] public int End { get; private set; }


    [StorableConstructor]
    protected StopInsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected StopInsertionInfo(StopInsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      Start = original.Start;
      End = original.End;
    }
    public StopInsertionInfo(int start, int end)
      : base() {
      Start = start;
      End = end;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StopInsertionInfo(this, cloner);
    }
  }

  [Item("TourInsertionInfo", "")]
  [StorableType("25a61cca-120b-43e2-845a-d290352ca1e9")]
  public class TourInsertionInfo : Item {
    [Storable] public double Penalty { get; set; }
    [Storable] public double Quality { get; set; }
    [Storable] public int Vehicle { get; set; }
    [Storable] private List<StopInsertionInfo> stopInsertionInfos;

    [StorableConstructor]
    protected TourInsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected TourInsertionInfo(TourInsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      Penalty = original.Penalty;
      Quality = original.Quality;
      Vehicle = original.Vehicle;
      stopInsertionInfos = original.stopInsertionInfos.Select(cloner.Clone).ToList();
    }
    public TourInsertionInfo(int vehicle)
      : base() {
      stopInsertionInfos = new List<StopInsertionInfo>();
      Vehicle = vehicle;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TourInsertionInfo(this, cloner);
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

  [Item("InsertionInfo", "")]
  [StorableType("ba569eb3-df25-4f73-bfa0-246b38c1d520")]
  public class InsertionInfo : Item {
    [Storable] private List<TourInsertionInfo> tourInsertionInfos;

    [StorableConstructor]
    protected InsertionInfo(StorableConstructorFlag _) : base(_) { }
    protected InsertionInfo(InsertionInfo original, Cloner cloner)
      : base(original, cloner) {
      tourInsertionInfos = original.tourInsertionInfos.Select(cloner.Clone).ToList();
    }
    public InsertionInfo()
      : base() {
      tourInsertionInfos = new List<TourInsertionInfo>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InsertionInfo(this, cloner);
    }

    public void AddTourInsertionInfo(TourInsertionInfo info) {
      tourInsertionInfos.Add(info);
    }

    public TourInsertionInfo GetTourInsertionInfo(int tour) {
      return tourInsertionInfos[tour];
    }
  }

  [Item("VRPEvaluation", "")]
  [StorableType("0c4dfa78-8e41-4558-b2dd-4a22954c35ba")]
  public class VRPEvaluation : EvaluationResult, ISingleObjectiveEvaluationResult {
    // TODO: Would be nice to collect these, into individual results in a run
    [Storable] public double Quality { get; set; }
    [Storable] public double Distance { get; set; }
    [Storable] public int VehicleUtilization { get; set; }
    [Storable] public InsertionInfo InsertionInfo { get; set; }
    [Storable] public double Penalty { get; set; }
    [Storable] public bool IsFeasible { get; set; }

    [StorableConstructor]
    protected VRPEvaluation(StorableConstructorFlag _) : base(_) { }
    protected VRPEvaluation(VRPEvaluation original, Cloner cloner)
      : base(original, cloner) {
      Quality = original.Quality;
      Distance = original.Distance;
      VehicleUtilization = original.VehicleUtilization;
      InsertionInfo = cloner.Clone(original.InsertionInfo);
      Penalty = original.Penalty;
      IsFeasible = original.IsFeasible;
    }
    public VRPEvaluation() : base() {
      InsertionInfo = new InsertionInfo();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPEvaluation(this, cloner);
    }
  }
}
