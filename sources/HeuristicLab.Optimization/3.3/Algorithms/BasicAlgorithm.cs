#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class BasicAlgorithm : Algorithm, IStorableContent {
    public string Filename { get; set; }

    [Storable]
    private ResultCollection results;
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
    }
    protected BasicAlgorithm()
      : base() {
      results = new ResultCollection();
    }

    public override void Prepare() {
      if (Problem == null) return;
      base.Prepare();
      results.Clear();
      OnPrepared();
    }

    public override void Start() {
      base.Start();
      CancellationTokenSource = new CancellationTokenSource();

      OnStarted();
      Task task = Task.Factory.StartNew(Run, cancellationTokenSource.Token, cancellationTokenSource.Token);
      task.ContinueWith(t => {
        try {
          t.Wait();
        }
        catch (AggregateException ex) {
          try {
            ex.Flatten().Handle(x => x is OperationCanceledException);
          }
          catch (AggregateException remaining) {
            if (remaining.InnerExceptions.Count == 1) OnExceptionOccurred(remaining.InnerExceptions[0]);
            else OnExceptionOccurred(remaining);
          }
        }
        CancellationTokenSource.Dispose();
        CancellationTokenSource = null;
        OnStopped();
      });
    }

    public override void Pause() {
      throw new NotSupportedException("Pause is not supported in basic algorithms.");
    }

    public override void Stop() {
      //CancellationToken.ThrowIfCancellationRequested() must be called from within the Run method, otherwise stop does nothing
      base.Stop();
      CancellationTokenSource.Cancel();
    }


    private DateTime lastUpdateTime;
    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.UtcNow;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        Run(cancellationToken);
      }
      finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      }
    }

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
