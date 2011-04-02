#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Analysis.Tests {
  [TestClass]
  public class MultidimensionalScalingTest {
    [TestMethod]
    public void TestGoodnessOfFit() {
      double stress;
      DoubleMatrix distances3 = new DoubleMatrix(3, 3);
      // Example 1: A right triangle
      distances3[0, 1] = distances3[1, 0] = 3;
      distances3[0, 2] = distances3[2, 0] = 4;
      distances3[1, 2] = distances3[2, 1] = 5;
      stress = MultidimensionalScaling.CalculateNormalizedStress(distances3,
        MultidimensionalScaling.KruskalShepard(distances3));
      Assert.IsTrue(stress < 0.1);
      // Example 2: An arbitrary triangle
      distances3[0, 1] = distances3[1, 0] = 8;
      distances3[0, 2] = distances3[2, 0] = 6.4;
      distances3[1, 2] = distances3[2, 1] = 5;
      stress = MultidimensionalScaling.CalculateNormalizedStress(distances3,
        MultidimensionalScaling.KruskalShepard(distances3));
      Assert.IsTrue(stress < 0.1);
      DoubleMatrix distances4 = new DoubleMatrix(4, 4);
      // Example 3: A small square
      distances4[0, 1] = distances4[1, 0] = 1;
      distances4[0, 2] = distances4[2, 0] = Math.Sqrt(2);
      distances4[0, 3] = distances4[3, 0] = 1;
      distances4[1, 2] = distances4[2, 1] = 1;
      distances4[1, 3] = distances4[3, 1] = Math.Sqrt(2);
      distances4[2, 3] = distances4[3, 2] = 1;
      stress = MultidimensionalScaling.CalculateNormalizedStress(distances4,
        MultidimensionalScaling.KruskalShepard(distances4));
      Assert.IsTrue(stress < 0.1);
      // Example 4: A large square
      distances4[0, 1] = distances4[1, 0] = 1000;
      distances4[0, 2] = distances4[2, 0] = Math.Sqrt(2000000);
      distances4[0, 3] = distances4[3, 0] = 1000;
      distances4[1, 2] = distances4[2, 1] = 1000;
      distances4[1, 3] = distances4[3, 1] = Math.Sqrt(2000000);
      distances4[2, 3] = distances4[3, 2] = 1000;
      stress = MultidimensionalScaling.CalculateNormalizedStress(distances4,
        MultidimensionalScaling.KruskalShepard(distances4));
      Assert.IsTrue(stress < 0.1);
      // Example 5: An arbitrary cloud of 8 points in a plane
      DoubleMatrix distancesK = GetDistances(new double[,] { { 2, 1 }, { 5, 2 }, { 7, 1 }, { 4, 0 }, { 3, 3 }, { 4, 2 }, { 1, 8 }, { 6, 3 } });
      stress = MultidimensionalScaling.CalculateNormalizedStress(distancesK,
        MultidimensionalScaling.KruskalShepard(distancesK));
      Assert.IsTrue(stress < 0.1);
      // Example 6: A tetrahedron
      distancesK = GetDistances(new double[,] { { 0, 0, 0 }, { 4, 0, 0 }, { 2, 3.4641, 0 }, { 2, 1.1547, 3.2660 } });
      stress = MultidimensionalScaling.CalculateNormalizedStress(distancesK,
        MultidimensionalScaling.KruskalShepard(distancesK));
      Assert.IsTrue(stress < 0.1);
    }

    internal DoubleMatrix GetDistances(double[,] coordinates) {
      int dimension = coordinates.GetLength(0);
      DoubleMatrix distances = new DoubleMatrix(dimension, dimension);
      for (int i = 0; i < dimension - 1; i++)
        for (int j = i + 1; j < dimension; j++) {
          double sum = 0;
          for (int k = 0; k < coordinates.GetLength(1); k++)
            sum += (coordinates[i, k] - coordinates[j, k]) * (coordinates[i, k] - coordinates[j, k]);
          distances[i, j] = distances[j, i] = Math.Sqrt(sum);
        }
      return distances;
    }
  }
}
