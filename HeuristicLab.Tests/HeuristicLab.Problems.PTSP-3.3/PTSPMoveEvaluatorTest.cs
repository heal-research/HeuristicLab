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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.PTSP.Tests {
  /// <summary>
  ///This is a test class for PTSP move evaluators
  ///</summary>
  [TestClass()]
  public class PTSPMoveEvaluatorTest {
    private const int ProblemSize = 10;
    private const int RealizationsSize = 100;
    private static DoubleMatrix coordinates;
    private static DistanceMatrix distances;
    private static Permutation tour;
    private static MersenneTwister random;
    private static ItemList<BoolArray> realizations;
    private static DoubleArray probabilities;

    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      random = new MersenneTwister();
      coordinates = new DoubleMatrix(ProblemSize, 2);
      distances = new DistanceMatrix(ProblemSize, ProblemSize);
      for (var i = 0; i < ProblemSize; i++) {
        coordinates[i, 0] = random.Next(ProblemSize * 10);
        coordinates[i, 1] = random.Next(ProblemSize * 10);
      }
      for (var i = 0; i < ProblemSize - 1; i++) {
        for (var j = i + 1; j < ProblemSize; j++) {
          distances[i, j] = Math.Round(Math.Sqrt(Math.Pow(coordinates[i, 0] - coordinates[j, 0], 2) + Math.Pow(coordinates[i, 1] - coordinates[j, 1], 2)));
          distances[j, i] = distances[i, j];
        }
      }

      probabilities = new DoubleArray(ProblemSize);
      for (var i = 0; i < ProblemSize; i++) {
        probabilities[i] = random.NextDouble();
      }

      realizations = new ItemList<BoolArray>(RealizationsSize);
      for (var i = 0; i < RealizationsSize; i++) {
        var countOnes = 0;
        var newRealization = new BoolArray(ProblemSize);
        while (countOnes < 4) { //only generate realizations with at least 4 cities visited
          countOnes = 0;
          for (var j = 0; j < ProblemSize; j++) {
            newRealization[j] = random.NextDouble() < probabilities[j];
            if (newRealization[j]) countOnes++;
          }
        }
        realizations.Add(newRealization);
      }

      tour = new Permutation(PermutationTypes.RelativeUndirected, ProblemSize, random);
    }

    [TestMethod]
    [TestCategory("Problems.ProbabilisticTravelingSalesman")]
    [TestProperty("Time", "short")]
    public void InversionMoveEvaluatorTest() {
      Func<int, int, double> distance = (a, b) => distances[a, b];
      double variance;
      var beforeMatrix = EstimatedProbabilisticTravelingSalesmanProblem.Evaluate(tour, distance, realizations, out variance);

      for (var i = 0; i < 500; i++) {
        var move = StochasticInversionSingleMoveGenerator.Apply(tour, random);
        var moveMatrix = PTSPEstimatedInversionMoveEvaluator.EvaluateMove(tour, move, distance, realizations);
        InversionManipulator.Apply(tour, move.Index1, move.Index2);
        var afterMatrix = EstimatedProbabilisticTravelingSalesmanProblem.Evaluate(tour, distance, realizations, out variance);

        Assert.IsTrue(Math.Abs(moveMatrix).IsAlmost(Math.Abs(afterMatrix - beforeMatrix)),
          string.Format(@"Inversion move is calculated with quality {0}, but actual difference is {4}.
The move would invert the tour {1} between values {2} and {3}.",
          moveMatrix, tour, tour[move.Index1], tour[move.Index2], Math.Abs(afterMatrix - beforeMatrix)));

        beforeMatrix = afterMatrix;
      }
    }

    [TestMethod]
    [TestCategory("Problems.ProbabilisticTravelingSalesman")]
    [TestProperty("Time", "short")]
    public void InsertionMoveEvaluatorTest() {
      Func<int, int, double> distance = (a, b) => distances[a, b];
      double variance;
      var beforeMatrix = EstimatedProbabilisticTravelingSalesmanProblem.Evaluate(tour, distance, realizations, out variance);
      for (var i = 0; i < 500; i++) {
        var move = StochasticTranslocationSingleMoveGenerator.Apply(tour, random);
        var moveMatrix = PTSPEstimatedInsertionMoveEvaluator.EvaluateMove(tour, move, distance, realizations);
        TranslocationManipulator.Apply(tour, move.Index1, move.Index1, move.Index3);
        var afterMatrix = EstimatedProbabilisticTravelingSalesmanProblem.Evaluate(tour, distance, realizations, out variance);

        Assert.IsTrue(Math.Abs(moveMatrix).IsAlmost(Math.Abs(afterMatrix - beforeMatrix)),
          string.Format(@"Insertion move is calculated with quality {0}, but actual difference is {4}.
The move would invert the tour {1} between values {2} and {3}.",
          moveMatrix, tour, tour[move.Index1], tour[move.Index2], Math.Abs(afterMatrix - beforeMatrix)));

        beforeMatrix = afterMatrix;
      }
    }
  }
}
