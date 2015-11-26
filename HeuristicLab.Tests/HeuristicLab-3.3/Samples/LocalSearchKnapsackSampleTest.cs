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
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Knapsack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class LocalSearchKnapsackSampleTest {
    private const string SampleFileName = "LS_Knapsack";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateLocalSearchKnapsackSampleTest() {
      var ls = CreateLocalSearchKnapsackSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ls, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "medium")]
    public void RunLocalSearchKnapsackSampleTest() {
      var ls = CreateLocalSearchKnapsackSample();
      ls.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ls);
      Assert.AreEqual(345, SamplesUtils.GetDoubleResult(ls, "BestQuality"));
      Assert.AreEqual(340.70731707317071, SamplesUtils.GetDoubleResult(ls, "CurrentAverageQuality"));
      Assert.AreEqual(337, SamplesUtils.GetDoubleResult(ls, "CurrentWorstQuality"));
      Assert.AreEqual(82000, SamplesUtils.GetIntResult(ls, "EvaluatedMoves"));
    }

    private LocalSearch CreateLocalSearchKnapsackSample() {
      LocalSearch ls = new LocalSearch();
      #region Problem Configuration
      KnapsackProblem problem = new KnapsackProblem();
      problem.BestKnownQuality = 362;
      problem.BestKnownSolution = new HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVector(new bool[] { 
       true , false, false, true , true , true , true , true , false, true , true , true , true , true , true , false, true , false, true , true , false, true , true , false, true , false, true , true , true , false, true , true , false, true , true , false, true , false, true , true , true , true , true , true , true , true , true , true , true , true , true , false, true , false, false, true , true , false, true , true , true , true , true , true , true , true , false, true , false, true , true , true , true , false, true , true , true , true , true , true , true , true});
      problem.SolutionCreatorParameter.Value = new RandomBinaryVectorCreator();
      problem.KnapsackCapacity = 297;
      problem.Values = new IntArray(new int[] { 
  6, 1, 1, 6, 7, 8, 7, 4, 2, 5, 2, 6, 7, 8, 7, 1, 7, 1, 9, 4, 2, 6, 5,  3, 5, 3, 3, 6, 5, 2, 4, 9, 4, 5, 7, 1, 4, 3, 5, 5, 8, 3, 6, 7, 3, 9, 7, 7, 5, 5, 7, 1, 4, 4, 3, 9, 5, 1, 6, 2, 2, 6, 1, 6, 5, 4, 4, 7, 1,  8, 9, 9, 7, 4, 3, 8, 7, 5, 7, 4, 4, 5});
      problem.Weights = new IntArray(new int[] { 
 1, 9, 3, 6, 5, 3, 8, 1, 7, 4, 2, 1, 2, 7, 9, 9, 8, 4, 9, 2, 4, 8, 3, 7, 5, 7, 5, 5, 1, 9, 8, 7, 8, 9, 1, 3, 3, 8, 8, 5, 1, 2, 4, 3, 6, 9, 4, 4, 9, 7, 4, 5, 1, 9, 7, 6, 7, 4, 7, 1, 2, 1, 2, 9, 8, 6, 8, 4, 7, 6, 7, 5, 3, 9, 4, 7, 4, 6, 1, 2, 5, 4});
      problem.Name = "Knapsack Problem";
      problem.Description = "Represents a Knapsack problem.";
      #endregion
      #region Algorithm Configuration
      ls.Name = "Local Search - Knapsack";
      ls.Description = "A local search algorithm that solves a randomly generated Knapsack problem";
      ls.Problem = problem;
      ls.MaximumIterations.Value = 1000;
      ls.MoveEvaluator = ls.MoveEvaluatorParameter.ValidValues
        .OfType<KnapsackOneBitflipMoveEvaluator>()
        .Single();
      ls.MoveGenerator = ls.MoveGeneratorParameter.ValidValues
        .OfType<ExhaustiveOneBitflipMoveGenerator>()
        .Single();
      ls.MoveMaker = ls.MoveMakerParameter.ValidValues
        .OfType<OneBitflipMoveMaker>()
        .Single();
      ls.SampleSize.Value = 100;
      ls.Seed.Value = 0;
      ls.SetSeedRandomly.Value = true;
      #endregion
      ls.Engine = new ParallelEngine.ParallelEngine();
      return ls;
    }
  }
}
