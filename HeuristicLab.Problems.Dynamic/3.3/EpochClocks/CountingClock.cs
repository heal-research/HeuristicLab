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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Counting Clock", "Represents a problem whose objective is to maximize the number of true values.")]
  [StorableType("6E7664A8-56FA-4399-AC6C-3C4E5FD5CDDA")]
  public class CountingClock : ParameterizedNamedItem, IEpochClock {
    [Storable]
    private long version;
    [Storable]
    private long lastTick;
    [Storable]
    private bool running;

    private const string IntervalSizeParameterName = "IntervalSize";

    public IFixedValueParameter<IntValue> IntervalSizeParameter => (IFixedValueParameter<IntValue>)Parameters[IntervalSizeParameterName];

    public int IntervalSize {
      get { return IntervalSizeParameter.Value.Value; }
      set { IntervalSizeParameter.Value.Value = value; }
    }

    public long CurrentEpoch => Interlocked.Read(ref version);
    public long CurrentTime => Interlocked.Read(ref lastTick);
    public bool IsRunning => running;

    #region Constructors and cloning
    public CountingClock() {
      Parameters.Add(new FixedValueParameter<IntValue>(IntervalSizeParameterName, "Number of increases before a version increase", new IntValue(10000)));
    }

    [StorableConstructor]
    protected CountingClock(StorableConstructorFlag _) : base(_) {
    }

    protected CountingClock(CountingClock original, Cloner cloner) : base(original, cloner) {
      version = original.CurrentEpoch;
      lastTick = original.lastTick;
      running = original.running;
    }
    
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CountingClock(this, cloner);
    }
    #endregion

    #region stopwatch functions //TODO make them thread safe
    public void Start(bool throwEvent) {
      lastTick = 0;
      version = 0;
      if (throwEvent)
        NewVersion?.Invoke(this, new EventArgs<long, long>(CurrentEpoch, CurrentTime));
      Resume();
    }

    public void Pause() {
      Stop();
    }

    public void Resume() {
      if (running) throw new InvalidOperationException("can not start/resume clock that is running");
      running = true;
    }

    public void Stop() {
      running = false;
    }

    public void Reset(bool throwEvent) {
      lastTick = -1;
      version = -1;
      if (throwEvent)
        NewVersion?.Invoke(this,new EventArgs<long, long>(CurrentEpoch, CurrentTime));
    }

    #endregion

    public event EventHandler<EventArgs<long, long>> NewVersion;

    public void Tick() {
      long v = Interlocked.Increment(ref lastTick);
      if (v % IntervalSize == 0) {
        Interlocked.Increment(ref version);
        NewVersion?.Invoke(this,new EventArgs<long, long>(CurrentEpoch, CurrentTime));
      }
    }
  }
}
