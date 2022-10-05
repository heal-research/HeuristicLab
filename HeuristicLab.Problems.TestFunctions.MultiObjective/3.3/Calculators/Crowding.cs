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

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  /// <summary>
  /// Crowding distance d(x,A) is usually defined between a point x and a set of points A
  /// d(x,A) is then a weighted sum over all dimensions where for each dimension the next larger and the next smaller Point to x are subtracted
  /// I extended the concept and defined the Crowding distance of a front A as the mean of the crowding distances of every point x in A 
  /// C(A) = mean(d(x,A)) where x in A  and d(x,A) is not infinite
  /// Beware that Crowding is not normalized for the number of dimensions. A higher number of dimensions normlly indicated higher expected Crowding values
  /// </summary>
  public static class Crowding {

    public static double Calculate(IEnumerable<double[]> front, double[,] bounds) {
      return GetForFront(front, bounds).Where(d => !double.IsPositiveInfinity(d)).DefaultIfEmpty(double.PositiveInfinity).Average();
    }

    public static IEnumerable<double> GetForFront(IEnumerable<double[]> front, double[,] bounds) {
      if (front == null) throw new ArgumentException("Fronts must not be null");
      if (!front.Any()) throw new ArgumentException("Fronts must not be empty");
      if (bounds == null) throw new ArgumentException("Bounds must not be null");
      double[] pointsums = new double[front.Count()];

      for (int dim = 0; dim < front.First().Length; dim++) {

        double[] arr = front.Select(x => x[dim]).ToArray();
        Array.Sort(arr);

        double fmax = bounds[dim % bounds.GetLength(0), 1];
        double fmin = bounds[dim % bounds.GetLength(0), 0];

        int pointIdx = 0;
        foreach (double[] point in front) {
          double d = 0;
          int pos = Array.BinarySearch(arr, point[dim]);
          if (pos != 0 && pos != arr.Count() - 1) {
            d = (arr[pos + 1] - arr[pos - 1]) / (fmax - fmin);
            pointsums[pointIdx++] += d;
          } else {
            pointsums[pointIdx++] = Double.PositiveInfinity;
          }
        }
      }
      return pointsums;
    }

    public static double GetForSinglePoints(IEnumerable<double[]> front, double[,] bounds, int pointIndex) {
      if (front == null) throw new ArgumentException("Fronts must not be null");
      if (!front.Any()) throw new ArgumentException("Fronts must not be empty");
      if (bounds == null) throw new ArgumentException("Bounds must not be null");
      if (pointIndex < 0 || front.Count() <= pointIndex) throw new ArgumentException("PointIndex is not valid ");
      double pointsum = 0;
      double[] point = front.ElementAt(pointIndex);
      for (int dim = 0; dim < front.First().Length; dim++) {

        double[] arr = front.Select(x => x[dim]).ToArray();
        Array.Sort(arr);

        double fmax = bounds[dim % bounds.GetLength(0), 1];
        double fmin = bounds[dim % bounds.GetLength(0), 0];

        int pointIdx = pointIndex;

        int pos = Array.BinarySearch(arr, point[dim]);
        if (pos != 0 && pos != arr.Count() - 1) {
          double d = (arr[pos + 1] - arr[pos - 1]) / (fmax - fmin);
          pointsum += d;
        }

      }
      return pointsum;
    }

  }
}
