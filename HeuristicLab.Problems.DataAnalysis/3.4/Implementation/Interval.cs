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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class Interval : IEquatable<Interval> {
    public double LowerBound { get; private set; }
    public double UpperBound { get; private set; }

    public Interval(double lowerBound, double upperBound) {
      if (lowerBound > upperBound)
        throw new ArgumentException("LowerBound must be smaller than UpperBound.");

      this.LowerBound = lowerBound;
      this.UpperBound = upperBound;
    }

    public bool Contains(double value) {
      return LowerBound <= value && value <= UpperBound;
    }

    public override string ToString() {
      return "Interval: [" + LowerBound + ", " + UpperBound + "]";
    }

    public bool IsInfiniteOrUndefined {
      get {
        return double.IsInfinity(LowerBound) || double.IsInfinity(UpperBound) ||
                double.IsNaN(LowerBound) || double.IsNaN(UpperBound);
      }
    }

    public static Interval GetInterval(IEnumerable<double> values) {
      if (values == null) throw new ArgumentNullException("values");
      if (!values.Any()) throw new ArgumentException($"No values are present.");

      var min = double.MaxValue;
      var max = double.MinValue;

      foreach (var value in values) {
        //If an value is NaN return an interval [NaN, NaN]
        if (double.IsNaN(value)) return new Interval(double.NaN, double.NaN);

        if (value < min) min = value;
        if (value > max) max = value;
      }

      return new Interval(min, max);
    }

    #region Equals, GetHashCode, == , !=
    public bool Equals(Interval other) {
      if (other == null)
        return false;

      return (UpperBound.IsAlmost(other.UpperBound) || (double.IsNaN(UpperBound) && double.IsNaN(other.UpperBound)))
        && (LowerBound.IsAlmost(other.LowerBound) || (double.IsNaN(LowerBound) && double.IsNaN(other.LowerBound)));
    }

    public override bool Equals(object obj) {
      return Equals(obj as Interval);
    }

    public override int GetHashCode() {
      return LowerBound.GetHashCode() ^ UpperBound.GetHashCode();
    }

    public static bool operator ==(Interval interval1, Interval interval2) {
      if (ReferenceEquals(interval1, null)) return ReferenceEquals(interval2, null);
      return interval1.Equals(interval2);
    }
    public static bool operator !=(Interval interval1, Interval interval2) {
      return !(interval1 == interval2);
    }
    #endregion

    #region operations

    // [x1,x2] + [y1,y2] = [x1 + y1,x2 + y2]
    public static Interval Add(Interval a, Interval b) {
      return new Interval(a.LowerBound + b.LowerBound, a.UpperBound + b.UpperBound);
    }

    // [x1,x2] − [y1,y2] = [x1 − y2,x2 − y1]
    public static Interval Subtract(Interval a, Interval b) {
      return new Interval(a.LowerBound - b.UpperBound, a.UpperBound - b.LowerBound);
    }

    // [x1,x2] * [y1,y2] = [min(x1*y1,x1*y2,x2*y1,x2*y2),max(x1*y1,x1*y2,x2*y1,x2*y2)]
    public static Interval Multiply(Interval a, Interval b) {
      double v1 = a.LowerBound * b.LowerBound;
      double v2 = a.LowerBound * b.UpperBound;
      double v3 = a.UpperBound * b.LowerBound;
      double v4 = a.UpperBound * b.UpperBound;

      double min = Math.Min(Math.Min(v1, v2), Math.Min(v3, v4));
      double max = Math.Max(Math.Min(v1, v2), Math.Max(v3, v4));
      return new Interval(min, max);
    }

    //mkommend: Division by intervals containing 0 is implemented as defined in
    //http://en.wikipedia.org/wiki/Interval_arithmetic
    public static Interval Divide(Interval a, Interval b) {
      if (b.Contains(0.0)) {
        if (b.LowerBound.IsAlmost(0.0)) return Interval.Multiply(a, new Interval(1.0 / b.UpperBound, double.PositiveInfinity));
        else if (b.UpperBound.IsAlmost(0.0)) return Interval.Multiply(a, new Interval(double.NegativeInfinity, 1.0 / b.LowerBound));
        else return new Interval(double.NegativeInfinity, double.PositiveInfinity);
      }
      return Interval.Multiply(a, new Interval(1.0 / b.UpperBound, 1.0 / b.LowerBound));
    }

    public static Interval Sine(Interval a) {
      if (Math.Abs(a.UpperBound - a.LowerBound) >= Math.PI * 2) return new Interval(-1, 1);

      //divide the interval by PI/2 so that the optima lie at x element of N (0,1,2,3,4,...)
      double Pihalf = Math.PI / 2;
      Interval scaled = Interval.Divide(a, new Interval(Pihalf, Pihalf));
      //move to positive scale
      if (scaled.LowerBound < 0) {
        int periodsToMove = Math.Abs((int)scaled.LowerBound / 4) + 1;
        scaled = Interval.Add(scaled, new Interval(periodsToMove * 4, periodsToMove * 4));
      }

      double scaledLowerBound = scaled.LowerBound % 4.0;
      double scaledUpperBound = scaled.UpperBound % 4.0;
      if (scaledUpperBound < scaledLowerBound) scaledUpperBound += 4.0;
      List<double> sinValues = new List<double>();
      sinValues.Add(Math.Sin(scaledLowerBound * Pihalf));
      sinValues.Add(Math.Sin(scaledUpperBound * Pihalf));

      int startValue = (int)Math.Ceiling(scaledLowerBound);
      while (startValue < scaledUpperBound) {
        sinValues.Add(Math.Sin(startValue * Pihalf));
        startValue += 1;
      }

      return new Interval(sinValues.Min(), sinValues.Max());
    }
    public static Interval Cosine(Interval a) {
      return Interval.Sine(Interval.Subtract(a, new Interval(Math.PI / 2, Math.PI / 2)));
    }
    public static Interval Tangens(Interval a) {
      return Interval.Divide(Interval.Sine(a), Interval.Cosine(a));
    }

    public static Interval Logarithm(Interval a) {
      return new Interval(Math.Log(a.LowerBound), Math.Log(a.UpperBound));
    }
    public static Interval Exponential(Interval a) {
      return new Interval(Math.Exp(a.LowerBound), Math.Exp(a.UpperBound));
    }

    public static Interval Power(Interval a, Interval b) {
      if (a.Contains(0.0) && b.LowerBound < 0) return new Interval(double.NaN, double.NaN);

      int bLower = (int)Math.Round(b.LowerBound);
      int bUpper = (int)Math.Round(b.UpperBound);

      List<double> powerValues = new List<double>();
      powerValues.Add(Math.Pow(a.UpperBound, bUpper));
      powerValues.Add(Math.Pow(a.UpperBound, bUpper - 1));
      powerValues.Add(Math.Pow(a.UpperBound, bLower));
      powerValues.Add(Math.Pow(a.UpperBound, bLower + 1));

      powerValues.Add(Math.Pow(a.LowerBound, bUpper));
      powerValues.Add(Math.Pow(a.LowerBound, bUpper - 1));
      powerValues.Add(Math.Pow(a.LowerBound, bLower));
      powerValues.Add(Math.Pow(a.LowerBound, bLower + 1));

      return new Interval(powerValues.Min(), powerValues.Max());
    }

    public static Interval Square(Interval a) {
      return Power(a, new Interval(2, 2));
    }

    public static Interval Cubic(Interval a) {
      return Power(a, new Interval(3, 3));
    }

    public static Interval Root(Interval a, Interval b) {
      int lower = (int)Math.Round(b.LowerBound);
      int higher = (int)Math.Round(b.UpperBound);

      return new Interval(Math.Pow(a.LowerBound, 1.0 / higher), Math.Pow(a.UpperBound, 1.0 / lower));
    }

    public static Interval SquareRoot(Interval a) {
      return Root(a, new Interval(2, 2));
    }

    public static Interval CubicRoot(Interval a) {
      return Root(a, new Interval(3, 3));
    }
    #endregion
  }
}
