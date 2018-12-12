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
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  [StorableClass]
  public class IncrementalSolver : Solver, IIncrementalSolver {

    [Storable]
    protected readonly IValueParameter<TimeSpanValue> qualityUpdateIntervalParam;

    private IndexedDataRow<double> bpcRow;

    private IndexedDataRow<double> qpcRow;

    [Storable]
    private IndexedDataTable<double> qualityPerClock;

    [StorableConstructor]
    protected IncrementalSolver(bool deserializing)
      : base(deserializing) {
    }

    public IncrementalSolver() {
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

    protected IncrementalSolver(IncrementalSolver original, Cloner cloner)
        : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
      qualityUpdateIntervalParam = cloner.Clone(original.qualityUpdateIntervalParam);
      if (original.qualityPerClock != null)
        qualityPerClock = cloner.Clone(original.qualityPerClock);
    }

    public virtual bool SupportsQualityUpdate => true;

    public TimeSpan QualityUpdateInterval {
      get => qualityUpdateIntervalParam.Value.Value;
      set => qualityUpdateIntervalParam.Value.Value = value;
    }

    protected virtual TimeSpan TimeLimit => QualityUpdateInterval;

    public override void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken) {
      if (!SupportsQualityUpdate || QualityUpdateInterval == TimeSpan.Zero) {
        base.Solve(algorithm, cancellationToken);
        return;
      }

      var timeLimit = algorithm.TimeLimit;
      var unlimitedRuntime = timeLimit == TimeSpan.Zero;

      if (!unlimitedRuntime) {
        timeLimit -= algorithm.ExecutionTime;
      }

      var iterations = (long)timeLimit.TotalMilliseconds / (long)QualityUpdateInterval.TotalMilliseconds;
      var remaining = timeLimit - TimeSpan.FromMilliseconds(iterations * QualityUpdateInterval.TotalMilliseconds);
      var validResultStatuses = new[] { ResultStatus.NotSolved, ResultStatus.Feasible };

      while (unlimitedRuntime || iterations > 0) {
        if (cancellationToken.IsCancellationRequested)
          return;

        base.Solve(algorithm, TimeLimit);
        UpdateQuality(algorithm);

        var resultStatus = ((EnumValue<ResultStatus>)algorithm.Results[nameof(solver.ResultStatus)].Value).Value;
        if (!validResultStatuses.Contains(resultStatus))
          return;

        if (!unlimitedRuntime)
          iterations--;
      }

      if (remaining > TimeSpan.Zero) {
        base.Solve(algorithm, remaining);
        UpdateQuality(algorithm);
      }
    }

    private void UpdateQuality(LinearProgrammingAlgorithm algorithm) {
      if (!algorithm.Results.Exists(r => r.Name == "QualityPerClock")) {
        qualityPerClock = new IndexedDataTable<double>("Quality per Clock");
        qpcRow = new IndexedDataRow<double>("Objective Value");
        bpcRow = new IndexedDataRow<double>("Bound");
        algorithm.Results.AddOrUpdateResult("QualityPerClock", qualityPerClock);
      }

      var resultStatus = ((EnumValue<ResultStatus>)algorithm.Results[nameof(solver.ResultStatus)].Value).Value;

      if (new[] { ResultStatus.Abnormal, ResultStatus.NotSolved, ResultStatus.Unbounded }.Contains(resultStatus))
        return;

      var objective = ((DoubleValue)algorithm.Results[$"Best{nameof(solver.ObjectiveValue)}"].Value).Value;
      var bound = solver.IsMip ? ((DoubleValue)algorithm.Results[$"Best{nameof(solver.ObjectiveBound)}"].Value).Value : double.NaN;
      var time = algorithm.ExecutionTime.TotalSeconds;

      if (!qpcRow.Values.Any()) {
        if (!double.IsInfinity(objective) && !double.IsNaN(objective)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          qpcRow.Values.Add(Tuple.Create(time, objective));
          qualityPerClock.Rows.Add(qpcRow);
          algorithm.Results.AddOrUpdateResult($"Best{nameof(solver.ObjectiveValue)}FoundAt", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      } else {
        var previousBest = qpcRow.Values.Last().Item2;
        qpcRow.Values[qpcRow.Values.Count - 1] = Tuple.Create(time, objective);
        if (!objective.IsAlmost(previousBest)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          algorithm.Results.AddOrUpdateResult($"Best{nameof(solver.ObjectiveValue)}FoundAt", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      }

      if (!solver.IsMip)
        return;

      if (!bpcRow.Values.Any()) {
        if (!double.IsInfinity(bound) && !double.IsNaN(bound)) {
          bpcRow.Values.Add(Tuple.Create(time, bound));
          bpcRow.Values.Add(Tuple.Create(time, bound));
          qualityPerClock.Rows.Add(bpcRow);
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
