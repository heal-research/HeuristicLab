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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GaussianProcessRegressionSampleTest {
    private const string SampleFileName = "GPR";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGaussianProcessRegressionSampleTest() {
      var gpr = CreateGaussianProcessRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(gpr, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGaussianProcessRegressionSample() {
      var gpr = CreateGaussianProcessRegressionSample();
      gpr.SetSeedRandomly = false;
      gpr.Seed = 1618551877;
      SamplesUtils.RunAlgorithm(gpr);
      Assert.AreEqual(-940.206256787095, SamplesUtils.GetDoubleResult(gpr, "NegativeLogLikelihood"), 1E-8);
      Assert.AreEqual(0.995598837993128, SamplesUtils.GetDoubleResult(gpr, "Training R²"), 1E-8);
    }

    private GaussianProcessRegression CreateGaussianProcessRegressionSample() {
      var gpr = new GaussianProcessRegression();
      var provider = new VariousInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Contains("Spatial co-evolution")).Single();
      var regProblem = new RegressionProblem();
      regProblem.Load(provider.LoadData(instance));

      #region Algorithm Configuration
      gpr.Name = "Gaussian Process Regression";
      gpr.Description = "A Gaussian process regression algorithm which solves the spatial co-evolution benchmark problem";
      gpr.Problem = regProblem;

      gpr.CovarianceFunction = new CovarianceSquaredExponentialIso();
      gpr.MeanFunction = new MeanConst();
      gpr.MinimizationIterations = 20;
      gpr.Seed = 0;
      gpr.SetSeedRandomly = true;
      #endregion

      gpr.Engine = new ParallelEngine.ParallelEngine();
      return gpr;
    }
  }
}
