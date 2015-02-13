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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Instances.TSPLIB;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class UnitTest1 {
    private const string SampleFileName = "IslandGA_TSP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateIslandGaTspSampleTest() {
      var ga = CreateIslandGaTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunIslandGaTspSampleTest() {
      var ga = CreateIslandGaTspSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(9918, SamplesUtils.GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(10324.64, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(11823, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(495500, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    private IslandGeneticAlgorithm CreateIslandGaTspSample() {
      IslandGeneticAlgorithm ga = new IslandGeneticAlgorithm();
      #region Problem Configuration
      var provider = new TSPLIBTSPInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name == "ch130").Single();
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.Load(provider.LoadData(instance));
      tspProblem.UseDistanceMatrix.Value = true;
      #endregion
      #region Algorithm Configuration
      ga.Name = "Island Genetic Algorithm - TSP";
      ga.Description = "An island genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      ga.Problem = tspProblem;
      SamplesUtils.ConfigureIslandGeneticAlgorithmParameters<ProportionalSelector, OrderCrossover2, InversionManipulator,
        UnidirectionalRingMigrator, BestSelector, WorstReplacer>(
        ga, 100, 1, 1000, 0.05, 5, 50, 0.25);
      #endregion
      return ga;
    }
  }
}
