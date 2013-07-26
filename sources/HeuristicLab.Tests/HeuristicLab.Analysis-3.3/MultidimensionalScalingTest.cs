#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using HeuristicLab.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Analysis.Tests {
  [TestClass]
  public class MultidimensionalScalingTest {
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
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
      DoubleMatrix coords3 = MultidimensionalScaling.KruskalShepard(distances3);
      Console.WriteLine("Coordinates: ");
      Console.WriteLine("A = ({0}, {1}), B = ({2}, {3}), C = ({4}, {5})", coords3[0, 0], coords3[0, 1], coords3[1, 0], coords3[1, 1], coords3[2, 0], coords3[2, 1]);
      stress = MultidimensionalScaling.CalculateNormalizedStress(distances3, coords3);
      Console.WriteLine("Stress = " + stress.ToString(CultureInfo.InvariantCulture.NumberFormat));
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
      // Example 7: A matrix of perceived dissimilarities between 14 colors, published in the literature
      distancesK = new DoubleMatrix(new double[,] {
{ 0.00, 0.14, 0.58, 0.58, 0.82, 0.94, 0.93, 0.96, 0.98, 0.93, 0.91, 0.88, 0.87, 0.84 },
{ 0.14, 0.00, 0.50, 0.56, 0.78, 0.91, 0.93, 0.93, 0.98, 0.96, 0.93, 0.89, 0.87, 0.86 },
{ 0.58, 0.50, 0.00, 0.19, 0.53, 0.83, 0.90, 0.92, 0.98, 0.99, 0.98, 0.99, 0.95, 0.97 },
{ 0.58, 0.56, 0.19, 0.00, 0.46, 0.75, 0.90, 0.91, 0.98, 0.99, 1.00, 0.99, 0.98, 0.96 },
{ 0.82, 0.78, 0.53, 0.46, 0.00, 0.39, 0.69, 0.74, 0.93, 0.98, 0.98, 0.99, 0.98, 1.00 },
{ 0.94, 0.91, 0.83, 0.75, 0.39, 0.00, 0.38, 0.55, 0.86, 0.92, 0.98, 0.98, 0.98, 0.99 },
{ 0.93, 0.93, 0.90, 0.90, 0.69, 0.38, 0.00, 0.27, 0.78, 0.86, 0.95, 0.98, 0.98, 1.00 },
{ 0.96, 0.93, 0.92, 0.91, 0.74, 0.55, 0.27, 0.00, 0.67, 0.81, 0.96, 0.97, 0.98, 0.98 },
{ 0.98, 0.98, 0.98, 0.98, 0.93, 0.86, 0.78, 0.67, 0.00, 0.42, 0.63, 0.73, 0.80, 0.77 },
{ 0.93, 0.96, 0.99, 0.99, 0.98, 0.92, 0.86, 0.81, 0.42, 0.00, 0.26, 0.50, 0.59, 0.72 },
{ 0.91, 0.93, 0.98, 1.00, 0.98, 0.98, 0.95, 0.96, 0.63, 0.26, 0.00, 0.24, 0.38, 0.45 },
{ 0.88, 0.89, 0.99, 0.99, 0.99, 0.98, 0.98, 0.97, 0.73, 0.50, 0.24, 0.00, 0.15, 0.32 },
{ 0.87, 0.87, 0.95, 0.98, 0.98, 0.98, 0.98, 0.98, 0.80, 0.59, 0.38, 0.15, 0.00, 0.24 },
{ 0.84, 0.86, 0.97, 0.96, 1.00, 0.99, 1.00, 0.98, 0.77, 0.72, 0.45, 0.32, 0.24, 0.00 }});
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
