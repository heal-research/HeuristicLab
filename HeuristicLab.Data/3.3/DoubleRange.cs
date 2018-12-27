#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass]
  [Item("DoubleRange", "Represents a range of values betweent start and end.")]
  public class DoubleRange : StringConvertibleValueTuple<DoubleValue, DoubleValue> {

    public double Start {
      get { return Item1.Value; }
      set { Item1.Value = value; }
    }
    public double End {
      get { return Item2.Value; }
      set { Item2.Value = value; }
    }
    public double Size {
      get { return End - Start; }
    }

    [StorableConstructor]
    protected DoubleRange(bool deserializing) : base(deserializing) { }
    protected DoubleRange(DoubleRange original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleRange(this, cloner);
    }

    public DoubleRange() : base(new DoubleValue(), new DoubleValue()) { }
    public DoubleRange(DoubleValue start, DoubleValue end) : base(start, end) { }
    public DoubleRange(double start, double end) : base(new DoubleValue(start), new DoubleValue(end)) { }

    public override string ToString() {
      return string.Format("Start: {0:r}, End: {1:r}", Start, End);
    }

    public override StringConvertibleValueTuple<DoubleValue, DoubleValue> AsReadOnly() {
      var readOnly = new DoubleRange();
      readOnly.values = Tuple.Create<DoubleValue, DoubleValue>((DoubleValue)Item1.AsReadOnly(), (DoubleValue)Item2.AsReadOnly());
      readOnly.readOnly = true;
      return readOnly;
    }
  }
}
