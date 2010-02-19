using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.Permutation.Tests {
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

    public TestRandom(int[] intNumbers, double[] doubleNumbers) {
      if (intNumbers == null) intNumbers = new int[0];
      else this.intNumbers = intNumbers;
      if (doubleNumbers == null) doubleNumbers = new double[0];
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

    public System.Drawing.Image ItemImage {
      get { throw new NotImplementedException(); }
    }

    public event ChangedEventHandler Changed;

    #endregion

    #region IDeepCloneable Members

    public IDeepCloneable Clone(Cloner cloner) {
      throw new NotImplementedException();
    }

    #endregion

    #region ICloneable Members

    public object Clone() {
      throw new NotImplementedException();
    }

    #endregion
  }
}
