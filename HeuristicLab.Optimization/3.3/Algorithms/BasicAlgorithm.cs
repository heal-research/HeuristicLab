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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class BasicAlgorithm : Algorithm, IStorableContent {

    private bool pausePending;
    private DateTime lastUpdateTime;

    public string Filename { get; set; }

    public abstract bool SupportsPause { get; }
    public virtual bool SupportsStop {
      get { return true; }
    }

    [Storable]
    private bool initialized;
    [Storable]
    private readonly ResultCollection results;
    public override ResultCollection Results {
      get { return results; }
    }

    private CancellationTokenSource cancellationTokenSource;
    protected CancellationTokenSource CancellationTokenSource {
      get { return cancellationTokenSource; }
      private set { cancellationTokenSource = value; }
    }

    [StorableConstructor]
    protected BasicAlgorithm(bool deserializing) : base(deserializing) { }
    protected BasicAlgorithm(BasicAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      results = cloner.Clone(original.Results);
      initialized = original.initialized;
    }
    protected BasicAlgorithm()
      : base() {
      results = new ResultCollection();
    }

    public override void Prepare() {
      if (Problem == null) return;
      base.Prepare();
      results.Clear();
      initialized = false;
      OnPrepared();
    }

    public override void Start(CancellationToken cancellationToken) {
      base.Start(cancellationToken);
      CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      pausePending = false;
      OnStarted();

      try {
        Run((object)cancellationTokenSource.Token);
      } catch (OperationCanceledException) {
      } catch (AggregateException ae) {
        ae.FlattenAndHandle(new[] { typeof(OperationCanceledException) }, e => OnExceptionOccurred(e));
      } catch (Exception e) {
        OnExceptionOccurred(e);
      }

      CancellationTokenSource.Dispose();
      CancellationTokenSource = null;
      if (pausePending) OnPaused();
      else OnStopped();
    }

    public override void Pause() {
      // CancellationToken.ThrowIfCancellationRequested() must be called from within the Run method, otherwise pause does nothing
      // alternatively check the IsCancellationRequested property of the cancellation token
      if (!SupportsPause)
        throw new NotSupportedException("Pause is not supported by this algorithm.");

      base.Pause();
      pausePending = true;
      if (CancellationTokenSource != null) CancellationTokenSource.Cancel();
    }

    public override void Stop() {
      // CancellationToken.ThrowIfCancellationRequested() must be called from within the Run method, otherwise stop does nothing
      // alternatively check the IsCancellationRequested property of the cancellation token
      if (!SupportsStop)
        throw new NotSupportedException("Stop is not supported by this algorithm.");

      base.Stop();
      if (ExecutionState == ExecutionState.Paused) OnStopped();
      else if (CancellationTokenSource != null) CancellationTokenSource.Cancel();
    }

    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.UtcNow;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        if (!initialized)
          Initialize(cancellationToken);
        initialized = true;
        Run(cancellationToken);
      } finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      }
    }

    protected virtual void Initialize(CancellationToken cancellationToken) { }
    protected abstract void Run(CancellationToken cancellationToken);

    #region Events
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }
    #endregion

  }
}
