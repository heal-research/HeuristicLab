#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Algorithms.ScatterSearch;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Instances.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class ScatterSearchVRPSampleTest {
    private const string SampleFileName = "SS_VRP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateScatterSearchVRPSampleTest() {
      var ss = CreateScatterSearchVRPSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ss, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunScatterSearchVRPSampleTest() {
      var ss = CreateScatterSearchVRPSample();
      ss.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ss);
      Assert.AreEqual(828.93686694283383, SamplesUtils.GetDoubleResult(ss, "BestQuality"));
      Assert.AreEqual(868.63623986983077, SamplesUtils.GetDoubleResult(ss, "CurrentAverageQuality"));
      Assert.AreEqual(1048.8333559209832, SamplesUtils.GetDoubleResult(ss, "CurrentWorstQuality"));
      Assert.AreEqual(262622, SamplesUtils.GetIntResult(ss, "EvaluatedSolutions"));
    }

    private ScatterSearch CreateScatterSearchVRPSample() {
      #region Problem Configuration
      var provider = new SolomonInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name == "C101");
      VehicleRoutingProblem vrpProblem = new VehicleRoutingProblem();
      vrpProblem.Load(provider.LoadData(instance));
      #endregion

      #region Algorithm Configuration
      ScatterSearch ss = new ScatterSearch();
      ss.Engine = new SequentialEngine.SequentialEngine();
      ss.Name = "Scatter Search - VRP";
      ss.Description = "A scatter search algorithm which solves the \"C101\" vehicle routing problem (imported from Solomon)";
      ss.Problem = vrpProblem;

      var improver = ss.Problem.Operators.OfType<VRPIntraRouteImprovementOperator>().First();
      improver.ImprovementAttemptsParameter.Value.Value = 15;
      improver.SampleSizeParameter.Value.Value = 10;
      ss.Improver = improver;

      var pathRelinker = ss.Problem.Operators.OfType<VRPPathRelinker>().First();
      pathRelinker.IterationsParameter.Value.Value = 25;
      ss.PathRelinker = pathRelinker;

      var similarityCalculator = ss.SimilarityCalculatorParameter.ValidValues.OfType<VRPSimilarityCalculator>().First();
      ss.SimilarityCalculator = similarityCalculator;

      ss.MaximumIterations.Value = 2;
      ss.PopulationSize.Value = 20;
      ss.ReferenceSetSize.Value = 10;
      ss.Seed.Value = 0;
      return ss;
      #endregion
    }
  }
}
