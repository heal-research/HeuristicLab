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

using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis_34.Tests {
  [TestClass]

  // reference values calculated with Rasmussen's GPML MATLAB package
  public class GaussianProcessModelTest {
    [TestMethod]
    [DeploymentItem(@"HeuristicLab.Algorithms.DataAnalysis-3.4/co2.txt")]
    public void GaussianProcessModelOutputTest() {
      var provider = new RegressionCSVInstanceProvider();
      var problemData = provider.ImportData("co2.txt");

      var targetVariable = "interpolated";
      var allowedInputVariables = new string[] { "decimal date" };
      var rows = Enumerable.Range(0, 401);

      var meanFunction = new MeanConst();
      var covarianceFunction = new CovarianceSum();
      covarianceFunction.Terms.Add(new CovarianceSEiso());
      var prod = new CovarianceProd();
      prod.Factors.Add(new CovarianceSEiso());
      prod.Factors.Add(new CovariancePeriodic());
      covarianceFunction.Terms.Add(prod);

      {
        var hyp = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        var model = new GaussianProcessModel(problemData.Dataset, targetVariable, allowedInputVariables, rows, hyp,
                                             meanFunction,
                                             covarianceFunction);
        Assert.AreEqual(4.3170e+004, model.NegativeLogLikelihood, 1);

        var dHyp = model.HyperparameterGradients;
        Assert.AreEqual(-248.7932, dHyp[0], 1E-2);
        var dHypCovExpected = new double[] { -0.5550e4, -5.5533e4, -0.2511e4, -2.7625e4, -1.3033e4, 0.0289e4, -2.7625e4 };
        AssertEqual(dHypCovExpected, dHyp.Skip(1).Take(7).ToArray(), 1);
        Assert.AreEqual(-2.0171e+003, dHyp.Last(), 1);


        var predTrain = model.GetEstimatedValues(problemData.Dataset, new int[] { 0, 400 }).ToArray();
        Assert.AreEqual(310.5930, predTrain[0], 1e-3);
        Assert.AreEqual(347.9993, predTrain[1], 1e-3);

        var predTrainVar = model.GetEstimatedVariance(problemData.Dataset, problemData.TrainingIndices).ToArray();
      }

      {
        var hyp = new double[] { 0.029973094285941, 0.455535210579926, 3.438647883940457, 1.464114485889487, 3.001788584487478, 3.815289323309630, 4.374914122810222, 3.001788584487478, 0.716427415979145 };
        var model = new GaussianProcessModel(problemData.Dataset, targetVariable, allowedInputVariables, rows, hyp,
                                             meanFunction,
                                             covarianceFunction);
        Assert.AreEqual(872.8448, model.NegativeLogLikelihood, 1e-3);

        var dHyp = model.HyperparameterGradients;
        Assert.AreEqual(-0.0046, dHyp[0], 1e-3);
        var dHypCovExpected = new double[] { 0.2652, -0.2386, 0.1706, -0.1744, 0.0000, 0.0000, -0.1744 };
        AssertEqual(dHypCovExpected, dHyp.Skip(1).Take(7).ToArray(), 1e-3);
        Assert.AreEqual(0.8621, dHyp.Last(), 1e-3);

        var predTrain = model.GetEstimatedValues(problemData.Dataset, new int[] { 0, 400 }).ToArray();
        Assert.AreEqual(315.3692, predTrain[0], 1e-3);
        Assert.AreEqual(356.6076, predTrain[1], 1e-3);
      }

      /*
      {
        // example from GPML book
        var hyp = new double[] { 
          341.0, // mean 341 ppm
          // SE iso (long term trend)
          Math.Log(67.0 / 45.0), // length scale 67 years
          Math.Log(Math.Sqrt(66)), // magnitude 66ppm
                    
          // product of SEiso and periodic
          Math.Log(90.0 / 45.0), // decay-time 90 years
          Math.Log(Math.Sqrt(2.4)), // magnitude 2.4ppm

          Math.Log(1.3), // smoothness
          Math.Log(1), // period 1 year
          Math.Log(Math.Sqrt(2.4)), // magnitude 2.4ppm

          // short term variation
          Math.Log(1.2 / 45.0), // typical length 1.2 years
          Math.Log(Math.Sqrt(0.66)), // magnitude 0.66ppm
          Math.Log(0.78), // shape (very small)

          // SEiso (correlated noise)
          Math.Log(1.6 / 45.0 / 12.0), // 1.6 months
          Math.Log(Math.Sqrt(0.18)), // amplitude of correlated noise 0.18ppm
          Math.Log(Math.Sqrt(0.19)),  // theta11 0.19ppm noise
          };

        covarianceFunction.Terms.Add(new CovarianceRQiso());
        covarianceFunction.Terms.Add(new CovarianceSEiso()); // correlated noise
        var model = new GaussianProcessModel(problemData.Dataset, targetVariable, allowedInputVariables, Enumerable.Range(0, 545), hyp,
                                             new MeanConst(),
                                             covarianceFunction);
        Assert.AreEqual(-108.5, model.NegativeLogLikelihood, 1);
      }
       */
    }


    private void AssertEqual(double[] expected, double[] actual, double delta = 1E-3) {
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
        Assert.AreEqual(expected[i], actual[i], delta);
    }
  }
}
