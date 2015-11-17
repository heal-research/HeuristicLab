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
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Instances.TSPLIB;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class AlpsTspSampleTest {
    private const string SampleFileName = "ALPSGA_TSP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateAlpsGaTspSampleTest() {
      var alpsGa = CreateAlpsGaTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(alpsGa, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunAlpsGaTspSampleTest() {
      var alpsGaE = CreateAlpsGaTspSample(plusSelection: false);
      alpsGaE.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(alpsGaE);
      Assert.AreEqual(23661, SamplesUtils.GetDoubleResult(alpsGaE, "BestQuality"));
      Assert.AreEqual(35131.1625, SamplesUtils.GetDoubleResult(alpsGaE, "CurrentAverageQuality"));
      Assert.AreEqual(49699, SamplesUtils.GetDoubleResult(alpsGaE, "CurrentWorstQuality"));
      Assert.AreEqual(26640, SamplesUtils.GetIntResult(alpsGaE, "EvaluatedSolutions"));

      var alpsGaPs = CreateAlpsGaTspSample(plusSelection: true);
      alpsGaPs.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(alpsGaPs);
      Assert.AreEqual(21365, SamplesUtils.GetDoubleResult(alpsGaPs, "BestQuality"));
      Assert.AreEqual(30774.695, SamplesUtils.GetDoubleResult(alpsGaPs, "CurrentAverageQuality"));
      Assert.AreEqual(49699, SamplesUtils.GetDoubleResult(alpsGaPs, "CurrentWorstQuality"));
      Assert.AreEqual(26900, SamplesUtils.GetIntResult(alpsGaPs, "EvaluatedSolutions"));
    }

    private AlpsGeneticAlgorithm CreateAlpsGaTspSample(bool plusSelection = false) {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new TSPLIBTSPInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name == "ch130");
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.Load(provider.LoadData(instance));
      tspProblem.UseDistanceMatrix.Value = true;
      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Algorithm - TSP";
      alpsGa.Description = "An age-layered population structure genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      alpsGa.Problem = tspProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<GeneralizedRankSelector, OrderCrossover2, InversionManipulator>(alpsGa, 
        numberOfLayers: 10, 
        popSize: 100, 
        mutationRate: 0.05, 
        elites: 1, 
        plusSelection: plusSelection, 
        agingScheme: AgingScheme.Polynomial, 
        ageGap: 20, 
        ageInheritance: 1.0, 
        maxGens: 100);
      #endregion
      return alpsGa;
    }
  }
}
