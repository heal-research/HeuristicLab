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

  [StorableType("BA2F4B70-8112-43E7-84B3-8B5901D41D35")]
  public abstract class MultiObjectiveDynamicBasicProblem<TEncoding, TSolution, TData> :
    MultiObjectiveBasicProblem<TEncoding>,
    IMultiObjectiveDynamicProblemDefinition
    where TEncoding : class, IEncoding
    where TSolution : class, IItem
    where TData : class {
    [Storable] private bool InitPending =false;
    
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
    public IFixedValueParameter<CheckedItemList<IDynamicProblemTracker<TData>>> TrackersParameter =>
      (IFixedValueParameter<CheckedItemList<IDynamicProblemTracker<TData>>>)Parameters[TrackersParameterName];
    #endregion

    #region Properties
    public int Seed { get { return SeedParameter.Value.Value; } set { SeedParameter.Value.Value = value; } }
    public bool SetSeedRandomly { get { return SetSeedRandomlyParameter.Value.Value; } set { SetSeedRandomlyParameter.Value.Value = value; } }
    public IEpochClock EpochClock => EpochClockParameter.Value;
    public ProblemUpdatePolicy ProblemUpdatePolicy => UpdatePolicyParameter.Value.Value;
    public IEnumerable<IDynamicProblemTracker<TData>> Trackers => TrackersParameter.Value.CheckedItems.Select(x=>x.Value);
    #endregion

    #region Fields and Storbales
    private readonly ReaderWriterLock rwLock = new ReaderWriterLock();
    [Storable]
    private bool Dirty { get; set; }
    [Storable]
    private long ClockVersion { get; set; }
    [Storable]
    private long ClockTime { get; set; }
    [Storable]
    protected IRandom EnvironmentRandom { get; set; }
    #endregion

    #region Constructors and cloning
    [StorableConstructor]
    protected MultiObjectiveDynamicBasicProblem(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
      if(!Operators.OfType<MultiObjectiveProblemStateAnalyzer>().Any())
        Operators.Add(new MultiObjectiveProblemStateAnalyzer(){Problem = this});
    }

    protected MultiObjectiveDynamicBasicProblem(MultiObjectiveDynamicBasicProblem<TEncoding, TSolution, TData> original, Cloner cloner)
      : base(original, cloner) {
      Dirty = original.Dirty;
      ClockVersion = original.ClockVersion;
      ClockTime = original.ClockTime;
      EnvironmentRandom = cloner.Clone(original.EnvironmentRandom);
      RegisterEventHandlers();
      InitPending = original.InitPending;
    }

    protected MultiObjectiveDynamicBasicProblem() {
      var clocks = new ItemSet<IEpochClock>(ApplicationManager.Manager.GetInstances(typeof(IEpochClock)).Cast<IEpochClock>());
      Parameters.Add(new ConstrainedValueParameter<IEpochClock>(EpochClockParameterName, "", clocks, clocks.OfType<CountingClock>().Single()));
      Parameters.Add(new FixedValueParameter<EnumValue<ProblemUpdatePolicy>>(UpdatePolicyParameterName, "Determines when the problem state changes", new EnumValue<ProblemUpdatePolicy>(ProblemUpdatePolicy.Immediate)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "Random Seed", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<CheckedItemList<IDynamicProblemTracker<TData>>>(TrackersParameterName,
        new CheckedItemList<IDynamicProblemTracker<TData>> {
          //TODO add generic algPerf trackers here
        }));
      RegisterEventHandlers();
      Operators.Add(new MultiObjectiveProblemStateAnalyzer(){Problem = this});
    }
    #endregion

    #region Problem Methods
    public sealed override double[] Evaluate(Individual individual, IRandom random) {
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.OnNextEvaluate)
        SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      double[] q;
      try {
        q = Evaluate(individual, random, true);
        foreach (var tracker in Trackers.OfType<IMultiObjectiveDynamicProblemTracker<TSolution, TData>>())
          tracker.OnEvaluation(GetData(), (TSolution)individual[Encoding.Name], q, EpochClock.CurrentEpoch, EpochClock.CurrentTime);

      } finally {
        rwLock.ReleaseReaderLock();
      }
      (EpochClock as CountingClock)?.Tick();
      return q;
    }

    public override void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.OnNextAnalyze) SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      try {
        Analyze(individuals, qualities, results, random, true);
      } finally { rwLock.ReleaseReaderLock(); }
    }

    public override void RegisterAlgorithmEvents(IAlgorithm algorithm) {
      algorithm.ExecutionStateChanged += OnAlgorithmExecutionStateChanged;
    }

    public override void DeregisterAlgorithmEvents(IAlgorithm algorithm) {
      algorithm.ExecutionStateChanged -= OnAlgorithmExecutionStateChanged;
    }

    public event EventHandler<long> EpochChanged;
    public void AnalyzeProblem(ResultCollection results, IRandom random) {
      if (Dirty && ProblemUpdatePolicy == ProblemUpdatePolicy.OnNextAnalyze) SafeUpdate();
      rwLock.AcquireReaderLock(-1);
      try {
        AnalyzeProblem(results, random, true);
      } finally { rwLock.ReleaseReaderLock(); }
    }

    #endregion

    #region protected Methods

    protected virtual void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results,
                                   IRandom random, bool dummy) {
      base.Analyze(individuals, qualities, results, random);
    }

    protected virtual void AnalyzeProblem(ResultCollection results, IRandom random, bool dummy) {
      foreach (var tracker in Trackers) tracker.OnAnalyze(GetData(), results);
    }

    protected abstract void Update(long version);

    protected abstract double[] Evaluate(Individual individual, IRandom random, bool dummy);

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
          foreach (var tracker in Trackers) tracker.Reset();
          EnvironmentRandom = null;
          AlgorithmReset();
          break;
        case ExecutionState.Started:
          ResetEnvironmentRandom();
          InitPending = true;//AlgorithmStart();
          //EpochClock.Start(false);
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
      }

      try {
        Update(ClockVersion);
        foreach (var tracker in Trackers) tracker.OnEpochChange(GetData(), ClockVersion, ClockTime);
      } finally { rwLock.ReleaseWriterLock(); }
      EpochChanged?.Invoke(this, ClockVersion);
    }

    protected abstract TData GetData();

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
      if (ProblemUpdatePolicy == ProblemUpdatePolicy.Immediate)
        SafeUpdate();
      else
        Dirty = true;
    }
    #endregion
  }
}
