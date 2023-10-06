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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Dynamic.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("DED64F8F-7529-43A5-A13A-3705B31D12BA")]
  public abstract class SingleObjectiveDynamicBasicProblem<TEncoding, TSolution, TState> :
    SingleObjectiveBasicProblem<TEncoding>, ISingleObjectiveDynamicProblemDefinition
    where TEncoding : class, IEncoding
    where TSolution : class, IItem
    where TState : class {
    [Storable] private bool InitPending = false;

    #region ParameterNames
    public const string EpochClockParameterName = "EpochClock";
    public const string UpdatePolicyParameterName = "AutomaticProblemUpdatePolicy";
    public const string SeedParameterName = "EnvironmentSeed";
    public const string SetSeedRandomlyParameterName = "SetEnvironmentSeedRandomly";
    public const string TrackersParameterName = "Trackers";
    #endregion

    #region Parameters
    public IConstrainedValueParameter<IEpochClock> EpochClockParameter =>
      (IConstrainedValueParameter<IEpochClock>)Parameters[EpochClockParameterName];
    public IFixedValueParameter<EnumValue<ProblemUpdatePolicy>> UpdatePolicyParameter =>
      (IFixedValueParameter<EnumValue<ProblemUpdatePolicy>>)Parameters[UpdatePolicyParameterName];
    public IFixedValueParameter<IntValue> SeedParameter =>
      (IFixedValueParameter<IntValue>)Parameters[SeedParameterName];
    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName];
    public IFixedValueParameter<CheckedItemList<IDynamicProblemTracker<TState>>> TrackersParameter =>
      (IFixedValueParameter<CheckedItemList<IDynamicProblemTracker<TState>>>)Parameters[TrackersParameterName];
    #endregion

    #region Properties
    public int Seed { get { return SeedParameter.Value.Value; } set { SeedParameter.Value.Value = value; } }
    public bool SetSeedRandomly { get { return SetSeedRandomlyParameter.Value.Value; } set { SetSeedRandomlyParameter.Value.Value = value; } }
    public IEpochClock EpochClock => EpochClockParameter.Value;
    public ProblemUpdatePolicy ProblemUpdatePolicy => UpdatePolicyParameter.Value.Value;
    public IEnumerable<IDynamicProblemTracker<TState>> Trackers => TrackersParameter.Value.CheckedItems.Select(x => x.Value);
    #endregion

    #region Fields and Storbales
    private readonly ReaderWriterLock rwLock = new ReaderWriterLock();
    [Storable] private readonly RealTimeClock realTimeClock;
    [Storable] private readonly CountingClock evaluationCountingClock;
    [Storable] private readonly CountingClock generationCountingClock;
    [Storable] private bool Dirty { get; set; }
    [Storable] private long ClockVersion { get; set; }
    [Storable] private long ClockTime { get; set; }
    [Storable] protected IRandom EnvironmentRandom { get; set; }
    #endregion

    #region Constructors and cloning
    [StorableConstructor]
    protected SingleObjectiveDynamicBasicProblem(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
      if (!Operators.OfType<SingleObjectiveProblemStateAnalyzer>().Any())
        Operators.Add(new SingleObjectiveProblemStateAnalyzer() { Problem = this });
    }

    protected SingleObjectiveDynamicBasicProblem(SingleObjectiveDynamicBasicProblem<TEncoding, TSolution, TState> original, Cloner cloner)
      : base(original, cloner) {
      Dirty = original.Dirty;
      ClockVersion = original.ClockVersion;
      ClockTime = original.ClockTime;
      EnvironmentRandom = cloner.Clone(original.EnvironmentRandom);
      RegisterEventHandlers();
      InitPending = original.InitPending;
      realTimeClock = cloner.Clone(original.realTimeClock);
      evaluationCountingClock = cloner.Clone(original.evaluationCountingClock);
      generationCountingClock = cloner.Clone(original.generationCountingClock);
    }

    protected SingleObjectiveDynamicBasicProblem() {
      realTimeClock = new RealTimeClock() { Name = "Real-time Clock"};
      evaluationCountingClock = new CountingClock() { Name = "Evaluation Counting Clock" };
      generationCountingClock = new CountingClock() { Name = "Generation Counting Clock" };
      
      var clocks = new ItemSet<IEpochClock>(new IEpochClock[] { realTimeClock, evaluationCountingClock, generationCountingClock});
      Parameters.Add(new ConstrainedValueParameter<IEpochClock>(EpochClockParameterName, "", clocks, evaluationCountingClock));
      Parameters.Add(new FixedValueParameter<EnumValue<ProblemUpdatePolicy>>(UpdatePolicyParameterName, "Determines when the problem state changes", new EnumValue<ProblemUpdatePolicy>(ProblemUpdatePolicy.Immediate)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "Random Seed", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<CheckedItemList<IDynamicProblemTracker<TState>>>(TrackersParameterName,
        new CheckedItemList<IDynamicProblemTracker<TState>> {
          { new AnyTimeQualityTracker<TSolution, TState>(), true },
          { new SingleObjectiveAlgorithmPerformanceTracker(), true },
        }));
      RegisterEventHandlers();
      Operators.Add(new SingleObjectiveProblemStateAnalyzer() { Problem = this });
    }
    #endregion

    #region Problem Methods
    public sealed override double Evaluate(Individual individual, IRandom random) {
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.BeforeNextEvaluate)
        SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      double q;
      try {
        q = Evaluate(individual, random, true);
        foreach (var tracker in Trackers.OfType<ISingleObjectiveDynamicProblemTracker<TSolution, TState>>())
          tracker.OnEvaluation(GetData(), (TSolution)individual[Encoding.Name], q, EpochClock.CurrentEpoch,
            EpochClock.CurrentTime);
      } finally {
        rwLock.ReleaseReaderLock();
      }
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.AfterNextEvaluate)
        SafeUpdate();
      if (EpochClock == realTimeClock || EpochClock == evaluationCountingClock)
        EpochClock.Tick();
      return q;
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.BeforeNextAnalyze) 
        SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      try {
        Analyze(individuals, qualities, results, random, true);
      } finally {
        rwLock.ReleaseReaderLock();
      }
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.AfterNextAnalyze) 
        SafeUpdate();
      if (EpochClock == realTimeClock || EpochClock == generationCountingClock)
        EpochClock.Tick();
    }

    public override void RegisterAlgorithmEvents(IAlgorithm algorithm) {
      algorithm.ExecutionStateChanged += OnAlgorithmExecutionStateChanged;
    }

    public override void DeregisterAlgorithmEvents(IAlgorithm algorithm) {
      algorithm.ExecutionStateChanged -= OnAlgorithmExecutionStateChanged;
    }

    public event EventHandler<long> EpochChanged;
    public void AnalyzeProblem(ResultCollection results, IRandom random) {
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.BeforeNextAnalyze) 
        SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      try {
        AnalyzeProblem(results, random, true);
      } finally {
        rwLock.ReleaseReaderLock();
      }
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.AfterNextAnalyze) 
        SafeUpdate();
    }

    #endregion

    #region protected Methods

    protected virtual void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random, bool dummy) {
      base.Analyze(individuals, qualities, results, random);
    }

    protected virtual void AnalyzeProblem(ResultCollection results, IRandom random, bool dummy) {
      foreach (var tracker in Trackers) {
        tracker.OnAnalyze(GetData(), results);
      }
    }

    protected abstract void Update(long version);

    protected abstract double Evaluate(Individual individual, IRandom random, bool dummy);

    protected abstract void AlgorithmReset();

    protected abstract void AlgorithmStart();

    protected void RegisterEventHandlers() {
      EpochClockParameter.ValueChanged -= OnEpochClockChanged;
      EpochClockParameter.ValueChanged += OnEpochClockChanged;
      EpochClock.NewVersion -= OnNewVersion;
      EpochClock.NewVersion += OnNewVersion;
    }
    #endregion

    #region Helpers

    private void OnAlgorithmExecutionStateChanged(object sender, EventArgs e) {
      var alg = (IAlgorithm)sender;
      switch (alg.ExecutionState) {
        case ExecutionState.Prepared:
          EpochClock.Reset();
          ClockVersion = 0;
          ClockTime = 0;
          EnvironmentRandom = null;
          InitPending = true;
          foreach (var tracker in Trackers) {
            tracker.Reset();
          }
          AlgorithmReset();
          break;
        case ExecutionState.Started:
          ResetEnvironmentRandom();
          SafeUpdate(); // do we really want to trigger an epoch change on algorithm start?
          break;
        case ExecutionState.Paused:
          EpochClock.Pause();
          break;
        case ExecutionState.Stopped:
          EpochClock.Stop();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void SafeUpdate() {
      rwLock.AcquireWriterLock(-1);
      if (InitPending) {
        InitPending = false;
        AlgorithmStart();
        EpochClock.Start(false);
      } else if (!EpochClock.IsRunning) {
        EpochClock.Resume();
      }

      try {
        Update(ClockVersion);
        Dirty = false;
        
        foreach (var tracker in Trackers) {
          tracker.OnEpochChange(GetData(), ClockVersion, ClockTime);
        }
      } finally {
        rwLock.ReleaseWriterLock();
      }
      EpochChanged?.Invoke(this, ClockVersion);
    }

    protected abstract TState GetData();

    private void ResetEnvironmentRandom() {
      if (SetSeedRandomly) Seed = new System.Random().Next();
      EnvironmentRandom = new FastRandom(Seed);
    }

    private void OnEpochClockChanged(object sender, EventArgs e) {
      foreach (var validValue in EpochClockParameter.ValidValues)
        validValue.NewVersion -= OnNewVersion;
      if (EpochClock == null) return;
      EpochClock.NewVersion += OnNewVersion;
    }

    private void OnNewVersion(object sender, EventArgs<long, long> e) {
      ClockVersion = e.Value;
      ClockTime = e.Value2;
      Dirty = true;
      if (ProblemUpdatePolicy == ProblemUpdatePolicy.Immediate) {
        SafeUpdate();
      }
    }
    #endregion
  }
}
