#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GPMultiplexerSampleTest {
    private const string SampleFileName = "GP_Multiplexer";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGpMultiplexerSampleTest() {
      var ga = CreateGpMultiplexerSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGpMultiplexerSampleTest() {
      var osga = CreateGpMultiplexerSample();
      osga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(osga);

      Assert.AreEqual(1856, SamplesUtils.GetDoubleResult(osga, "BestQuality"), 1E-8);
      Assert.AreEqual(1784.76, SamplesUtils.GetDoubleResult(osga, "CurrentAverageQuality"), 1E-8);
      Assert.AreEqual(1536, SamplesUtils.GetDoubleResult(osga, "CurrentWorstQuality"), 1E-8);
      Assert.AreEqual(66900, SamplesUtils.GetIntResult(osga, "EvaluatedSolutions"));
    }

    public static OffspringSelectionGeneticAlgorithm CreateGpMultiplexerSample() {
      var problem = new HeuristicLab.Problems.GeneticProgramming.Boolean.MultiplexerProblem();
      problem.Name = "11-Multiplexer Problem";
      problem.Encoding.TreeLength = 50;
      problem.Encoding.TreeDepth = 50;

      var osga = new OffspringSelectionGeneticAlgorithm();
      osga.Name = "Genetic Programming - Multiplexer 11 Problem";
      osga.Description = "A genetic programming algorithm that solves the 11-bit multiplexer problem.";
      osga.Problem = problem;
      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>
        (osga, popSize: 100, elites: 1, maxGens: 50, mutationRate: 0.25);
      osga.MaximumSelectionPressure.Value = 200;
      return osga;

    }
  }
}
