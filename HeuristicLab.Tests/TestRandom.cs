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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Tests {
  public class TestRandom : IRandom {
    #region Variables and Properties
    private int[] intNumbers;
    public int[] IntNumbers {
      get { return intNumbers; }
      set {
        if (value == null) intNumbers = new int[0];
        else intNumbers = value;
      }
    }
    private int nextIntIndex;
    private double[] doubleNumbers;
    public double[] DoubleNumbers {
      get { return doubleNumbers; }
      set {
        if (value == null) doubleNumbers = new double[0];
        else doubleNumbers = value;
      }
    }
    private int nextDoubleIndex;
    #endregion

    public TestRandom() {
      intNumbers = new int[0];
      doubleNumbers = new double[0];
      nextIntIndex = 0;
      nextDoubleIndex = 0;
    }

    protected TestRandom(TestRandom original, Cloner cloner) {
      this.intNumbers = original.intNumbers.ToArray();
      this.doubleNumbers = original.doubleNumbers.ToArray();
    }

    public TestRandom(int[] intNumbers, double[] doubleNumbers) {
      if (intNumbers == null) this.intNumbers = new int[0];
      else this.intNumbers = intNumbers;
      if (doubleNumbers == null) this.doubleNumbers = new double[0];
      else this.doubleNumbers = doubleNumbers;
      nextIntIndex = 0;
      nextDoubleIndex = 0;
    }

    #region IRandom Members

    public void Reset() {
      nextIntIndex = 0;
      nextDoubleIndex = 0;
    }

    public void Reset(int seed) {
      throw new NotImplementedException();
    }

    public int Next() {
      if (nextIntIndex >= intNumbers.Length) throw new InvalidOperationException("Random: No more integer random numbers available");
      return intNumbers[nextIntIndex++];
    }

    public int Next(int maxVal) {
      if (nextIntIndex >= intNumbers.Length) throw new InvalidOperationException("Random: No more integer random numbers available");
      if (IntNumbers[nextIntIndex] >= maxVal) throw new InvalidOperationException("Random: Next integer random number (" + IntNumbers[nextIntIndex] + ") is >= " + maxVal);
      return intNumbers[nextIntIndex++];
    }

    public int Next(int minVal, int maxVal) {
      if (nextIntIndex >= intNumbers.Length) throw new InvalidOperationException("Random: No more integer random numbers available");
      if (IntNumbers[nextIntIndex] < minVal || IntNumbers[nextIntIndex] >= maxVal) throw new InvalidOperationException("Random: Next integer random number (" + IntNumbers[nextIntIndex] + ") is not in the range [" + minVal + ";" + maxVal + ")");
      return intNumbers[nextIntIndex++];
    }

    public double NextDouble() {
      if (nextDoubleIndex >= doubleNumbers.Length) throw new InvalidOperationException("Random: No more double random numbers available");
      if (doubleNumbers[nextDoubleIndex] < 0.0 || doubleNumbers[nextDoubleIndex] >= 1.0) throw new InvalidOperationException("Random: Next double ranomd number (" + DoubleNumbers[nextDoubleIndex] + ") is not in the range [0;1)");
      return doubleNumbers[nextDoubleIndex++];
    }

    #endregion

    #region IItem Members

    public string ItemName {
      get { throw new NotImplementedException(); }
    }

    public string ItemDescription {
      get { throw new NotImplementedException(); }
    }

    public Version ItemVersion {
      get { throw new NotImplementedException(); }
    }

    public System.Drawing.Image ItemImage {
      get { throw new NotImplementedException(); }
    }

#pragma warning disable 67
    public event EventHandler ItemImageChanged;
    public event EventHandler ToStringChanged;
#pragma warning restore 67
    #endregion

    #region IDeepCloneable Members

    public IDeepCloneable Clone(Cloner cloner) {
      return new TestRandom(this, cloner);
    }

    #endregion

    #region ICloneable Members

    public object Clone() {
      throw new NotImplementedException();
    }

    #endregion
  }
}
