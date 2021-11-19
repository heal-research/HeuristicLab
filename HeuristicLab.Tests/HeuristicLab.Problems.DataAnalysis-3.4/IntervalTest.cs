using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {
  [TestClass]
  public class IntervalTest {
    private readonly Interval a = new Interval(-1, 1);
    private readonly Interval b = new Interval(-2, 2);
    private readonly Interval c = new Interval(0, 3);
    private readonly Interval d = new Interval(1, 3);
    private readonly Interval e = new Interval(4, 6);

    private void CheckLowerAndUpperBoundOfInterval(Interval expected, Interval calculated) {
      var lowerBoundExpected = expected.LowerBound;
      var upperBoundExpected = expected.UpperBound;
      var lowerBoundCalculated = calculated.LowerBound;
      var upperBoundCalculated = calculated.UpperBound;

      if(double.IsNaN(lowerBoundExpected) && double.IsNaN(lowerBoundCalculated)) {
        Assert.IsTrue(double.IsNaN(lowerBoundExpected) && double.IsNaN(lowerBoundCalculated));
      } else if (double.IsNaN(upperBoundExpected) && double.IsNaN(upperBoundCalculated)) {
        Assert.IsTrue(double.IsNaN(upperBoundExpected) && double.IsNaN(upperBoundCalculated));
      } else {
        Assert.AreEqual(lowerBoundExpected, lowerBoundCalculated, 1e-9);
        Assert.AreEqual(upperBoundExpected, upperBoundCalculated, 1e-9);
      } 
    }

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
      CheckLowerAndUpperBoundOfInterval(Interval.Subtract(a, b), new Interval(-3, 3));
      //([-1, 1] - [-2, 2]) - [0, 3] = [-6, 3]
      CheckLowerAndUpperBoundOfInterval(Interval.Subtract(Interval.Subtract(a, b), c), new Interval(-6, 3));
      //([-1, 1] - [0, 3]) - [-2, 2] = [-6, 3]
      CheckLowerAndUpperBoundOfInterval(Interval.Subtract(Interval.Subtract(a, c), b), new Interval(-6, 3));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MultiplyIntervalTest() {
      //multiply   [x1,x2] * [y1,y2] = [min(x1*y1,x1*y2,x2*y1,x2*y2),max(x1*y1,x1*y2,x2*y1,x2*y2)]

      //[-1, 1] * [-2, 2] = [-2, 2]
      CheckLowerAndUpperBoundOfInterval(Interval.Multiply(a, b), new Interval(-2, 2));
      //([-1, 1] * [-2, 2]) * [0, 3] = [-6, 6]
      CheckLowerAndUpperBoundOfInterval(Interval.Multiply(Interval.Multiply(a, b), c), new Interval(-6, 6));
      //([-1, 1] * [0, 3]) * [-2, 2] = [-6, 6]
      CheckLowerAndUpperBoundOfInterval(Interval.Multiply(Interval.Multiply(a, c), b), new Interval(-6, 6));

      // [-2, 0] * [-2, 0]  = [0, 4]
      CheckLowerAndUpperBoundOfInterval(new Interval(0, 4), Interval.Multiply(new Interval(-2, 0), new Interval(-2, 0)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void DivideIntervalTest() {
      //divide  [x1, x2] / [y1, y2] = [x1, x2] * (1/[y1, y2]), where 1 / [y1,y2] = [1 / y2,1 / y1] if 0 not in [y_1, y_2].

      //[4, 6] / [1, 3] = [4/3, 6]
      CheckLowerAndUpperBoundOfInterval(Interval.Divide(e, d), new Interval(4.0 / 3.0, 6));
      //([4, 6] / [1, 3]) / [1, 3] = [4/9, 6]
      CheckLowerAndUpperBoundOfInterval(Interval.Divide(Interval.Divide(e, d), d), new Interval(4.0 / 9.0, 6));
      //[4, 6] / [0, 3] = [4/3, +Inf]
      CheckLowerAndUpperBoundOfInterval(Interval.Divide(e, c), new Interval(4.0 / 3.0, double.PositiveInfinity));
      //[-1, 1] / [0, 3] = [+Inf, -Inf]
      CheckLowerAndUpperBoundOfInterval(Interval.Divide(a, c), new Interval(double.NegativeInfinity, double.PositiveInfinity));
      //Devision by 0 ==> IsInfiniteOrUndefined == true
      Assert.IsTrue(Interval.Divide(e, c).IsInfiniteOrUndefined);
      //Devision by 0 ==> IsInfiniteOrUndefined == true
      Assert.IsTrue(Interval.Divide(a, c).IsInfiniteOrUndefined);
      CheckLowerAndUpperBoundOfInterval(Interval.Divide(d, b), new Interval(double.NegativeInfinity, double.PositiveInfinity));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SineIntervalTest() {
      //sine depends on interval
      //sin([0, 2*pi]) = [-1, 1]
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(0, 2 * Math.PI)), new Interval(-1, 1));
      //sin([-pi/2, pi/2]) = [sin(-pi/2), sin(pi/2)]
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(-1 * Math.PI / 2, Math.PI / 2)), new Interval(-1, 1));
      //sin([0, pi/2]) = [sin(0), sin(pi/2)]
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(0, Math.PI / 2)), new Interval(0, 1));
      //sin([pi, 3*pi/2]) = [sin(pi), sin(3*pi/2)]
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(Math.PI, 3 * Math.PI / 2)), new Interval(-1, 0));
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(1, 2)), new Interval(Math.Min(Math.Sin(1), Math.Sin(2)), 1));
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(1, 3)), new Interval(Math.Min(Math.Sin(1), Math.Sin(3)), 1));
      CheckLowerAndUpperBoundOfInterval(Interval.Sine(new Interval(Math.PI, 5 * Math.PI / 2)), new Interval(-1, 1));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CosineIntervalTest() {
      //Cosine uses sine Interval.Sine(Interval.Subtract(a, new Interval(Math.PI / 2, Math.PI / 2)));
      CheckLowerAndUpperBoundOfInterval(Interval.Cosine(new Interval(0, 2 * Math.PI)), new Interval(-1, 1));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, 1), Interval.Cosine(new Interval(Math.PI, 4 * Math.PI / 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void LogIntervalTest() {
      //Log([3, 5]) = [log(3), log(5)]
      CheckLowerAndUpperBoundOfInterval(new Interval(Math.Log(3), Math.Log(5)), Interval.Logarithm(new Interval(3, 5)));
      //Log([0.5, 1]) = [log(0.5), log(1)]
      CheckLowerAndUpperBoundOfInterval(new Interval(Math.Log(0.5), 0), Interval.Logarithm(new Interval(0.5, 1)));
      //Log([-1, 5]) = [NaN, log(5)]
      var result = Interval.Logarithm(new Interval(-1, 5));
      CheckLowerAndUpperBoundOfInterval(new Interval(double.NaN, Math.Log(5)),result);
      Assert.IsTrue(result.IsInfiniteOrUndefined);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void ExponentialIntervalTest() {
      //Exp([0, 1]) = [exp(0), exp(1)]
      CheckLowerAndUpperBoundOfInterval(new Interval(1, Math.Exp(1)), Interval.Exponential(new Interval(0, 1)));
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SquareIntervalTest() {
      CheckLowerAndUpperBoundOfInterval(new Interval(1, 4), Interval.Square(new Interval(1, 2)));
      CheckLowerAndUpperBoundOfInterval(new Interval(1, 4), Interval.Square(new Interval(-2, -1)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0, 4), Interval.Square(new Interval(-2, 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void SquarerootIntervalTest() {
      CheckLowerAndUpperBoundOfInterval(new Interval(-2, 2), Interval.SquareRoot(new Interval(1, 4)));
      CheckLowerAndUpperBoundOfInterval(new Interval(double.NaN, double.NaN), Interval.SquareRoot(new Interval(-4, -1)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CubeIntervalTest() {
      CheckLowerAndUpperBoundOfInterval(new Interval(1, 8), Interval.Cube(new Interval(1, 2)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-8, -1), Interval.Cube(new Interval(-2, -1)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-8, 8), Interval.Cube(new Interval(-2, 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CubeRootIntervalTest() {
      CheckLowerAndUpperBoundOfInterval(new Interval(1, 2), Interval.CubicRoot(new Interval(1, 8)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-2, -2), Interval.CubicRoot(new Interval(-8, -8)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-2, 2), Interval.CubicRoot(new Interval(-8, 8)));
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
      //Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound), Interval.AnalyticalQuotient(aPos, bZero));
      //Assert.AreEqual(new Interval(aZero.LowerBound, aZero.UpperBound), Interval.AnalyticalQuotient(aZero, bZero));
      //Assert.AreEqual(new Interval(aNeg.LowerBound, aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bZero));
      ////Second interval is positive
      //Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aPos, bPos));
      //Assert.AreEqual(new Interval(aZero.LowerBound/Math.Sqrt(5), aZero.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aZero, bPos));
      //Assert.AreEqual(new Interval(aNeg.LowerBound/Math.Sqrt(5), aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bPos));
      ////Second interval is negative
      //Assert.AreEqual(new Interval(aPos.LowerBound/Math.Sqrt(17), aPos.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aPos, bNeg));
      //Assert.AreEqual(new Interval(aZero.LowerBound/Math.Sqrt(5), aZero.UpperBound/Math.Sqrt(5)), Interval.AnalyticalQuotient(aZero, bNeg));
      //Assert.AreEqual(new Interval(aNeg.LowerBound/Math.Sqrt(5), aNeg.UpperBound/Math.Sqrt(17)), Interval.AnalyticalQuotient(aNeg, bNeg));
      Assert.AreEqual(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.AnalyticQuotient(aPos, bZero));
      Assert.AreEqual(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.AnalyticQuotient(aPos, bPos));
      Assert.AreEqual(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.AnalyticQuotient(aZero, bNeg));
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

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void ContaintsTest() {
      var negativeInterval = new Interval(-10, -5);
      var positiveInterval = new Interval(5, 10);
      var overZeroInterval = new Interval(-5, 5);

      //Tests for negative intervals
      Assert.AreEqual(true, negativeInterval.Contains(new Interval(-9, -7)));
      Assert.AreEqual(false, negativeInterval.Contains(new Interval(-11, -3)));
      Assert.AreEqual(false, negativeInterval.Contains(positiveInterval));
      Assert.AreEqual(false, negativeInterval.Contains(overZeroInterval));
      Assert.AreEqual(true, negativeInterval.Contains(-8));
      Assert.AreEqual(false, negativeInterval.Contains(-12));
      Assert.AreEqual(false, negativeInterval.Contains(0));
      //Testes for positive intervals
      Assert.AreEqual(true, positiveInterval.Contains(new Interval(6, 10)));
      Assert.AreEqual(false, positiveInterval.Contains(new Interval(6, 12)));
      Assert.AreEqual(false, positiveInterval.Contains(negativeInterval));
      Assert.AreEqual(false, positiveInterval.Contains(overZeroInterval));
      Assert.AreEqual(true, positiveInterval.Contains(7));
      Assert.AreEqual(false, positiveInterval.Contains(11));
      Assert.AreEqual(false, positiveInterval.Contains(0));
      //Tests for over zero intervals
      Assert.AreEqual(true, overZeroInterval.Contains(new Interval(-3, 3)));
      Assert.AreEqual(true, overZeroInterval.Contains(new Interval(-4, -1)));
      Assert.AreEqual(true, overZeroInterval.Contains(new Interval(1, 5)));
      Assert.AreEqual(false, overZeroInterval.Contains(new Interval(-6, 0)));
      Assert.AreEqual(false, overZeroInterval.Contains(new Interval(0, 6)));
      Assert.AreEqual(false, overZeroInterval.Contains(new Interval(-7, 7)));
      Assert.AreEqual(true, overZeroInterval.Contains(-3));
      Assert.AreEqual(true, overZeroInterval.Contains(0));
      Assert.AreEqual(true, overZeroInterval.Contains(3));
      Assert.AreEqual(false, overZeroInterval.Contains(12));
      Assert.AreEqual(false, overZeroInterval.Contains(-7));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void GetIntervalTest() {
      var values = new List<double>() { -2.5, -9, 2, 7, 0 ,12, 12.4, 12.4, 9.29, 1002, -29.9};
      var valuesNan = new List<double>() { double.NaN, 2, 4, 19, -2, -12.2};
      var valuesInf = new List<double>() {double.NegativeInfinity, double.PositiveInfinity, 12, 2, -2, -12.2};

      var valuesInterval = new Interval(-29.9, 1002);
      var valuesNanInterval = new Interval(double.NaN, double.NaN);
      var valuesInfInterval = new Interval(double.NegativeInfinity, double.PositiveInfinity);


      Assert.AreEqual(valuesInterval, Interval.GetInterval(values));
      Assert.AreEqual(valuesNanInterval, Interval.GetInterval(valuesNan));
      Assert.AreEqual(valuesInfInterval, Interval.GetInterval(valuesInf));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void GeometricTest() {
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, -0.936456687290796), Interval.Cosine(new Interval(3, 3.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, -0.936456687290796), Interval.Cosine(new Interval(-3.5, -3)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, 1), Interval.Cosine(new Interval(-3.5, 3)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-0.839071529076452, 0.843853958732493), Interval.Cosine(new Interval(10, 12)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0.136737218207833, 0.907446781450197), Interval.Cosine(new Interval(13, 14)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-0.839071529076452, 1), Interval.Cosine(new Interval(10, 14)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, 0.136737218207833), Interval.Cosine(new Interval(14, 16)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-0.839071529076452, 0.004425697988051), Interval.Cosine(new Interval(-11, -10)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0.136737218207833, 0.907446781450197), Interval.Cosine(new Interval(-14, -13)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, 0.136737218207833), Interval.Cosine(new Interval(-16, -14)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0.101585703696621, 1), Interval.Cosine(new Interval(-102, -100)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1, 1), Interval.Cosine(new Interval(4.6e15, 4.7e15)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0.87758256189037265, 0.87758256189037276), Interval.Cosine(new Interval(0.5, 0.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-0.09904103659872825, 0.8775825618903728), Interval.Cosine(new Interval(0.5, 1.67)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 0.77556587851025016), Interval.Cosine(new Interval(2.1, 5.6)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 1.0), Interval.Cosine(new Interval(0.5, 8.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, -0.09904103659872801), Interval.Cosine(new Interval(1.67, 3.2)));

      CheckLowerAndUpperBoundOfInterval(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Tangens(new Interval(double.NegativeInfinity, double.PositiveInfinity)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0, 1.55740772465490223051), Interval.Tangens(new Interval(0, 1)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.55740772465490223051, 0), Interval.Tangens(new Interval(-1, 0)));
      CheckLowerAndUpperBoundOfInterval(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Tangens(new Interval(-2, -1)));
      CheckLowerAndUpperBoundOfInterval(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Tangens(new Interval(202, 203)));
      CheckLowerAndUpperBoundOfInterval(new Interval(0.54630248984379048, 0.5463024898437906), Interval.Tangens(new Interval(0.5, 0.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Tangens(new Interval(0.5,
        1.67)));

      CheckLowerAndUpperBoundOfInterval(new Interval(double.NegativeInfinity, double.PositiveInfinity), Interval.Tangens(new Interval(
        6.638314112824137, 8.38263151220128)));

      CheckLowerAndUpperBoundOfInterval(new Interval(0.47942553860420295, 0.47942553860420301), Interval.Sine(new Interval(0.5, 0.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(4.7942553860420295e-01, 1.0), Interval.Sine(new Interval(0.5, 1.67)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-5.8374143427580093e-02, 9.9508334981018021e-01), Interval.Sine(new Interval(1.67,
        3.2)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 0.863209366648874), Interval.Sine(new Interval(2.1, 5.6)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 1.0), Interval.Sine(new Interval(0.5, 8.5)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 0.9775301176650971), Interval.Sine(new Interval(-4.5, 0.1)));
      CheckLowerAndUpperBoundOfInterval(new Interval(-1.0, 1.0), Interval.Sine(new Interval(1.3, 6.3)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void EqualsTest() {
      var interval1 = new Interval(0, 12);
      var interval2 = new Interval(-12, 8);
      var interval3 = new Interval(double.NegativeInfinity, 0);

      Assert.AreEqual(true, interval1.Equals(new Interval(0, 12)));
      Assert.AreEqual(false, interval1.Equals(interval2));
      Assert.AreEqual(true, interval3 == new Interval(double.NegativeInfinity, 0));
      Assert.AreEqual(true, interval1 != interval2);
      Assert.AreEqual(false, interval2 == interval3);
      Assert.AreEqual(false, interval1 != new Interval(0, 12));
    }
    //[TestMethod]
    //[TestCategory("Problems.DataAnalysis")]
    //[TestProperty("Time", "short")]
    //public void RootTest() {
    //  var posInterval = new Interval(3, 5);
    //  var negInterval = new Interval(-6, -2);
    //  var posIntervalConst = new Interval(5, 5);
    //  var negIntervalConst = new Interval(-3, -3);
    //  var zeroIntervalConst = new Interval(0, 0);
    //  var zeroPosInterval = new Interval(0, 2);
    //  var zeroNegInterval = new Interval(-2, 0);

    //  var posRoot = new Interval(4, 4);
    //  var negRoot = new Interval(-4, -4);
    //  var zeroRoot = new Interval(0, 0);
    //  var oneRoot = new Interval(1, 1);

    //  Assert.AreEqual(Interval.Root(posInterval, posRoot), new Interval(Math.Pow(3, (1.0/4)), Math.Pow(5, (1.0/4))));
    //  Assert.AreEqual(Interval.Root(posInterval, negRoot), new Interval(Math.Pow(5, -(1.0/4)), Math.Pow(3, -(1.0/4))));
    //  Assert.AreEqual(Interval.Root(posInterval, zeroRoot), new Interval(double.NaN, double.NaN));
    //  Assert.AreEqual(Interval.Root(posInterval, oneRoot), new Interval(3, 5));

    //  Assert.AreEqual(Interval.Root(negInterval, posRoot), new Interval(Math.Pow(-6, (1.0 / 4)), Math.Pow(-2, (1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(negInterval, negRoot), new Interval(Math.Pow(-2, -(1.0 / 4)), Math.Pow(-6, -(1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(negInterval, zeroRoot), new Interval(double.NaN, double.NaN));
    //  Assert.AreEqual(Interval.Root(negInterval, oneRoot), new Interval(-6, -2));

    //  Assert.AreEqual(Interval.Root(posIntervalConst, posRoot), new Interval(Math.Pow(5, (1.0 / 4)), Math.Pow(5, (1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(posIntervalConst, negRoot), new Interval(Math.Pow(5, -(1.0 / 4)), Math.Pow(5, -(1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(posIntervalConst, zeroRoot), new Interval(double.NaN, double.NaN));
    //  Assert.AreEqual(Interval.Root(posIntervalConst, oneRoot), new Interval(5, 5));

    //  Assert.AreEqual(Interval.Root(negIntervalConst, posRoot), new Interval(Math.Pow(-3, (1.0 / 4)), Math.Pow(-3, (1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(negIntervalConst, negRoot), new Interval(Math.Pow(-3, -(1.0 / 4)), Math.Pow(-3, -(1.0 / 4))));
    //  Assert.AreEqual(Interval.Root(negIntervalConst, zeroRoot), new Interval(double.NaN, double.NaN));
    //  Assert.AreEqual(Interval.Root(negIntervalConst, oneRoot), new Interval(-3, -3));

    //  Assert.AreEqual(Interval.Root(zeroIntervalConst, posRoot), new Interval(0, 0));
    //  //Compley Infinity https://www.wolframalpha.com/input/?i=root%28-4%2C+0%29
    //  Assert.AreEqual(Interval.Root(zeroIntervalConst, negRoot), new Interval(double.PositiveInfinity, double.PositiveInfinity));
    //  Assert.AreEqual(Interval.Root(zeroIntervalConst, zeroRoot), new Interval(0, 0));
    //  Assert.AreEqual(Interval.Root(zeroIntervalConst, oneRoot), new Interval(0, 0));

    //  Assert.AreEqual(Interval.Root(zeroPosInterval, posRoot), new Interval(0, Math.Pow(2, (1.0 / 4))));
    //  //Check for complex infinity
    //  Assert.AreEqual(Interval.Root(zeroPosInterval, negRoot), new Interval(Math.Pow(2, -(1.0 / 4)), double.PositiveInfinity));
    //  Assert.AreEqual(Interval.Root(zeroPosInterval, zeroRoot), new Interval(0, 0));
    //  Assert.AreEqual(Interval.Root(zeroPosInterval, oneRoot), new Interval(0, 2));

    //  Assert.AreEqual(Interval.Root(zeroNegInterval, posRoot), new Interval(Math.Pow(-2, (1.0 / 4)),0));
    //  //Check for complex infinity
    //  Assert.AreEqual(Interval.Root(zeroNegInterval, negRoot), new Interval(Math.Pow(-2, -(1.0 / 4)), double.PositiveInfinity));
    //  Assert.AreEqual(Interval.Root(zeroNegInterval, zeroRoot), new Interval(double.NaN, double.NaN));
    //  Assert.AreEqual(Interval.Root(zeroNegInterval, oneRoot), new Interval(-2, 0));
    //}
  }
}
