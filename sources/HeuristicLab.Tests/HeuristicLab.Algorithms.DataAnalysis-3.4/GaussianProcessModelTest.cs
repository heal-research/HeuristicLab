#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis.Tests {
  [TestClass]

  // reference values calculated with Rasmussen's GPML MATLAB package
  public class GaussianProcessModelTest {
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "medium")]
    public void GaussianProcessModelOutputTest() {
      var provider = new RegressionCSVInstanceProvider();
      var problemData = provider.ImportData("co2.txt");

      var targetVariable = "interpolated";
      var allowedInputVariables = new string[] { "decimal date" };
      var rows = Enumerable.Range(0, 401);

      var meanFunction = new MeanConst();
      var covarianceFunction = new CovarianceSum();
      covarianceFunction.Terms.Add(new CovarianceSquaredExponentialIso());
      var prod = new CovarianceProduct();
      prod.Factors.Add(new CovarianceSquaredExponentialIso());
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
    }


    private void AssertEqual(double[] expected, double[] actual, double delta = 1E-3) {
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++)
        Assert.AreEqual(expected[i], actual[i], delta);
    }
  }
}
