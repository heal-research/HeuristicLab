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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Problems.Programmable;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GAGroupingProblemSampleTest {
    private const string SampleFileName = "GA_Grouping";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    #region Code
    private const string ProblemCode = @"
using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledSingleObjectiveProblemDefinition<LinearLinkageEncoding, LinearLinkage> {
    private const int ProblemSize = 100;
    public override bool Maximization { get { return false; } }

    private bool[,] adjacencyMatrix;
    
    public override void Initialize() {
      Encoding.Length = ProblemSize;
      adjacencyMatrix = new bool[ProblemSize, ProblemSize];
      var random = new System.Random(13);
      for (var i = 0; i < ProblemSize - 1; i++)
        for (var j = i + 1; j < ProblemSize; j++)
          adjacencyMatrix[i, j] = adjacencyMatrix[j, i] = random.Next(2) == 0;
    }

    public override double Evaluate(LinearLinkage solution, IRandom random) {
      var penalty = 0;
      var groups = solution.GetGroups().ToList();
      for (var i = 0; i < groups.Count; i++) {
        for (var j = 0; j < groups[i].Count; j++)
          for (var k = j + 1; k < groups[i].Count; k++)
            if (!adjacencyMatrix[groups[i][j], groups[i][k]]) penalty++;
      }
      var result = groups.Count;
      if (penalty > 0) result += penalty + ProblemSize;
      return result;
    }

    public override void Analyze(LinearLinkage[] solutions, double[] qualities, ResultCollection results, IRandom random) { }

    public override IEnumerable<LinearLinkage> GetNeighbors(LinearLinkage solution, IRandom random) {
      foreach (var move in ExhaustiveSwap2MoveGenerator.Generate(solution)) {
        var neighbor = (LinearLinkage)solution.Clone();
        Swap2MoveMaker.Apply(neighbor, move);
        yield return neighbor;
      }
    }
  }
}
";
    #endregion

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGaGroupingProblemSampleTest() {
      var ga = CreateGaGroupingProblemSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGaGroupingProblemSampleTest() {
      var ga = CreateGaGroupingProblemSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(127, SamplesUtils.GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(129,38, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(132, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(99100, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaGroupingProblemSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration
      var problem = new SingleObjectiveLinearLinkageProgrammableProblem() {
        ProblemScript = { Code = ProblemCode }
      };
      problem.ProblemScript.Compile();
      #endregion
      #region Algorithm Configuration
      ga.Name = "Genetic Algorithm - Graph Coloring";
      ga.Description = "A genetic algorithm which solves a graph coloring problem using the linear linkage encoding.";
      ga.Problem = problem;
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, MultiLinearLinkageCrossover, MultiLinearLinkageManipulator>(
        ga, 100, 1, 1000, 0.05, 2);
      #endregion

      return ga;
    }
  }
}
