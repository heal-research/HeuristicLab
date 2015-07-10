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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("DoubleLimit", "Represents a lower and a upper bound for double values.")]
  public class DoubleLimit : StringConvertibleValueTuple<DoubleValue, DoubleValue> {

    public double Lower {
      get { return Item1.Value; }
      set { Item1.Value = value; }
    }
    public double Upper {
      get { return Item2.Value; }
      set { Item2.Value = value; }
    }

    [StorableConstructor]
    protected DoubleLimit(bool deserializing) : base(deserializing) { }
    protected DoubleLimit(DoubleLimit original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleLimit(this, cloner);
    }

    public DoubleLimit() : base(new DoubleValue(), new DoubleValue()) { }
    public DoubleLimit(DoubleValue start, DoubleValue end) : base(start, end) { }
    public DoubleLimit(double start, double end) : base(new DoubleValue(start), new DoubleValue(end)) { }

    public override string ToString() {
      return string.Format("Lower: {0}, Upper: {1}", Lower, Upper);
    }

    public override StringConvertibleValueTuple<DoubleValue, DoubleValue> AsReadOnly() {
      var readOnly = new DoubleLimit();
      readOnly.values = Tuple.Create<DoubleValue, DoubleValue>((DoubleValue)Item1.AsReadOnly(), (DoubleValue)Item2.AsReadOnly());
      readOnly.readOnly = true;
      return readOnly;
    }
  }
}
