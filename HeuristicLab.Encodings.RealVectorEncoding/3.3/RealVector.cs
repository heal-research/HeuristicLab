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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [StorableType("C4213FB3-77C3-4814-A7A3-0151B0FF5270")]
  [Item("RealVector", "Represents a vector of real values.")]
  public class RealVector : DoubleArray {
    [StorableConstructor]
    protected RealVector(StorableConstructorFlag _) : base(_) { }
    protected RealVector(RealVector original, Cloner cloner) : base(original, cloner) { }
    public RealVector() : base() { }
    public RealVector(int length) : base(length) { }
    public RealVector(int length, IRandom random, double min, double max)
      : this(length) {
      Randomize(random, min, max);
    }
    public RealVector(double[] elements) : base(elements) { }
    public RealVector(DoubleArray elements)
      : this(elements.Length) {
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i];
    }
    public RealVector(RealVector other) : this(other.array) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVector(this, cloner);
    }

    public virtual void Randomize(IRandom random, int startIndex, int length, double min, double max) {
      double delta = max - min;
      if (length > 0) {
        for (int i = 0; i < length; i++)
          array[startIndex + i] = min + delta * random.NextDouble();
        OnReset();
      }
    }

    public virtual void Randomize(IRandom random, int startIndex, int length, DoubleMatrix bounds) {
      if (length > 0) {
        for (int i = startIndex; i < startIndex + length; i++) {
          double min = bounds[i % bounds.Rows, 0];
          double max = bounds[i % bounds.Rows, 1];
          array[i] = min + (max - min) * random.NextDouble();
        }
        OnReset();
      }
    }

    public void Randomize(IRandom random, double min, double max) {
      Randomize(random, 0, Length, min, max);
    }

    public void Randomize(IRandom random, DoubleMatrix bounds) {
      Randomize(random, 0, Length, bounds);
    }

    public double DotProduct(RealVector other) {
      if (other.Length != Length) throw new ArgumentException("Vectors are of unequal length.");
      var dotProd = 0.0;
      for (var i = 0; i < Length; i++)
        dotProd += this[i] * other[i];
      return dotProd;
    }
  }
}
