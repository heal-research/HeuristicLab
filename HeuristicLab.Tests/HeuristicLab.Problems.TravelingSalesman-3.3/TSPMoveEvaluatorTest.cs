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

using HeuristicLab.Common;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.TravelingSalesman.Tests {
  /// <summary>
  ///This is a test class for TSP move evaluators
  ///</summary>
  [TestClass()]
  public class TSPMoveEvaluatorTest {
    private const int ProblemSize = 10;
    private static double[,] coordinates;
    private static double[,] distances;
    private static Permutation tour;
    private static MersenneTwister random;

    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      random = new MersenneTwister();
      coordinates = new double[ProblemSize, 2];
      for (int i = 0; i < ProblemSize; i++) {
        coordinates[i, 0] = random.Next(ProblemSize * 10);
        coordinates[i, 1] = random.Next(ProblemSize * 10);
      }
      distances = new double[ProblemSize, ProblemSize];
      for (int i = 0; i < ProblemSize-1; i++)
        for (int j = i + 1; j < ProblemSize; j++) {
          distances[i, j] = DistanceHelper.EuclideanDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
          distances[j, i] = distances[i, j];
        }
      tour = new Permutation(PermutationTypes.RelativeUndirected, ProblemSize, random);
    }

    [TestMethod]
    [TestCategory("Problems.TravelingSalesman")]
    [TestProperty("Time", "short")]
    public void InversionMoveEvaluatorTest() {
      var euclidTSP = new TSP() { TSPData = new EuclideanTSPData("test", coordinates) };
      var matrixTSP = new TSP() { TSPData = new MatrixTSPData("test", distances) };
      double beforeMatrix = matrixTSP.Evaluate(tour);
      double beforeCoordinates = euclidTSP.Evaluate(tour);
      Assert.IsTrue(beforeMatrix.IsAlmost(beforeCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

      for (int i = 0; i < 500; i++) {
        var move = StochasticInversionSingleMoveGenerator.Apply(tour, random);

        double moveMatrix = TSPInversionMoveEvaluator.CalculateTourLengthDelta(matrixTSP.TSPData, tour, move);
        double moveCoordinates = TSPInversionMoveEvaluator.CalculateTourLengthDelta(euclidTSP.TSPData, tour, move);
        Assert.IsTrue(moveMatrix.IsAlmost(moveCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

        string failureString = string.Format(@"Inversion move is calculated with quality {0}, but actual difference is {4}.
The move would invert the tour {1} between values {2} and {3}.", moveMatrix.ToString(), tour.ToString(), tour[move.Index1].ToString(), tour[move.Index2].ToString(), "{0}");

        InversionManipulator.Apply(tour, move.Index1, move.Index2);

        double afterMatrix = matrixTSP.Evaluate(tour);
        double afterCoordinates = euclidTSP.Evaluate(tour);
        Assert.IsTrue(afterMatrix.IsAlmost(afterCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

        Assert.IsTrue(moveMatrix.IsAlmost(afterMatrix - beforeMatrix), string.Format(failureString, (afterMatrix - beforeMatrix).ToString()));

        beforeMatrix = afterMatrix;
        beforeCoordinates = afterCoordinates;
      }
    }

    [TestMethod]
    [TestCategory("Problems.TravelingSalesman")]
    [TestProperty("Time", "short")]
    public void TranslocationMoveEvaluatorTest() {

      var euclidTSP = new TSP() { TSPData = new EuclideanTSPData("test", coordinates) };
      var matrixTSP = new TSP() { TSPData = new MatrixTSPData("test", distances) };
      double beforeMatrix = matrixTSP.Evaluate(tour);
      double beforeCoordinates = euclidTSP.Evaluate(tour);
      Assert.IsTrue(beforeMatrix.IsAlmost(beforeCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

      for (int i = 0; i < 500; i++) {
        var move = StochasticTranslocationSingleMoveGenerator.Apply(tour, random);

        double moveMatrix = TSPTranslocationMoveEvaluator.CalculateTourLengthDelta(matrixTSP.TSPData, tour, move);
        double moveCoordinates = TSPTranslocationMoveEvaluator.CalculateTourLengthDelta(euclidTSP.TSPData, tour, move);
        Assert.IsTrue(moveMatrix.IsAlmost(moveCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

        string failureString = string.Format(@"Translocation move is calculated with quality {0}, but actual difference is {5}.
The move would move the segment between {1} and {2} in the tour {3} to the new index {4}.", moveMatrix.ToString(), tour[move.Index1].ToString(), tour[move.Index2].ToString(), tour.ToString(), move.Index3.ToString(), "{0}");
        TranslocationManipulator.Apply(tour, move.Index1, move.Index2, move.Index3);

        double afterMatrix = matrixTSP.Evaluate(tour);
        double afterCoordinates = euclidTSP.Evaluate(tour);
        Assert.IsTrue(afterMatrix.IsAlmost(afterCoordinates), "Evaluation differs between using the coordinates and using the distance matrix.");

        Assert.IsTrue(moveMatrix.IsAlmost(afterMatrix - beforeMatrix), string.Format(failureString, (afterMatrix - beforeMatrix).ToString()));

        beforeMatrix = afterMatrix;
        beforeCoordinates = afterCoordinates;
      }
    }

  }
}
