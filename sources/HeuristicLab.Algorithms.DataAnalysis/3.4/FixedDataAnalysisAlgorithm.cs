#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  public abstract class FixedDataAnalysisAlgorithm<T> : Algorithm,
    IDataAnalysisAlgorithm<T>,
    IStorableContent
    where T : class, IDataAnalysisProblem {
    public string Filename { get; set; }

    #region Properties
    public override Type ProblemType {
      get { return typeof(T); }
    }
    public new T Problem {
      get { return (T)base.Problem; }
      set { base.Problem = value; }
    }
    [Storable]
    private ResultCollection results;
    public override ResultCollection Results {
      get { return results; }
    }
    #endregion

    private DateTime lastUpdateTime;

    [StorableConstructor]
    protected FixedDataAnalysisAlgorithm(bool deserializing) : base(deserializing) { }
    protected FixedDataAnalysisAlgorithm(FixedDataAnalysisAlgorithm<T> original, Cloner cloner)
      : base(original, cloner) {
      results = cloner.Clone(original.Results);
    }
    public FixedDataAnalysisAlgorithm()
      : base() {
      results = new ResultCollection();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
      results.Clear();
      OnPrepared();
    }

    public override void Start() {
      base.Start();
      var cancellationTokenSource = new CancellationTokenSource();

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
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        OnStopped();
      });
    }
    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.UtcNow;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        Run();
      }
      finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      }

      cancellationToken.ThrowIfCancellationRequested();
    }
    protected abstract void Run();
    #region Events
    protected override void OnProblemChanged() {
      Problem.Reset += new EventHandler(Problem_Reset);
      base.OnProblemChanged();
    }
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
