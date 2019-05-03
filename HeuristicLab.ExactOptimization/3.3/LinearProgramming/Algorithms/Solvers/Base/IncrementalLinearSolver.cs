#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [StorableType("8730ECDD-8F38-47C2-B0D9-2B1F38FC0A27")]
  public class IncrementalLinearSolver : LinearSolver, IIncrementalLinearSolver {

    [Storable]
    protected readonly IValueParameter<TimeSpanValue> qualityUpdateIntervalParam;

    private readonly Stopwatch stopwatch = new Stopwatch();

    [Storable]
    private TimeSpan executionTime = TimeSpan.Zero;

    [Storable]
    private IndexedDataTable<double> qualityPerClock;

    public IncrementalLinearSolver() {
      Parameters.Add(qualityUpdateIntervalParam =
        new ValueParameter<TimeSpanValue>(nameof(QualityUpdateInterval),
          "Time interval before solver is paused, results are retrieved and solver is resumed. " +
          "Set to zero for no intermediate results and faster solving.", new TimeSpanValue(new TimeSpan(0, 0, 10))));
      problemTypeParam.Value.ValueChanged += (sender, args) => {
        if (SupportsQualityUpdate) {
          if (!Parameters.Contains(qualityUpdateIntervalParam)) {
            Parameters.Add(qualityUpdateIntervalParam);
          }
        } else {
          Parameters.Remove(qualityUpdateIntervalParam);
        }
      };
    }

    [StorableConstructor]
    protected IncrementalLinearSolver(StorableConstructorFlag _) : base(_) { }

    protected IncrementalLinearSolver(IncrementalLinearSolver original, Cloner cloner)
      : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
      qualityUpdateIntervalParam = cloner.Clone(original.qualityUpdateIntervalParam);
      if (original.qualityPerClock != null)
        qualityPerClock = cloner.Clone(original.qualityPerClock);
    }

    public TimeSpan QualityUpdateInterval {
      get => qualityUpdateIntervalParam.Value.Value;
      set => qualityUpdateIntervalParam.Value.Value = value;
    }

    public IValueParameter<TimeSpanValue> QualityUpdateIntervalParameter => qualityUpdateIntervalParam;

    public virtual bool SupportsQualityUpdate => true;

    protected virtual TimeSpan IntermediateTimeLimit => QualityUpdateInterval;

    public override void Reset() {
      base.Reset();
      executionTime = TimeSpan.Zero;
    }

    public override void Solve(ILinearProblemDefinition problemDefinition,
      ResultCollection results, CancellationToken cancellationToken) {
      if (!SupportsQualityUpdate || QualityUpdateInterval == TimeSpan.Zero) {
        base.Solve(problemDefinition, results, cancellationToken);
        return;
      }

      var timeLimit = TimeLimit;
      var unlimitedRuntime = timeLimit == TimeSpan.Zero;

      if (!unlimitedRuntime) {
        timeLimit -= executionTime;
      }

      var iterations = (long)timeLimit.TotalMilliseconds / (long)QualityUpdateInterval.TotalMilliseconds;
      var remaining = timeLimit - TimeSpan.FromMilliseconds(iterations * QualityUpdateInterval.TotalMilliseconds);
      var validResultStatuses = new[] { ResultStatus.NotSolved, ResultStatus.Feasible };

      while (unlimitedRuntime || iterations > 0) {
        if (cancellationToken.IsCancellationRequested)
          return;

        stopwatch.Restart();
        Solve(problemDefinition, results, IntermediateTimeLimit);
        stopwatch.Stop();
        executionTime += stopwatch.Elapsed;
        UpdateQuality(results, executionTime);

        var resultStatus = ((EnumValue<ResultStatus>)results["ResultStatus"].Value).Value;
        if (!validResultStatuses.Contains(resultStatus))
          return;

        if (!unlimitedRuntime)
          iterations--;
      }

      if (remaining > TimeSpan.Zero) {
        Solve(problemDefinition, results, remaining);
        UpdateQuality(results, executionTime);
      }
    }

    private void UpdateQuality(ResultCollection results, TimeSpan executionTime) {
      IndexedDataRow<double> qpcRow;
      IndexedDataRow<double> bpcRow;

      if (!results.Exists(r => r.Name == "QualityPerClock")) {
        qualityPerClock = new IndexedDataTable<double>("Quality per Clock");
        qpcRow = new IndexedDataRow<double>("Objective Value");
        bpcRow = new IndexedDataRow<double>("Bound");
        qualityPerClock.Rows.Add(qpcRow);
        qualityPerClock.Rows.Add(bpcRow);
        results.AddOrUpdateResult("QualityPerClock", qualityPerClock);
      } else {
        qpcRow = qualityPerClock.Rows["Objective Value"];
        bpcRow = qualityPerClock.Rows["Bound"];
      }

      var resultStatus = ((EnumValue<ResultStatus>)results["ResultStatus"].Value).Value;

      if (new[] { ResultStatus.Abnormal, ResultStatus.NotSolved, ResultStatus.Unbounded }.Contains(resultStatus))
        return;

      var objective = ((DoubleValue)results["BestObjectiveValue"].Value).Value;
      var bound = solver.IsMip() ? ((DoubleValue)results["BestObjectiveBound"].Value).Value : double.NaN;
      var time = executionTime.TotalSeconds;

      if (!qpcRow.Values.Any()) {
        if (!double.IsInfinity(objective) && !double.IsNaN(objective)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          qpcRow.Values.Add(Tuple.Create(time, objective));
          results.AddOrUpdateResult("BestObjectiveValueFoundAt", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      } else {
        var previousBest = qpcRow.Values.Last().Item2;
        qpcRow.Values[qpcRow.Values.Count - 1] = Tuple.Create(time, objective);
        if (!objective.IsAlmost(previousBest)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          results.AddOrUpdateResult("BestObjectiveValueFoundAt", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      }

      if (!solver.IsMip())
        return;

      if (!bpcRow.Values.Any()) {
        if (!double.IsInfinity(bound) && !double.IsNaN(bound)) {
          bpcRow.Values.Add(Tuple.Create(time, bound));
          bpcRow.Values.Add(Tuple.Create(time, bound));
        }
      } else {
        var previousBest = bpcRow.Values.Last().Item2;
        bpcRow.Values[bpcRow.Values.Count - 1] = Tuple.Create(time, bound);
        if (!bound.IsAlmost(previousBest)) {
          bpcRow.Values.Add(Tuple.Create(time, bound));
        }
      }
    }
  }
}
