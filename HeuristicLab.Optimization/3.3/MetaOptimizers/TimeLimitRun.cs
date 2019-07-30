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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A run in which an algorithm is executed for a certain maximum time only.
  /// </summary>
  [Item("Timelimit Run", "A run in which an optimizer is executed a certain maximum time.")]
  [Creatable(CreatableAttribute.Categories.TestingAndAnalysis, Priority = 115)]
  [StorableType("85A1AB82-689F-4925-B888-B3886707BE88")]
  public sealed class TimeLimitRun : NamedItem, IOptimizer, IStorableContent, INotifyPropertyChanged {
    public string Filename { get; set; }

    #region ItemImage
    public static new Image StaticItemImage {
      get { return VSImageLibrary.Event; }
    }
    public override Image ItemImage {
      get { return (Algorithm != null) ? Algorithm.ItemImage : VSImageLibrary.ExecutableStopped; }
    }
    #endregion

    private bool pausedForSnapshot = false;
    private bool pausedForTermination = false;

    [Storable]
    private TimeSpan maximumExecutionTime;
    public TimeSpan MaximumExecutionTime {
      get { return maximumExecutionTime; }
      set {
        if (maximumExecutionTime == value) return;
        maximumExecutionTime = value;
        OnPropertyChanged(nameof(MaximumExecutionTime));
      }
    }

    [Storable]
    private int snapshotTimesIndex;
    [Storable]
    private ObservableList<TimeSpan> snapshotTimes;
    public ObservableList<TimeSpan> SnapshotTimes {
      get { return snapshotTimes; }
      set {
        if (snapshotTimes == value) return;
        snapshotTimes = value;
        snapshotTimes.Sort();
        FindNextSnapshotTimeIndex(ExecutionTime);
        OnPropertyChanged(nameof(SnapshotTimes));
      }
    }

    [Storable]
    private bool storeAlgorithmInEachSnapshot;
    [Storable]
    public bool StoreAlgorithmInEachSnapshot {
      get { return storeAlgorithmInEachSnapshot; }
      set {
        if (storeAlgorithmInEachSnapshot == value) return;
        storeAlgorithmInEachSnapshot = value;
        OnPropertyChanged(nameof(StoreAlgorithmInEachSnapshot));
      }
    }

    [Storable]
    private RunCollection snapshots;
    public RunCollection Snapshots {
      get { return snapshots; }
      set {
        if (snapshots == value) return;
        snapshots = value;
        OnPropertyChanged(nameof(Snapshots));
      }
    }

    #region Inherited Properties
    [Storable]
    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      private set {
        if (executionState == value) return;
        executionState = value;
        OnExecutionStateChanged();
        OnPropertyChanged(nameof(ExecutionState));
      }
    }

    public TimeSpan ExecutionTime {
      get { return (Algorithm != null) ? Algorithm.ExecutionTime : TimeSpan.FromSeconds(0); }
    }

    [Storable]
    private IAlgorithm algorithm;
    public IAlgorithm Algorithm {
      get { return algorithm; }
      set {
        if (algorithm == value) return;
        if (ExecutionState == ExecutionState.Started || ExecutionState == ExecutionState.Paused)
          throw new InvalidOperationException("Cannot change algorithm while the TimeLimitRun is running or paused.");
        if (algorithm != null) DeregisterAlgorithmEvents();
        algorithm = value;
        if (algorithm != null) RegisterAlgorithmEvents();
        OnPropertyChanged(nameof(Algorithm));
        OnPropertyChanged(nameof(NestedOptimizers));
        if (algorithm != null) {
          if (algorithm.ExecutionState == ExecutionState.Started || algorithm.ExecutionState == ExecutionState.Paused)
            throw new InvalidOperationException("Cannot assign a running or paused algorithm to a TimeLimitRun.");
          Prepare();
          if (algorithm.ExecutionState == ExecutionState.Stopped) OnStopped();
        } else OnStopped();
      }
    }

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
      private set {
        if (value == null) throw new ArgumentNullException();
        if (runs == value) return;
        runs = value;
        OnPropertyChanged(nameof(Runs));
      }
    }

    public IEnumerable<IOptimizer> NestedOptimizers {
      get {
        if (Algorithm == null) yield break;
        yield return Algorithm;
        foreach (var opt in Algorithm.NestedOptimizers)
          yield return opt;
      }
    }
    #endregion

    [StorableConstructor]
    private TimeLimitRun(StorableConstructorFlag _) : base(_) { }
    private TimeLimitRun(TimeLimitRun original, Cloner cloner)
      : base(original, cloner) {
      maximumExecutionTime = original.maximumExecutionTime;
      snapshotTimes = new ObservableList<TimeSpan>(original.snapshotTimes);
      snapshotTimesIndex = original.snapshotTimesIndex;
      snapshots = cloner.Clone(original.snapshots);
      storeAlgorithmInEachSnapshot = original.storeAlgorithmInEachSnapshot;
      executionState = original.executionState;
      algorithm = cloner.Clone(original.algorithm);
      runs = cloner.Clone(original.runs);

      Initialize();
    }
    public TimeLimitRun()
      : base() {
      name = ItemName;
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      maximumExecutionTime = TimeSpan.FromMinutes(.5);
      snapshotTimes = new ObservableList<TimeSpan>(new[] {
          TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(10),
          TimeSpan.FromSeconds(15) });
      snapshotTimesIndex = 0;
      snapshots = new RunCollection();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    public TimeLimitRun(string name)
      : base(name) {
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      maximumExecutionTime = TimeSpan.FromMinutes(.5);
      snapshotTimes = new ObservableList<TimeSpan>(new[] {
          TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(10),
          TimeSpan.FromSeconds(15) });
      snapshotTimesIndex = 0;
      snapshots = new RunCollection();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    public TimeLimitRun(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      maximumExecutionTime = TimeSpan.FromMinutes(.5);
      snapshotTimes = new ObservableList<TimeSpan>(new[] {
          TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(10),
          TimeSpan.FromSeconds(15) });
      snapshotTimesIndex = 0;
      snapshots = new RunCollection();
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      return new TimeLimitRun(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (Algorithm != null && executionState != Algorithm.ExecutionState) {
        executionState = Algorithm.ExecutionState;
      } else if (Algorithm == null) executionState = ExecutionState.Stopped;
      #endregion
    }

    private void Initialize() {
      if (algorithm != null) RegisterAlgorithmEvents();
      snapshotTimes.ItemsAdded += snapshotTimes_Changed;
      snapshotTimes.ItemsMoved += snapshotTimes_Changed;
      snapshotTimes.ItemsRemoved += snapshotTimes_Changed;
      snapshotTimes.ItemsReplaced += snapshotTimes_Changed;
      snapshotTimes.CollectionReset += snapshotTimes_Changed;
    }

    private void snapshotTimes_Changed(object sender, CollectionItemsChangedEventArgs<IndexedItem<TimeSpan>> e) {
      if (e.Items.Any()) snapshotTimes.Sort();
      FindNextSnapshotTimeIndex(ExecutionTime);
    }

    public void Snapshot() {
      if (Algorithm == null || Algorithm.ExecutionState != ExecutionState.Paused) throw new InvalidOperationException("Snapshot not allowed in execution states other than Paused");
      Task.Factory.StartNew(MakeSnapshot);
    }

    public void Prepare() {
      Prepare(false);
    }
    public void Prepare(bool clearRuns) {
      if (Algorithm == null) return;
      Algorithm.Prepare(clearRuns);
    }
    public void Start() {
      Start(CancellationToken.None);
    }
    public void Start(CancellationToken cancellationToken) {
      Algorithm.Start(cancellationToken);
    }
    public async Task StartAsync() { await StartAsync(CancellationToken.None); }
    public async Task StartAsync(CancellationToken cancellationToken) {
      await AsyncHelper.DoAsync(Start, cancellationToken);
    }
    public void Pause() {
      Algorithm.Pause();
    }
    public void Stop() {
      Algorithm.Stop();
    }

    #region Events
    protected override void OnNameChanged() {
      base.OnNameChanged();
      runs.OptimizerName = Name;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(property));
    }

    #region IExecutable Events
    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged() {
      var handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      var handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      ExecutionState = ExecutionState.Started;
      var handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      var handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      ExecutionState = ExecutionState.Stopped;
      var handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      ExecutionState = ExecutionState.Paused;
      var handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }
    #endregion

    #region Algorithm Events
    private void RegisterAlgorithmEvents() {
      algorithm.ExceptionOccurred += Algorithm_ExceptionOccurred;
      algorithm.ExecutionTimeChanged += Algorithm_ExecutionTimeChanged;
      algorithm.Paused += Algorithm_Paused;
      algorithm.Prepared += Algorithm_Prepared;
      algorithm.Started += Algorithm_Started;
      algorithm.Stopped += Algorithm_Stopped;
    }
    private void DeregisterAlgorithmEvents() {
      algorithm.ExceptionOccurred -= Algorithm_ExceptionOccurred;
      algorithm.ExecutionTimeChanged -= Algorithm_ExecutionTimeChanged;
      algorithm.Paused -= Algorithm_Paused;
      algorithm.Prepared -= Algorithm_Prepared;
      algorithm.Started -= Algorithm_Started;
      algorithm.Stopped -= Algorithm_Stopped;
    }
    private void Algorithm_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Algorithm_ExecutionTimeChanged(object sender, EventArgs e) {
      if (snapshotTimesIndex < SnapshotTimes.Count && ExecutionTime >= SnapshotTimes[snapshotTimesIndex]
        && !pausedForSnapshot) {
        pausedForSnapshot = true;
        Algorithm.Pause();
      }
      if (ExecutionTime >= MaximumExecutionTime && !pausedForTermination) {
        pausedForTermination = true;
        if (!pausedForSnapshot) Algorithm.Pause();
      }
      OnExecutionTimeChanged();
    }
    private void Algorithm_Paused(object sender, EventArgs e) {
      var action = pausedForTermination ? ExecutionState.Stopped : (pausedForSnapshot ? ExecutionState.Started : ExecutionState.Paused);
      if (pausedForSnapshot || pausedForTermination) {
        pausedForSnapshot = pausedForTermination = false;
        MakeSnapshot();
        FindNextSnapshotTimeIndex(ExecutionTime);
      } else OnPaused();
      if (action == ExecutionState.Started) Algorithm.Start();
      else if (action == ExecutionState.Stopped) Algorithm.Stop();
    }
    private void Algorithm_Prepared(object sender, EventArgs e) {
      snapshotTimesIndex = 0;
      snapshots.Clear();
      OnPrepared();
    }
    private void Algorithm_Started(object sender, EventArgs e) {
      if (ExecutionState == ExecutionState.Prepared || ExecutionState == ExecutionState.Paused)
        OnStarted();
      if (ExecutionState == ExecutionState.Stopped)
        throw new InvalidOperationException("Algorithm was started although TimeLimitRun was in state Stopped.");
      // otherwise the algorithm was just started after being snapshotted
    }
    private void Algorithm_Stopped(object sender, EventArgs e) {
      try {
        var cloner = new Cloner();
        var algRun = cloner.Clone(Algorithm.Runs.Last());
        var clonedSnapshots = cloner.Clone(snapshots);
        algRun.Results.Add("TimeLimitRunSnapshots", clonedSnapshots);
        Runs.Add(algRun);
        Algorithm.Runs.Clear();
      } finally { OnStopped(); }
    }
    #endregion
    #endregion

    private void FindNextSnapshotTimeIndex(TimeSpan reference) {
      var index = 0;
      while (index < snapshotTimes.Count && snapshotTimes[index] <= reference) {
        index++;
      }
      snapshotTimesIndex = index;
    }

    private void MakeSnapshot() {
      string time = Math.Round(ExecutionTime.TotalSeconds, 1).ToString("0.0");
      string runName = "Snapshot " + time + "s " + algorithm.Name;
      var changed = false;
      if (StoreAlgorithmInEachSnapshot && !Algorithm.StoreAlgorithmInEachRun) {
        Algorithm.StoreAlgorithmInEachRun = true;
        changed = true;
      } else if (!StoreAlgorithmInEachSnapshot && Algorithm.StoreAlgorithmInEachRun) {
        Algorithm.StoreAlgorithmInEachRun = false;
        changed = true;
      }
      var run = new Run(runName, Algorithm);
      if (changed)
        Algorithm.StoreAlgorithmInEachRun = !Algorithm.StoreAlgorithmInEachRun;

      snapshots.Add(run);
    }
  }
}
