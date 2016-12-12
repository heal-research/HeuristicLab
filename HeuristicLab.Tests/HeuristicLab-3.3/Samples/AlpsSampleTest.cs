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
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Problems.Instances.TSPLIB;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class AlpsTspSampleTest {
    private const string TspSampleFileName = "ALPSGA_TSP";
    private const string SymRegSampleFileName = "ALPSGP_SymReg";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateAlpsGaTspSampleTest() {
      var alpsGa = CreateAlpsGaTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, TspSampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(alpsGa, path);
    }

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateAlpsGaSymRegSampleTest() {
      var alpsGa = CreateAlpsGaSymRegSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SymRegSampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(alpsGa, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunAlpsGaTspSampleTest() {
      var alpsGa = CreateAlpsGaTspSample();
      alpsGa.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(alpsGa);
      Assert.AreEqual(7967, SamplesUtils.GetDoubleResult(alpsGa, "BestQuality"));
      Assert.AreEqual(17565.174444444445, SamplesUtils.GetDoubleResult(alpsGa, "CurrentAverageQuality"));
      Assert.AreEqual(50295, SamplesUtils.GetDoubleResult(alpsGa, "CurrentWorstQuality"));
      Assert.AreEqual(621900, SamplesUtils.GetIntResult(alpsGa, "EvaluatedSolutions"));
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunAlpsGaSymRegSampleTest() {
      var alpsGa = CreateAlpsGaSymRegSample();
      alpsGa.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(alpsGa);
      Assert.AreEqual(265855, SamplesUtils.GetIntResult(alpsGa, "EvaluatedSolutions"));
    }

    private AlpsGeneticAlgorithm CreateAlpsGaTspSample() {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new TSPLIBTSPInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name == "ch130");
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.Load(provider.LoadData(instance));
      tspProblem.UseDistanceMatrix = true;
      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Algorithm - TSP";
      alpsGa.Description = "An age-layered population structure genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      alpsGa.Problem = tspProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<GeneralizedRankSelector, MultiPermutationCrossover, InversionManipulator>(alpsGa,
        numberOfLayers: 1000,
        popSize: 100,
        mutationRate: 0.05,
        elites: 1,
        plusSelection: true,
        agingScheme: AgingScheme.Polynomial,
        ageGap: 20,
        ageInheritance: 1.0,
        maxGens: 1000);
      var checkedCrossovers = new[] { typeof(EdgeRecombinationCrossover), typeof(MaximalPreservativeCrossover), typeof(OrderCrossover2) };
      var multiCrossover = (MultiPermutationCrossover)alpsGa.Crossover;
      var crossovers = multiCrossover.Operators.Where(c => checkedCrossovers.Any(cc => cc.IsInstanceOfType(c))).ToList();
      foreach (var c in multiCrossover.Operators)
        multiCrossover.Operators.SetItemCheckedState(c, crossovers.Contains(c));
      #endregion
      return alpsGa;
    }

    private AlpsGeneticAlgorithm CreateAlpsGaSymRegSample() {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new VladislavlevaInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.StartsWith("Vladislavleva-5 F5"));
      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Load(provider.LoadData(instance));

      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 35;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 35;

      var grammar = (TypeCoherentExpressionGrammar)symbRegProblem.SymbolicExpressionTreeGrammar;
      grammar.Symbols.OfType<Exponential>().Single().Enabled = false;
      grammar.Symbols.OfType<Logarithm>().Single().Enabled = false;

      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Programming - Symbolic Regression";
      alpsGa.Description = "An ALPS-GP to solve a symbolic regression problem (Vladislavleva-5 dataset)";
      alpsGa.Problem = symbRegProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<GeneralizedRankSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(alpsGa,
        numberOfLayers: 1000,
        popSize: 100,
        mutationRate: 0.25,
        elites: 1,
        plusSelection: false,
        agingScheme: AgingScheme.Polynomial,
        ageGap: 15,
        ageInheritance: 1.0,
        maxGens: 500);
      #endregion
      return alpsGa;
    }
  }
}
