#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A run in which an algorithm is executed a given number of times.
  /// </summary>
  [Item("Batch Run", "A run in which an algorithm is executed a given number of times.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public sealed class BatchRun : NamedItem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Event; }
    }

    private IAlgorithm algorithm;
    [Storable]
    private IAlgorithm AlgorithmPersistence {
      get { return algorithm; }
      set {
        if (algorithm != null) DeregisterAlgorithmEvents();
        algorithm = value;
        if (algorithm != null) RegisterAlgorithmEvents();
      }
    }
    public IAlgorithm Algorithm {
      get { return algorithm; }
      set {
        if (algorithm != value) {
          if (algorithm != null) DeregisterAlgorithmEvents();
          algorithm = value;
          if (algorithm != null) RegisterAlgorithmEvents();
          OnAlgorithmChanged();
          Prepare();
        }
      }
    }

    [Storable]
    private int repetitions;
    public int Repetitions {
      get { return repetitions; }
      set {
        if (repetitions != value) {
          repetitions = value;
          OnRepetitionsChanged();
        }
      }
    }

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
    }

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      private set {
        if (executionTime != value) {
          executionTime = value;
          OnExecutionTimeChanged();
        }
      }
    }

    private bool running;
    public bool Running {
      get { return running; }
      private set {
        if (running != value) {
          running = value;
          OnRunningChanged();
        }
      }
    }

    public bool Finished {
      get { return ((Algorithm == null) || (Algorithm.Finished && (runs.Count >= repetitions))); }
    }

    private bool canceled;

    public BatchRun()
      : base() {
      repetitions = 10;
      runs = new RunCollection();
      executionTime = TimeSpan.Zero;
    }
    public BatchRun(string name) : base(name) {
      repetitions = 10;
      runs = new RunCollection();
      executionTime = TimeSpan.Zero;
    }
    public BatchRun(string name, string description) : base(name, description) {
      repetitions = 10;
      runs = new RunCollection();
      executionTime = TimeSpan.Zero;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      BatchRun clone = (BatchRun)base.Clone(cloner);
      clone.Algorithm = (IAlgorithm)cloner.Clone(algorithm);
      clone.repetitions = repetitions;
      clone.runs = (RunCollection)cloner.Clone(runs);
      clone.executionTime = executionTime;
      clone.running = running;
      clone.canceled = canceled;
      return clone;
    }

    public void Prepare() {
      executionTime = TimeSpan.Zero;
      runs.Clear();
      if (Algorithm != null) Algorithm.Prepare();
      OnPrepared();
    }
    public void Start() {
      if (Algorithm != null) {
        OnStarted();
        Running = true;
        canceled = false;
        Algorithm.Start();
      }
    }
    public void Stop() {
      if (Algorithm != null) {
        canceled = true;
        Algorithm.Stop();
      }
    }

    #region Events
    public event EventHandler AlgorithmChanged;
    private void OnAlgorithmChanged() {
      if (AlgorithmChanged != null)
        AlgorithmChanged(this, EventArgs.Empty);
    }
    public event EventHandler RepetitionsChanged;
    private void OnRepetitionsChanged() {
      if (RepetitionsChanged != null)
        RepetitionsChanged(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      if (ExecutionTimeChanged != null)
        ExecutionTimeChanged(this, EventArgs.Empty);
    }
    public event EventHandler RunningChanged;
    private void OnRunningChanged() {
      if (RunningChanged != null)
        RunningChanged(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      if (Prepared != null)
        Prepared(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      if (Started != null)
        Started(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      if (Stopped != null)
        Stopped(this, EventArgs.Empty);
    }
    public event EventHandler<HeuristicLab.Common.EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new HeuristicLab.Common.EventArgs<Exception>(exception));
    }

    private void RegisterAlgorithmEvents() {
      algorithm.ExceptionOccurred += new EventHandler<HeuristicLab.Common.EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      algorithm.Started += new EventHandler(Algorithm_Started);
      algorithm.Stopped += new EventHandler(Algorithm_Stopped);
    }
    private void DeregisterAlgorithmEvents() {
      algorithm.ExceptionOccurred -= new EventHandler<HeuristicLab.Common.EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      algorithm.Started -= new EventHandler(Algorithm_Started);
      algorithm.Stopped -= new EventHandler(Algorithm_Stopped);
    }

    private void Algorithm_ExceptionOccurred(object sender, HeuristicLab.Common.EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Algorithm_Started(object sender, EventArgs e) {
      if (!Running) {
        OnStarted();
        Running = true;
        canceled = false;
      }
    }
    private void Algorithm_Stopped(object sender, EventArgs e) {
      if (!canceled) {
        ExecutionTime += Algorithm.ExecutionTime;
        runs.Add(new Run("Run " + Algorithm.ExecutionTime.ToString(), Algorithm));
        Algorithm.Prepare();

        if (runs.Count < repetitions)
          Algorithm.Start();
        else {
          Running = false;
          OnStopped();
        }
      } else {
        canceled = false;
        Running = false;
        OnStopped();
      }
    }
    #endregion
  }
}
