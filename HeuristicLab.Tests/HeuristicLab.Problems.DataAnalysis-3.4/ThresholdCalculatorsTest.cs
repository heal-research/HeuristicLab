#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class ThresholdCalculatorsTest {
    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void NormalDistributionCutPointsThresholdCalculatorTest() {

      {
        // simple two-class case
        double[] estimatedValues = new double[] { 1.0, 0.99, 1.01, 2.0, 1.99, 2.01 };
        double[] targetClassValues = new double[] { 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);

        var expectedClassValues = new double[] { 0.0, 1.0 };
        var expectedTresholds = new double[] { double.NegativeInfinity, 1.5 };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }

      {
        // switched classes two-class case
        double[] estimatedValues = new double[] { 1.0, 0.99, 1.01, 2.0, 1.99, 2.01 };
        double[] targetClassValues = new double[] { 1.0, 1.0, 1.0, 0.0, 0.0, 0.0 };
        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);

        var expectedClassValues = new double[] { 1.0, 0.0 };
        var expectedTresholds = new double[] { double.NegativeInfinity, 1.5 };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }

      {
        // three-class case with permutated estimated values
        double[] estimatedValues = new double[] { 1.0, 0.99, 1.01, 2.0, 1.99, 2.01, -1.0, -0.99, -1.01 };
        double[] targetClassValues = new double[] { 2.0, 2.0, 2.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);

        var expectedClassValues = new double[] { 1.0, 2.0, 0.0 };
        var expectedTresholds = new double[] { double.NegativeInfinity, 0.0, 1.5 };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }

      {
        // constant output values for all classes
        // most frequent class is 0
        double[] estimatedValues = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
        double[] targetClassValues = new double[] { 2.0, 2.0, 2.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);

        var expectedClassValues = new double[] { 0.0 };
        var expectedTresholds = new double[] { double.NegativeInfinity };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }

      {
        // constant output values for two of three classes
        double[] estimatedValues = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -0.99, -1.01 };
        double[] targetClassValues = new double[] { 2.0, 2.0, 2.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);


        var expectedClassValues = new double[] { 1.0, 0.0, 1.0 };
        double range = 1.0 + 1.01;
        var expectedTresholds = new double[] { double.NegativeInfinity, 1.0 - 0.001 * range, 1.0 + 0.001 * range };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }


      {
        // normal operation
        double[] estimatedValues = new double[]
                                     {
                                       2.9937,
                                       2.9861,
                                       1.0202,
                                       0.9844,
                                       1.9912,
                                       1.9970,
                                       0.9776,
                                       0.9611,
                                       1.9882,
                                       1.9953,
                                       2.0147,
                                       2.0106,
                                       2.9949,
                                       0.9925,
                                       3.0050,
                                       1.9987,
                                       2.9973,
                                       1.0110,
                                       2.0160,
                                       2.9559,
                                       1.9943,
                                       2.9477,
                                       2.0158,
                                       2.0026,
                                       1.9837,
                                       3.0185,
                                     };
        double[] targetClassValues = new double[]
                                       {
                                          3,
                                          3,
                                          1,
                                          1,
                                          2,
                                          2,
                                          1,
                                          1,
                                          2,
                                          2,
                                          2,
                                          2,
                                          3,
                                          1,
                                          3,
                                          2,
                                          3,
                                          1,
                                          2,
                                          3,
                                          2,
                                          3,
                                          2,
                                          2,
                                          2,
                                          3,
                                       };

        double[] classValues;
        double[] thresholds;
        NormalDistributionCutPointsThresholdCalculator.CalculateThresholds(null, estimatedValues, targetClassValues,
                                                                           out classValues, out thresholds);


        var expectedClassValues = new double[] { 3.0, 1.0, 2.0, 3.0 };
        var expectedTresholds = new double[] { double.NegativeInfinity, -18.36483129043598, 1.6574168546810319, 2.3148463106026012 };

        AssertEqual(expectedClassValues, classValues);
        AssertEqual(expectedTresholds, thresholds);
      }
    }


    private static void AssertEqual(double[] expected, double[] actual) {
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
        Assert.AreEqual(expected[i], actual[i]);
    }
  }
}
