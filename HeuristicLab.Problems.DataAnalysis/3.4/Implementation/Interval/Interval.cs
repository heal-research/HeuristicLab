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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("849e42d3-8934-419d-9aff-64ad81c06b67")]
  public class Interval : IEquatable<Interval> {
    [Storable]
    public double LowerBound { get; private set; }
    [Storable]
    public double UpperBound { get; private set; }

    public double Width => UpperBound - LowerBound;

    [StorableConstructor]
    protected Interval(StorableConstructorFlag _) { }

    /// <summary>
    /// Creates an interval with given bounds, where lower bound must be smaller than
    /// the upper bound. Floating point precision errors trough calculations are fixed by,
    /// checking if the intervals are almost the same (E-12). If this is the case, the bounds 
    /// will be set to the bound closer to zero.
    /// </summary>
    /// <param name="lowerBound">Lower bound of the interval</param>
    /// <param name="upperBound">Upper bound of the interval</param>
    public Interval(double lowerBound, double upperBound) {
      if (lowerBound.IsAlmost(upperBound)) {
        //If the bounds go over zero
        if (lowerBound <= 0 && upperBound >= 0) {
          lowerBound = 0.0;
          upperBound = 0.0;
          //Interval is negative
        } else if (upperBound < 0) {
          lowerBound = upperBound;
          //Interval is positive
        } else {
          upperBound = lowerBound;
        }
      }

      if (lowerBound > upperBound)
        throw new ArgumentException("lowerBound must be smaller than or equal to upperBound.");

      this.LowerBound = lowerBound;
      this.UpperBound = upperBound;
    }

    private Interval(double v) : this(v, v) { }

    public bool Contains(double value) {
      return LowerBound <= value && value <= UpperBound;
    }

    public bool Contains(Interval other) {
      if (double.IsNegativeInfinity(LowerBound) && double.IsPositiveInfinity(UpperBound)) return true;
      if (other.LowerBound >= LowerBound && other.UpperBound <= UpperBound) return true;
  
      return false;
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

    /// <summary>
    /// True if the interval is positive without zero
    /// </summary>
    public bool IsPositive {
      get => LowerBound > 0.0; 
    }

    /// <summary>
    /// True if the interval is negative without zero
    /// </summary>
    public bool IsNegative {
      get => UpperBound < 0.0;
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

      return (UpperBound==other.UpperBound || (double.IsNaN(UpperBound) && double.IsNaN(other.UpperBound)))
        && (LowerBound==other.LowerBound || (double.IsNaN(LowerBound) && double.IsNaN(other.LowerBound)));
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
      double max = Math.Max(Math.Max(v1, v2), Math.Max(v3, v4));
      return new Interval(min, max);
    }

    //Division by intervals containing 0 is implemented as defined in
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
      return Interval.Sine(Interval.Add(a, new Interval(Math.PI / 2, Math.PI / 2)));
    }
    public static Interval Tangens(Interval a) {
      return Interval.Divide(Interval.Sine(a), Interval.Cosine(a));
    }
    public static Interval HyperbolicTangent(Interval a) {
      return new Interval(Math.Tanh(a.LowerBound), Math.Tanh(a.UpperBound));
    }

    public static Interval Logarithm(Interval a) {
      return new Interval(Math.Log(a.LowerBound), Math.Log(a.UpperBound));
    }
    public static Interval Exponential(Interval a) {
      return new Interval(Math.Exp(a.LowerBound), Math.Exp(a.UpperBound));
    }

    public static Interval Square(Interval a) {
      if (a.UpperBound <= 0) return new Interval(a.UpperBound * a.UpperBound, a.LowerBound * a.LowerBound);     // interval is negative
      else if (a.LowerBound >= 0) return new Interval(a.LowerBound * a.LowerBound, a.UpperBound * a.UpperBound); // interval is positive
      else return new Interval(0, Math.Max(a.LowerBound * a.LowerBound, a.UpperBound * a.UpperBound)); // interval goes over zero
    }

    public static Interval Cube(Interval a) {
      return new Interval(Math.Pow(a.LowerBound, 3), Math.Pow(a.UpperBound, 3));
    }

    /// <summary>
    /// The interval contains both possible results of the calculated square root +-sqrt(x). That results in a wider
    /// interval, but it contains all possible solutions.
    /// </summary>
    /// <param name="a">Interval to build square root from.</param>
    /// <returns></returns>
    public static Interval SquareRoot(Interval a) {
      if (a.LowerBound < 0) return new Interval(double.NaN, double.NaN);
      return new Interval(-Math.Sqrt(a.UpperBound), Math.Sqrt(a.UpperBound));
    }

    public static Interval CubicRoot(Interval a) {
      var lower = (a.LowerBound < 0) ? -Math.Pow(-a.LowerBound, 1d / 3d) : Math.Pow(a.LowerBound, 1d / 3d);
      var upper = (a.UpperBound < 0) ? -Math.Pow(-a.UpperBound, 1d / 3d) : Math.Pow(a.UpperBound, 1d / 3d);

      return new Interval(lower, upper);
    }

    public static Interval Absolute(Interval a) {
      var absLower = Math.Abs(a.LowerBound);
      var absUpper = Math.Abs(a.UpperBound);
      var min = Math.Min(absLower, absUpper);
      var max = Math.Max(absLower, absUpper);

      if (a.Contains(0.0)) {
        min = 0.0;
      }

      return new Interval(min, max);
    }

    public static Interval AnalyticQuotient(Interval a, Interval b) {
      var dividend = a;
      var divisor = Add(Square(b), new Interval(1.0, 1.0));
      divisor = SquareRoot(divisor);

      var quotient = Divide(dividend, divisor);
      return quotient;
    }
    #endregion

    #region arithmetic overloads
    public static Interval operator +(Interval a, Interval b) => Add(a, b);
    public static Interval operator +(Interval a, double b) => Add(a, new Interval(b));
    public static Interval operator +(double a, Interval b) => Add(new Interval(a), b);
    public static Interval operator -(Interval a, Interval b) => Subtract(a, b);
    public static Interval operator -(Interval a, double b) => Subtract(a, new Interval(b));
    public static Interval operator -(double a, Interval b) => Subtract(new Interval(a), b);
    public static Interval operator -(Interval a) => Subtract(new Interval(0), a);
    public static Interval operator *(Interval a, Interval b) => Multiply(a, b);
    public static Interval operator *(Interval a, double b) => Multiply(a, new Interval(b));
    public static Interval operator *(double a, Interval b) => Multiply(new Interval(a), b);
    public static Interval operator /(Interval a, Interval b) => Divide(a, b);
    public static Interval operator /(Interval a, double b) => Divide(a, new Interval(b));
    public static Interval operator /(double a, Interval b) => Divide(new Interval(a), b);
    public static Interval Exponential(double a) { return Exponential(new Interval(a)); }
    public static Interval Logarithm(double a) { return Logarithm(new Interval(a)); }
    public static Interval Sine(double a) { return Sine(new Interval(a)); }
    public static Interval Cosine(double a) { return Cosine(new Interval(a)); }
    public static Interval Tangens(double a) { return Tangens(new Interval(a)); }
    public static Interval HyperbolicTangent(double a) { return HyperbolicTangent(new Interval(a)); }
    public static Interval Square(double a) { return Square(new Interval(a)); }
    public static Interval Cube(double a) { return Cube(new Interval(a)); }
    public static Interval SquareRoot(double a) { return SquareRoot(new Interval(a)); }
    public static Interval CubicRoot(double a) { return CubicRoot(new Interval(a)); }
    public static Interval Absolute(double a) { return Absolute(new Interval(a)); }
    public static Interval AnalyticQuotient(Interval a, double b) { return AnalyticQuotient(a, new Interval(b)); }
    public static Interval AnalyticQuotient(double a, Interval b) { return AnalyticQuotient(new Interval(a), b); }
    public static Interval AnalyticQuotient(double a, double b) { return AnalyticQuotient(new Interval(a), new Interval(b)); }
    #endregion
  }
}
