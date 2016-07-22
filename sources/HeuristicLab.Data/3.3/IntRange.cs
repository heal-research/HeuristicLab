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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass]
  [Item("IntRange", "Represents a range of values betweent start and end.")]
  public class IntRange : StringConvertibleValueTuple<IntValue, IntValue> {

    public int Start {
      get { return Item1.Value; }
      set { Item1.Value = value; }
    }
    public int End {
      get { return Item2.Value; }
      set { Item2.Value = value; }
    }
    public int Size {
      get { return End - Start; }
    }

    [StorableConstructor]
    protected IntRange(bool deserializing) : base(deserializing) { }
    protected IntRange(IntRange original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntRange(this, cloner);
    }

    public IntRange() : base(new IntValue(), new IntValue()) { }
    public IntRange(IntValue start, IntValue end) : base(start, end) { }
    public IntRange(int start, int end) : base(new IntValue(start), new IntValue(end)) { }

    public override string ToString() {
      return string.Format("Start: {0}, End: {1}", Start, End);
    }

    public override StringConvertibleValueTuple<IntValue, IntValue> AsReadOnly() {
      var readOnly = new IntRange();
      readOnly.values = Tuple.Create<IntValue, IntValue>((IntValue)Item1.AsReadOnly(), (IntValue)Item2.AsReadOnly());
      readOnly.readOnly = true;
      return readOnly;
    }
  }
}
