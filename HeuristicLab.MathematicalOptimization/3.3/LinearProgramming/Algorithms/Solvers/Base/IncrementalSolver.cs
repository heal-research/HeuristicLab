using System;
using System.Linq;
using System.Threading;
using Google.OrTools.LinearSolver;
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
    protected readonly IValueParameter<BoolValue> incrementalityParam;

    [Storable]
    protected readonly IValueParameter<TimeSpanValue> qualityUpdateIntervalParam;

    private IndexedDataRow<double> bpcRow;

    private IndexedDataRow<double> qpcRow;

    [Storable]
    private IndexedDataTable<double> qualityPerClock;

    public IncrementalSolver() {
      Parameters.Add(incrementalityParam = new ValueParameter<BoolValue>(nameof(Incrementality),
        "Advanced usage: incrementality from one solve to the next.",
        new BoolValue(MPSolverParameters.kDefaultIncrementality == MPSolverParameters.INCREMENTALITY_ON)));
      Parameters.Add(qualityUpdateIntervalParam =
        new ValueParameter<TimeSpanValue>(nameof(QualityUpdateInterval),
          "Time interval before solver is paused, resuls are retrieved and solver is resumed.",
          new TimeSpanValue(new TimeSpan(0, 0, 10))));

      incrementalityParam.Value.ValueChanged += (sender, args) => {
        if (((BoolValue)sender).Value) {
          qualityUpdateIntervalParam.Value = new TimeSpanValue(qualityUpdateIntervalParam.Value.Value);
        } else {
          qualityUpdateIntervalParam.Value = (TimeSpanValue)qualityUpdateIntervalParam.Value.AsReadOnly();
        }
      };
    }

    protected IncrementalSolver(IncrementalSolver original, Cloner cloner)
        : base(original, cloner) {
      programmingTypeParam = cloner.Clone(original.programmingTypeParam);
      qualityUpdateIntervalParam = cloner.Clone(original.qualityUpdateIntervalParam);
      incrementalityParam = cloner.Clone(original.incrementalityParam);
      if (original.qualityPerClock != null)
        qualityPerClock = cloner.Clone(original.qualityPerClock);
    }

    public bool Incrementality {
      get => incrementalityParam.Value.Value;
      set => incrementalityParam.Value.Value = value;
    }

    public TimeSpan QualityUpdateInterval {
      get => qualityUpdateIntervalParam.Value.Value;
      set => qualityUpdateIntervalParam.Value.Value = value;
    }

    public override void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken) {
      if (!Incrementality) {
        base.Solve(algorithm, cancellationToken);
        return;
      }

      var timeLimit = algorithm.TimeLimit;
      var unlimitedRuntime = timeLimit == TimeSpan.Zero;

      if (!unlimitedRuntime) {
        var wallTime = ((TimeSpanValue)algorithm.Results.SingleOrDefault(r => r.Name == "Wall Time")?.Value)?.Value;
        if (wallTime.HasValue) {
          timeLimit -= wallTime.Value;
        }
      }

      var iterations = (long)timeLimit.TotalMilliseconds / (long)QualityUpdateInterval.TotalMilliseconds;
      var remaining = timeLimit - TimeSpan.FromMilliseconds(iterations * QualityUpdateInterval.TotalMilliseconds);
      var validResultStatuses = new[] { ResultStatus.NOT_SOLVED, ResultStatus.FEASIBLE };

      while (unlimitedRuntime || iterations > 0) {
        base.Solve(algorithm, QualityUpdateInterval, true);
        UpdateQuality(algorithm);

        var resultStatus = ((EnumValue<ResultStatus>)algorithm.Results["Result Status"].Value).Value;
        if (!validResultStatuses.Contains(resultStatus) || cancellationToken.IsCancellationRequested)
          return;

        if (!unlimitedRuntime)
          iterations--;
      }

      if (remaining > TimeSpan.Zero) {
        base.Solve(algorithm, remaining, true);
        UpdateQuality(algorithm);
      }
    }

    private void UpdateQuality(LinearProgrammingAlgorithm algorithm) {
      if (!algorithm.Results.Exists(r => r.Name == "QualityPerClock")) {
        qualityPerClock = new IndexedDataTable<double>("Quality per Clock");
        qpcRow = new IndexedDataRow<double>("First-hit Graph Objective");
        bpcRow = new IndexedDataRow<double>("First-hit Graph Bound");
        algorithm.Results.AddOrUpdateResult("QualityPerClock", qualityPerClock);
      }

      var resultStatus = ((EnumValue<ResultStatus>)algorithm.Results["Result Status"].Value).Value;

      if (new[] { ResultStatus.ABNORMAL, ResultStatus.NOT_SOLVED, ResultStatus.UNBOUNDED }.Contains(resultStatus))
        return;

      var objective = ((DoubleValue)algorithm.Results["Best Objective Value"].Value).Value;
      var bound = ((DoubleValue)algorithm.Results["Best Objective Bound"].Value).Value;
      var time = algorithm.ExecutionTime.TotalSeconds;

      if (!qpcRow.Values.Any()) {
        if (!double.IsInfinity(objective) && !double.IsNaN(objective)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          qpcRow.Values.Add(Tuple.Create(time, objective));
          qualityPerClock.Rows.Add(qpcRow);
          algorithm.Results.AddOrUpdateResult("Best Solution Found At", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      } else {
        var previousBest = qpcRow.Values.Last().Item2;
        qpcRow.Values[qpcRow.Values.Count - 1] = Tuple.Create(time, objective);
        if (!objective.IsAlmost(previousBest)) {
          qpcRow.Values.Add(Tuple.Create(time, objective));
          algorithm.Results.AddOrUpdateResult("Best Solution Found At", new TimeSpanValue(TimeSpan.FromSeconds(time)));
        }
      }

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