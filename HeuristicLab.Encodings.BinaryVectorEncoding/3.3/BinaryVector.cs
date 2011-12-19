#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [StorableClass]
  [Item("BinaryVector", "Represents a vector of binary values.")]
  public class BinaryVector : BoolArray {
    [StorableConstructor]
    protected BinaryVector(bool deserializing) : base(deserializing) { }
    protected BinaryVector(BinaryVector original, Cloner cloner) : base(original, cloner) { }
    public BinaryVector() : base() { }
    public BinaryVector(int length) : base(length) { }
    public BinaryVector(int length, IRandom random)
      : this(length) {
      Randomize(random);
    }
    public BinaryVector(bool[] elements) : base(elements) { }
    public BinaryVector(BoolArray elements)
      : this(elements.Length) {
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i];
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryVector(this, cloner);
    }

    public virtual void Randomize(IRandom random, int startIndex, int length) {
      if (length > 0) {
        for (int i = 0; i < length; i++)
          array[startIndex + i] = random.Next(2) == 0;
        OnReset();
      }
    }
    public void Randomize(IRandom random) {
      Randomize(random, 0, Length);
    }
  }
}
