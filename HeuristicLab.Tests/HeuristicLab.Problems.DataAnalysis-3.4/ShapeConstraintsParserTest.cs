using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Problems.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {
  [TestClass]
  public class ShapeConstraintsParserTest {
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_ValidText_ShapeConstraintsCount5() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2 \n"+
          "∂f /∂c in [0 .. inf.] \n" +
          "df / dG in [-inf. .. 0] \n" +
          "d²f / dG² in [-inf. .. 0] where c in [-50 .. 10] \n" +
          "∂³f/∂c³ in [0 .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(5, constraints.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_ValidTextWithLineComment_ShapeConstraintsCount4() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2 \n" +
          "∂f /∂c in [0 .. inf.] \n" +
          "df / dG in [-inf. .. 0] \n" +
          "d²f / dG² in [-inf. .. 0] where c in [-50 .. 10] \n" +
          "#∂³f/∂c³ in [0 .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(4, constraints.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_ValidTextWithLineCommentBeforeRegionList_ShapeConstraintsCount5() {
      string expression =
         "f in [-inf. .. 0] #where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2 \n" +
          "∂f /∂c in [0 .. inf.] \n" +
          "df / dG in [-inf. .. 0] \n" +
          "d²f / dG² in [-inf. .. 0] where c in [-50 .. 10] \n" +
          "∂³f/∂c³ in [0 .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(5, constraints.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_ValidTextWithTwoShapeConstraintInLine_ShapeConstraintsCount2() {
      string expression =
         "f in [-inf. .. 0] df / dG in [-inf. .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(0, constraints[0].Regions.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_ValidTextWithLineCommentBeforeRegionList_RegionCount0() {
      string expression =
         "f in [-inf. .. 0] #where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(0, constraints[0].Regions.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    [ExpectedException(typeof(ArgumentException))]
    public void Parse_InvalidText_ThrowException() {
      string expression =
         "f in (-inf. .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithNegativeInf_ReturnsCorrectLowerBound() {
      string expression =
         "f in [-inf. .. 0]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(double.NegativeInfinity, constraints[0].Interval.LowerBound);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithPositiveInf_ReturnsCorrectUpperBound() {
      string expression =
         "f in [0 .. inf.]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(double.PositiveInfinity, constraints[0].Interval.UpperBound);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithAlternateIntervalSeparator_ThrowsNoException() {
      string expression =
         "f in [0, inf.]";
      ShapeConstraintsParser.ParseConstraints(expression);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithNoDerivative_IsDerivativeFalse() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(false, constraints[0].IsDerivative);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithFirstDerivative_NumberOfDerivations1() {
      string expression =
         "∂f/∂c in [0 .. inf.]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(1, constraints[0].NumberOfDerivations);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithSecondDerivative_NumberOfDerivations2() {
      string expression =
         "d²f / dG² in [0 .. inf.]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(2, constraints[0].NumberOfDerivations);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithThirdDerivative_NumberOfDerivations3() {
      string expression =
         "∂³f/∂c³ in [0 .. inf.]";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(3, constraints[0].NumberOfDerivations);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_With2Regions_RegionCount2() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(2, constraints[0].Regions.Count);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithFirstRegion_ReturnsCorrectLowerBound() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(-10.0, constraints[0].Regions.GetInterval("G").LowerBound);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithFirstRegion_ReturnsCorrectUpperBound() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(10.0, constraints[0].Regions.GetInterval("G").UpperBound);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithSecondRegion_ReturnsCorrectLowerBound() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(-1234.23, constraints[0].Regions.GetInterval("c").LowerBound);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithSecondRegion_ReturnsCorrectUpperBound() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(7.53e-4, constraints[0].Regions.GetInterval("c").UpperBound);

    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis.ShapeConstraintsParser")]
    public void Parse_WithWeight_ReturnsCorrectWeight() {
      string expression =
         "f in [-inf. .. 0] where G in [-10 .. 10], c in [-1234.23 .. 7.53e-4] with weight: 1.2";
      var constraints = ShapeConstraintsParser.ParseConstraints(expression);
      Assert.AreEqual(1.2, constraints[0].Weight);

    }
  }
}
