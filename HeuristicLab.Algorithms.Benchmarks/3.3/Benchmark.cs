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
using System.Drawing;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.Benchmarks {
  [Item("Benchmark", "Base class for benchmarks.")]
  [StorableType("0715912F-4F40-40F6-AC08-677CC4F76819")]
  public abstract class Benchmark : IBenchmark {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public static Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public virtual Image ItemImage {
      get { return ItemAttribute.GetImage(this.GetType()); }
    }

    [Storable]
    private byte[][] chunk;
    public byte[][] ChunkData {
      get { return chunk; }
      set { chunk = value; }
    }

    [Storable]
    private TimeSpan timeLimit;
    public TimeSpan TimeLimit {
      get { return timeLimit; }
      set { timeLimit = value; }
    }

    [StorableConstructor]
    protected Benchmark(StorableConstructorFlag _) { }
    protected Benchmark(Benchmark original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
    }
    protected Benchmark() { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public abstract IDeepCloneable Clone(Cloner cloner);

    public abstract void Run(CancellationToken token, ResultCollection results);

    public override string ToString() {
      return ItemName;
    }

    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
