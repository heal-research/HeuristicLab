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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Instances.TSPLIB;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GATspSampleTest {
    private const string SampleFileName = "GA_TSP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGaTspSampleTest() {
      var ga = CreateGaTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGaTspSampleTest() {
      var ga = CreateGaTspSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(12332, SamplesUtils.GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(13123.2, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(14538, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(99100, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaTspSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration
      var provider = new TSPLIBTSPInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name == "ch130").Single();
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.Load(provider.LoadData(instance));
      tspProblem.UseDistanceMatrix = true;
      #endregion
      #region Algorithm Configuration
      ga.Name = "Genetic Algorithm - TSP";
      ga.Description = "A genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      ga.Problem = tspProblem;
      SamplesUtils.ConfigureGeneticAlgorithmParameters<ProportionalSelector, OrderCrossover2, InversionManipulator>(
        ga, 100, 1, 1000, 0.05);
      #endregion

      return ga;
    }
  }
}
