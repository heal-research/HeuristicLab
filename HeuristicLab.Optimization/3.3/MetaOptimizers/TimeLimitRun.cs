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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A run in which an algorithm is executed for a certain maximum time only.
  /// </summary>
  [Item("Timelimit Run", "A run in which an optimizer is executed a certain maximum time.")]
  [Creatable(CreatableAttribute.Categories.TestingAndAnalysis, Priority = 115)]
  [StorableClass]
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
        OnPropertyChanged("MaximumExecutionTime");
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
        OnPropertyChanged("SnapshotTimes");
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
        OnPropertyChanged("StoreAlgorithmInEachSnapshot");
      }
    }

    [Storable]
    private RunCollection snapshots;
    public RunCollection Snapshots {
      get { return snapshots; }
      set {
        if (snapshots == value) return;
        snapshots = value;
        OnPropertyChanged("Snapshots");
      }
    }

    #region Inherited Properties
    public ExecutionState ExecutionState {
      get { return (Algorithm != null) ? Algorithm.ExecutionState : ExecutionState.Stopped; }
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
        if (algorithm != null) DeregisterAlgorithmEvents();
        algorithm = value;
        if (algorithm != null) {
          RegisterAlgorithmEvents();
        }
        OnPropertyChanged("Algorithm");
        Prepare();
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
        OnPropertyChanged("Runs");
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
    private TimeLimitRun(bool deserializing) : base(deserializing) { }
    private TimeLimitRun(TimeLimitRun original, Cloner cloner)
      : base(original, cloner) {
      maximumExecutionTime = original.maximumExecutionTime;
      snapshotTimes = new ObservableList<TimeSpan>(original.snapshotTimes);
      snapshotTimesIndex = original.snapshotTimesIndex;
      snapshots = cloner.Clone(original.snapshots);
      storeAlgorithmInEachSnapshot = original.storeAlgorithmInEachSnapshot;
      algorithm = cloner.Clone(original.algorithm);
      runs = cloner.Clone(original.runs);

      Initialize();
    }
    public TimeLimitRun()
      : base() {
      name = ItemName;
      description = ItemDescription;
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
      maximumExecutionTime = TimeSpan.FromMinutes(.5);
      snapshotTimes = new ObservableList<TimeSpan>(new[] {
          TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(10),
          TimeSpan.FromSeconds(15) });
      snapshotTimesIndex = 0;
      Runs = new RunCollection { OptimizerName = Name };
      Initialize();
    }
    public TimeLimitRun(string name, string description)
      : base(name, description) {
      maximumExecutionTime = TimeSpan.FromMinutes(.5);
      snapshotTimes = new ObservableList<TimeSpan>(new[] {
          TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(10),
          TimeSpan.FromSeconds(15) });
      snapshotTimesIndex = 0;
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
      Algorithm.Prepare(clearRuns);
    }
    public void Start() {
      Algorithm.Start();
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
      var handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      var handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      var handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      var handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      var handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }
    #endregion

    #region Algorithm Events
    private void RegisterAlgorithmEvents() {
      algorithm.ExceptionOccurred += Algorithm_ExceptionOccurred;
      algorithm.ExecutionTimeChanged += Algorithm_ExecutionTimeChanged;
      algorithm.ExecutionStateChanged += Algorithm_ExecutionStateChanged;
      algorithm.Paused += Algorithm_Paused;
      algorithm.Prepared += Algorithm_Prepared;
      algorithm.Started += Algorithm_Started;
      algorithm.Stopped += Algorithm_Stopped;
    }
    private void DeregisterAlgorithmEvents() {
      algorithm.ExceptionOccurred -= Algorithm_ExceptionOccurred;
      algorithm.ExecutionTimeChanged -= Algorithm_ExecutionTimeChanged;
      algorithm.ExecutionStateChanged -= Algorithm_ExecutionStateChanged;
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
    private void Algorithm_ExecutionStateChanged(object sender, EventArgs e) {
      OnExecutionStateChanged();
    }
    private void Algorithm_Paused(object sender, EventArgs e) {
      var action = pausedForTermination ? ExecutionState.Stopped : (pausedForSnapshot ? ExecutionState.Started : ExecutionState.Paused);
      if (pausedForSnapshot || pausedForTermination) {
        pausedForSnapshot = pausedForTermination = false;
        MakeSnapshot();
        FindNextSnapshotTimeIndex(ExecutionTime);
      }
      OnPaused();
      if (action == ExecutionState.Started) Algorithm.Start();
      else if (action == ExecutionState.Stopped) Algorithm.Stop();
    }
    private void Algorithm_Prepared(object sender, EventArgs e) {
      snapshotTimesIndex = 0;
      snapshots.Clear();
      OnPrepared();
    }
    private void Algorithm_Started(object sender, EventArgs e) {
      OnStarted();
    }
    private void Algorithm_Stopped(object sender, EventArgs e) {
      var cloner = new Cloner();
      var algRun = cloner.Clone(Algorithm.Runs.Last());
      var clonedSnapshots = cloner.Clone(snapshots);
      algRun.Results.Add("TimeLimitRunSnapshots", clonedSnapshots);
      Runs.Add(algRun);
      Algorithm.Runs.Clear();
      OnStopped();
    }
    #endregion
    #endregion

    private void FindNextSnapshotTimeIndex(TimeSpan reference) {
      var index = 0;
      while (index < snapshotTimes.Count && snapshotTimes[index] <= reference) {
        index++;
      };
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
