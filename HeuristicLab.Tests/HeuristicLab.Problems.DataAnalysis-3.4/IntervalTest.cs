using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {
  [TestClass]
  public class IntervalTest {
    private readonly Interval a = new Interval(-1, 1);
    private readonly Interval b = new Interval(-2, 2);
    private readonly Interval c = new Interval(0, 3);
    private readonly Interval d = new Interval(1, 3);
    private readonly Interval e = new Interval(4, 6);

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void AddIntervalTest() {
      //add        [x1,x2] + [y1,y2] = [x1 + y1,x2 + y2]

      // [-1,1] + [-2,2] = [-3,3]
      Assert.AreEqual(Interval.Add(a, b), new Interval(-3, 3));
      //([-1, 1] + [-2, 2]) + [0, 3] = [-3, 6]
      Assert.AreEqual(Interval.Add(Interval.Add(a, b), c), new Interval(-3, 6));
      //([-1, 1] + [0, 3]) + [-2, 2] = [-3, 6]
      Assert.AreEqual(Interval.Add(Interval.Add(a, c), b), new Interval(-3, 6));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SubtractIntervalTest() {
      //subtract   [x1,x2] − [y1,y2] = [x1 − y2,x2 − y1]

      //[-1, 1] - [-2, 2] = [-3, 3]
      Assert.AreEqual<Interval>(Interval.Subtract(a, b), new Interval(-3, 3));
      //([-1, 1] - [-2, 2]) - [0, 3] = [-6, 3]
      Assert.AreEqual<Interval>(Interval.Subtract(Interval.Subtract(a, b), c), new Interval(-6, 3));
      //([-1, 1] - [0, 3]) - [-2, 2] = [-6, 3]
      Assert.AreEqual<Interval>(Interval.Subtract(Interval.Subtract(a, c), b), new Interval(-6, 3));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MultiplyIntervalTest() {
      //multiply   [x1,x2] * [y1,y2] = [min(x1*y1,x1*y2,x2*y1,x2*y2),max(x1*y1,x1*y2,x2*y1,x2*y2)]

      //[-1, 1] * [-2, 2] = [-2, 2]
      Assert.AreEqual<Interval>(Interval.Multiply(a, b), new Interval(-2, 2));
      //([-1, 1] * [-2, 2]) * [0, 3] = [-6, 6]
      Assert.AreEqual<Interval>(Interval.Multiply(Interval.Multiply(a, b), c), new Interval(-6, 6));
      //([-1, 1] * [0, 3]) * [-2, 2] = [-6, 6]
      Assert.AreEqual<Interval>(Interval.Multiply(Interval.Multiply(a, c), b), new Interval(-6, 6));

      // [-2, 0] * [-2, 0]  = [0, 4]
      Assert.AreEqual<Interval>(new Interval(0, 4), Interval.Multiply(new Interval(-2, 0), new Interval(-2, 0)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void DivideIntervalTest() {
      //divide  [x1, x2] / [y1, y2] = [x1, x2] * (1/[y1, y2]), where 1 / [y1,y2] = [1 / y2,1 / y1] if 0 not in [y_1, y_2].

      //[4, 6] / [1, 3] = [4/3, 6]
      Assert.AreEqual<Interval>(Interval.Divide(e, d), new Interval(4.0 / 3.0, 6));
      //([4, 6] / [1, 3]) / [1, 3] = [4/9, 6]
      Assert.AreEqual<Interval>(Interval.Divide(Interval.Divide(e, d), d), new Interval(4.0 / 9.0, 6));
      //[4, 6] / [0, 3] = [4/3, +Inf]
      Assert.AreEqual<Interval>(Interval.Divide(e, c), new Interval(4.0 / 3.0, double.PositiveInfinity));
      //[-1, 1] / [0, 3] = [+Inf, -Inf]
      Assert.AreEqual<Interval>(Interval.Divide(a, c), new Interval(double.NegativeInfinity, double.PositiveInfinity));
      //Devision by 0 ==> IsInfiniteOrUndefined == true
      Assert.IsTrue(Interval.Divide(e, c).IsInfiniteOrUndefined);
      //Devision by 0 ==> IsInfiniteOrUndefined == true
      Assert.IsTrue(Interval.Divide(a, c).IsInfiniteOrUndefined);
      Assert.AreEqual<Interval>(Interval.Divide(d, b), new Interval(double.NegativeInfinity, double.PositiveInfinity));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SineIntervalTest() {
      //sine depends on interval
      //sin([0, 2*pi]) = [-1, 1]
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(0, 2 * Math.PI)), new Interval(-1, 1));
      //sin([-pi/2, pi/2]) = [sin(-pi/2), sin(pi/2)]
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(-1 * Math.PI / 2, Math.PI / 2)), new Interval(-1, 1));
      //sin([0, pi/2]) = [sin(0), sin(pi/2)]
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(0, Math.PI / 2)), new Interval(0, 1));
      //sin([pi, 3*pi/2]) = [sin(pi), sin(3*pi/2)]
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(Math.PI, 3 * Math.PI / 2)), new Interval(-1, 0));
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(1, 2)), new Interval(Math.Min(Math.Sin(1), Math.Sin(2)), 1));
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(1, 3)), new Interval(Math.Min(Math.Sin(1), Math.Sin(3)), 1));
      Assert.AreEqual<Interval>(Interval.Sine(new Interval(Math.PI, 5 * Math.PI / 2)), new Interval(-1, 1));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CosineIntervalTest() {
      //Cosine uses sine Interval.Sine(Interval.Subtract(a, new Interval(Math.PI / 2, Math.PI / 2)));
      Assert.AreEqual<Interval>(Interval.Cosine(new Interval(0, 2 * Math.PI)), new Interval(-1, 1));
      Assert.AreEqual<Interval>(new Interval(-1, 1), Interval.Cosine(new Interval(Math.PI, 4 * Math.PI / 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void LogIntervalTest() {
      //Log([3, 5]) = [log(3), log(5)]
      Assert.AreEqual<Interval>(new Interval(Math.Log(3), Math.Log(5)), Interval.Logarithm(new Interval(3, 5)));
      //Log([0.5, 1]) = [log(0.5), log(1)]
      Assert.AreEqual<Interval>(new Interval(Math.Log(0.5), 0), Interval.Logarithm(new Interval(0.5, 1)));
      //Log([-1, 5]) = [NaN, log(5)]
      var result = Interval.Logarithm(new Interval(-1, 5));
      Assert.AreEqual<Interval>(new Interval(double.NaN, Math.Log(5)),result);
      Assert.IsTrue(result.IsInfiniteOrUndefined);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void ExponentialIntervalTest() {
      //Exp([0, 1]) = [exp(0), exp(1)]
      Assert.AreEqual<Interval>(new Interval(1, Math.Exp(1)), Interval.Exponential(new Interval(0, 1)));
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SquareIntervalTest() {
      Assert.AreEqual<Interval>(new Interval(1, 4), Interval.Square(new Interval(1, 2)));
      Assert.AreEqual<Interval>(new Interval(1, 4), Interval.Square(new Interval(-2, -1)));
      Assert.AreEqual<Interval>(new Interval(0, 4), Interval.Square(new Interval(-2, 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SquarerootIntervalTest() {
      Assert.AreEqual<Interval>(new Interval(1, 2), Interval.SquareRoot(new Interval(1, 4)));
      Assert.AreEqual<Interval>(new Interval(double.NaN, double.NaN), Interval.SquareRoot(new Interval(-4, -1)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CubeIntervalTest() {
      Assert.AreEqual<Interval>(new Interval(1, 8), Interval.Cube(new Interval(1, 2)));
      Assert.AreEqual<Interval>(new Interval(-8, -1), Interval.Cube(new Interval(-2, -1)));
      Assert.AreEqual<Interval>(new Interval(-8, 8), Interval.Cube(new Interval(-2, 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CubeRootIntervalTest() {
      Assert.AreEqual<Interval>(new Interval(1, 2), Interval.CubicRoot(new Interval(1, 8)));
      Assert.AreEqual<Interval>(new Interval(-2, -2), Interval.CubicRoot(new Interval(-8, -8)));
      Assert.AreEqual<Interval>(new Interval(-2, 2), Interval.CubicRoot(new Interval(-8, 8)));
      Assert.AreEqual(new Interval(2, 2), Interval.CubicRoot(new Interval(8, 8)));
      Assert.AreEqual(new Interval(-Math.Pow(6, 1.0 / 3), 2), Interval.CubicRoot(new Interval(-6, 8)));
      Assert.AreEqual(new Interval(2, 2), Interval.CubicRoot(new Interval(8, 8)));
      Assert.AreEqual(new Interval(-2, 0), Interval.CubicRoot(new Interval(-8, 0)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void AbsoluteIntervalTest() {
      Assert.AreEqual(new Interval(2, 5), Interval.Absolute(new Interval(-5, -2)));
      Assert.AreEqual(new Interval(2, 5), Interval.Absolute(new Interval(2, 5)));
      Assert.AreEqual(new Interval(0, 3), Interval.Absolute(new Interval(-3, 0)));
      Assert.AreEqual(new Interval(0, 5), Interval.Absolute(new Interval(0, 5)));
      Assert.AreEqual(new Interval(0, 5), Interval.Absolute(new Interval(-2, 5)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void AnalyticalQuotientIntervalTest() {
      //Analytical Quotient ==> a / sqrt(b^2 + 1)
      var aPos = new Interval(3, 5);
      var aZero = new Interval(-3, 5);
      var aNeg = new Interval(-5, -3);

      var bPos = new Interval(2, 4);
      var bZero = new Interval(-2, 4);
      var bNeg = new Interval(-4, -2);

      //Second interval goes over zero
      Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound), Interval.AnalyticalQuotient(aPos, bZero));
      Assert.AreEqual(new Interval(aZero.LowerBound, aZero.UpperBound), Interval.AnalyticalQuotient(aZero, bZero));
      Assert.AreEqual(new Interval(aNeg.LowerBound, aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bZero));
      //Second interval is positive
      Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aPos, bPos));
      Assert.AreEqual(new Interval(aZero.LowerBound/Math.Sqrt(5), aZero.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aZero, bPos));
      Assert.AreEqual(new Interval(aNeg.LowerBound/Math.Sqrt(5), aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bPos));
      //Second interval is negative
      Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aPos, bNeg));
      Assert.AreEqual(new Interval(aZero.LowerBound/Math.Sqrt(5), aZero.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aZero, bNeg));
      Assert.AreEqual(new Interval(aNeg.LowerBound/Math.Sqrt(5), aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bNeg));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void IsNegativeIntervalTest() {
      Assert.IsTrue(new Interval(-2, -1).IsNegative);
      Assert.IsFalse(new Interval(-2, 0).IsNegative);
      Assert.IsFalse(new Interval(-2, 2).IsNegative);
      Assert.IsFalse(new Interval(2, 4).IsNegative);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void IsPositiveIntervalTest() {
      Assert.IsTrue(new Interval(3, 5).IsPositive);
      Assert.IsFalse(new Interval(0, 5).IsPositive);
      Assert.IsFalse(new Interval(-1, 5).IsPositive);
      Assert.IsFalse(new Interval(-5, -2).IsPositive);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void IsAlmostIntervalTest() {
      var negativeLowerBound = -2E-13;
      var negativeUpperBound = -1E-13;
      var positiveLowerBound = 3E-13;
      var positiveUpperBound = 5E-13;

      var negativeInterval = new Interval(negativeLowerBound, negativeUpperBound);
      var positiveInterval = new Interval(positiveLowerBound, positiveUpperBound);
      var zeroInterval = new Interval(negativeUpperBound, positiveLowerBound);

      //Check for right-shift of negative interval
      Assert.AreEqual(negativeUpperBound, negativeInterval.LowerBound);
      Assert.AreEqual(negativeUpperBound, negativeInterval.UpperBound);
      //Check for left-shift of positive interval
      Assert.AreEqual(positiveLowerBound, positiveInterval.LowerBound);
      Assert.AreEqual(positiveLowerBound, positiveInterval.UpperBound);
      //Check for setting interval to zero
      Assert.AreEqual(0, zeroInterval.LowerBound);
      Assert.AreEqual(0, zeroInterval.UpperBound);

    }
  }
}
