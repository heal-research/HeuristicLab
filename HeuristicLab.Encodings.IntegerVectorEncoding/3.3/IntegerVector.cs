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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("2DDD33C1-4732-478B-83B1-17226A8F2B61")]
  [Item("IntegerVector", "Represents a vector of integer values.")]
  public class IntegerVector : IntArray, IEncodedSolution {
    [StorableConstructor]
    protected IntegerVector(StorableConstructorFlag _) : base(_) { }
    protected IntegerVector(IntegerVector original, Cloner cloner) : base(original, cloner) { }
    public IntegerVector() : base() { }
    public IntegerVector(int length) : base(length) { }
    public IntegerVector(int length, IRandom random, int min, int max)
      : this(length) {
      Randomize(random, min, max);
    }
    public IntegerVector(int[] elements) : base(elements) { }
    public IntegerVector(IntArray elements)
      : this(elements.Length) {
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i];
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerVector(this, cloner);
    }

    public virtual void Randomize(IRandom random, int startIndex, int length, int min, int max, int step = 1) {
      if (length > 0) {
        int numbers = (int)Math.Floor((max - min) / (double)step);
        for (int i = startIndex; i < startIndex + length; i++) {
          array[i] = random.Next(numbers) * step + min;
        }
        OnReset();
      }
    }
    public virtual void Randomize(IRandom random, int startIndex, int length, IntMatrix bounds) {
      if (length > 0) {
        for (int i = startIndex; i < startIndex + length; i++) {
          int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1], step = 1;
          if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
          int numbers = (int)Math.Floor((max - min) / (double)step);
          array[i] = random.Next(numbers) * step + min;
        }
        OnReset();
      }
    }
    public void Randomize(IRandom random, int min, int max, int step = 1) {
      Randomize(random, 0, Length, min, max, step);
    }
    public void Randomize(IRandom random, IntMatrix bounds) {
      Randomize(random, 0, Length, bounds);
    }
  }
}
