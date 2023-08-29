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
using System.Threading;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Real Time Clock", "Represents a problem whose objective is to maximize the number of true values.")]
  [StorableType("DA842A7B-5F46-4716-B2B8-6F304CF92765")]
  public class RealTimeClock : ParameterizedNamedItem, IEpochClock {
    private long version;
    private DateTime lastTick;
    private CancellationTokenSource cts;

    private const string IntervalSizeParameterName = "IntervalSize";

    public IFixedValueParameter<TimeSpanValue> IntervalSizeParameter => (IFixedValueParameter<TimeSpanValue>)Parameters[IntervalSizeParameterName];

    public TimeSpan IntervalSize {
      get { return IntervalSizeParameter.Value.Value; }
      set { IntervalSizeParameter.Value.Value = value; }
    }

    public long CurrentEpoch => Interlocked.Read(ref version);
    public long CurrentTime => DateTime.Now.Ticks;
    public bool IsRunning => cts != null;

    #region Constructors and cloning
    public RealTimeClock() {
      Parameters.Add(new FixedValueParameter<TimeSpanValue>(IntervalSizeParameterName, "Number of increases before a version increase", new TimeSpanValue(TimeSpan.FromSeconds(10))));
    }

    [StorableConstructor]
    protected RealTimeClock(StorableConstructorFlag _) : base(_) { }

    protected RealTimeClock(RealTimeClock original, Cloner cloner) : base(original, cloner) {
      version = original.CurrentEpoch;
      lastTick = original.lastTick;
      if (original.cts != null) throw new InvalidOperationException();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealTimeClock(this, cloner);
    }
    #endregion

    #region stopwatch functions //TODO make them thread safe
    public void Start(bool throwEvent) {
      lastTick = DateTime.Now;
      version = 0;
      if (throwEvent) 
        NewVersion?.Invoke(this, new EventArgs<long, long>(version, lastTick.Ticks));
      Resume();
    }

    public void Pause() {
      Stop();
    }

    public void Resume() {
      if (cts != null) throw new InvalidOperationException("can not start/resume clock that is running");
      cts = new CancellationTokenSource();
      Task.Factory.StartNew(() => RunTimer(cts.Token), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public void Stop() {
      if (cts == null) return;
      cts.Cancel();
      cts = null;
    }

    public void Reset(bool throwEvent) {
      lastTick = DateTime.MinValue;
      version = -1;
      if (throwEvent) 
        NewVersion?.Invoke(this,new EventArgs<long, long>(version, lastTick.Ticks));
    }

    public void Tick() {
      // nothing to do here
    }

    #endregion

    public event EventHandler<EventArgs<long, long>> NewVersion;

    private async void RunTimer(CancellationToken token) {
      while (!token.IsCancellationRequested) {
        var now = DateTime.Now;
        if (now < lastTick + IntervalSize) {
          await Task.Delay(lastTick + IntervalSize - now, token);
        }
        if (token.IsCancellationRequested) continue;
        lastTick = DateTime.Now;
        long v = Interlocked.Increment(ref version);
        NewVersion?.Invoke(this,new EventArgs<long, long>(v, lastTick.Ticks));
      }
    }
  }
}
