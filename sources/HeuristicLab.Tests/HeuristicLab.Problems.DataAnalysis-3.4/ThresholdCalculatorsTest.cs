#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Problems.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis_34.Tests {

  [TestClass()]
  public class ThresholdCalculatorsTest {
    [TestMethod]
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
        double[] estimatedValues = new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
        double[] targetClassValues = new double[] { 2.0, 2.0, 2.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
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

    }


    private static void AssertEqual(double[] expected, double[] actual) {
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
        Assert.AreEqual(expected[i], actual[i]);
    }
  }
}
