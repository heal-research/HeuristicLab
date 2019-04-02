using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {
  [TestClass]
  public class IntervalInterpreterTest {
    private IRegressionProblemData problemData;
    private Dictionary<string, Interval> variableRanges;

    [TestInitialize]
    public void InitTest() {
      double[,] arr = new double[4, 3];

      arr[0, 0] = 3;
      arr[0, 1] = 6;
      arr[0, 2] = 2;
      arr[1, 0] = 5;
      arr[1, 1] = 2;
      arr[1, 2] = 1;
      arr[2, 0] = 8;
      arr[2, 1] = 5;
      arr[2, 2] = 0;
      arr[3, 0] = 3;
      arr[3, 1] = 4;
      arr[3, 2] = 2;

      var ds = new Dataset(new string[] { "x1", "x2", "y" }, arr);
      problemData = (IRegressionProblemData)new RegressionProblemData(ds, new string[] { "x1", "x2" }, "y");

      variableRanges = new Dictionary<string, Interval>();
      variableRanges.Add("x1", new Interval(1, 10));
      variableRanges.Add("x2", new Interval(4, 6));
    }

    private void EvaluateTest(string expression, Interval expectedResult, Dictionary<string, Interval> variableRanges = null) {
      var parser = new InfixExpressionParser();
      var tree = parser.Parse(expression);
      var interpreter = new IntervalInterpreter();
      Interval result;
      if (variableRanges == null)
        result = interpreter.GetSymbolicExpressionTreeInterval(tree, problemData.Dataset, problemData.AllIndices);
      else
        result = interpreter.GetSymbolicExpressionTreeInterval(tree, variableRanges);

      Assert.AreEqual(expectedResult, result);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterAdd() {
      EvaluateTest("x1 + x2", new Interval(5, 14));
      EvaluateTest("x1 + x2", new Interval(5, 16), variableRanges);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterLogAdd() {
      EvaluateTest("log(x1 + x2)", new Interval(Math.Log(5), Math.Log(14)));
      EvaluateTest("log(x1 + x2)", new Interval(Math.Log(5), Math.Log(16)), variableRanges);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterLogAddMul() {
      EvaluateTest("log(3*x1 + x2)", new Interval(Math.Log(11), Math.Log(30)));
      EvaluateTest("log(3*x1 + x2)", new Interval(Math.Log(7), Math.Log(36)), variableRanges);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterSin() {
      EvaluateTest("sin(x1+x2)", new Interval(-1, 1));
      EvaluateTest("sin(x1+x2)", new Interval(-1, 1), variableRanges);
      EvaluateTest("sin(1+2)", new Interval(Math.Sin(3), Math.Sin(3)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterCos() {
      EvaluateTest("cos(x1+x2)", new Interval(-1, 1));
      EvaluateTest("cos(x1+x2)", new Interval(-1, 1), variableRanges);
      EvaluateTest("cos(1+2)", new Interval(Math.Sin(3 - Math.PI / 2), Math.Sin(3 - Math.PI / 2)));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterExp() {
      EvaluateTest("exp(x1-x2)", new Interval(Math.Exp(-3), Math.Exp(6)));
      EvaluateTest("exp(x1-x2)", new Interval(Math.Exp(-5), Math.Exp(6)), variableRanges);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterExpRoot() {
      EvaluateTest("exp(root(x1*x2, 2))", new Interval(Math.Exp(Math.Sqrt(6)), Math.Exp(Math.Sqrt(48))));
      EvaluateTest("exp(root(x1*x2, 2))", new Interval(Math.Exp(Math.Sqrt(4)), Math.Exp(Math.Sqrt(60))), variableRanges);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void TestIntervalInterpreterPower() {
      EvaluateTest("pow(x1, 2)", new Interval(Math.Pow(3, 1), Math.Pow(8, 3)));
    }
  }
}
