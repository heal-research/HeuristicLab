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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class KMeansClusteringUtil {
    public static IEnumerable<int> FindClosestCenters(IEnumerable<double[]> centers, IDataset dataset, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows) {
      int nRows = rows.Count();
      int nCols = allowedInputVariables.Count();
      int[] closestCenter = new int[nRows];
      double[] bestCenterDistance = Enumerable.Repeat(double.MaxValue, nRows).ToArray();
      int centerIndex = 1;

      foreach (double[] center in centers) {
        if (nCols != center.Length) throw new ArgumentException();
        int rowIndex = 0;
        foreach (var row in rows) {
          // calc euclidian distance of point to center
          double centerDistance = 0;
          int col = 0;
          foreach (var inputVariable in allowedInputVariables) {
            double d = center[col++] - dataset.GetDoubleValue(inputVariable, row);
            d = d * d; // square;
            centerDistance += d;
            if (centerDistance > bestCenterDistance[rowIndex]) break;
          }
          if (centerDistance < bestCenterDistance[rowIndex]) {
            bestCenterDistance[rowIndex] = centerDistance;
            closestCenter[rowIndex] = centerIndex;
          }
          rowIndex++;
        }
        centerIndex++;
      }
      return closestCenter;
    }

    public static double CalculateIntraClusterSumOfSquares(KMeansClusteringModel model, IDataset dataset, IEnumerable<int> rows) {
      List<int> clusterValues = model.GetClusterValues(dataset, rows).ToList();
      List<string> allowedInputVariables = model.AllowedInputVariables.ToList();
      int nCols = allowedInputVariables.Count;
      Dictionary<int, List<double[]>> clusterPoints = new Dictionary<int, List<double[]>>();
      Dictionary<int, double[]> clusterMeans = new Dictionary<int, double[]>();
      foreach (var clusterValue in clusterValues.Distinct()) {
        clusterPoints.Add(clusterValue, new List<double[]>());
      }

      // collect points of clusters
      int clusterValueIndex = 0;
      foreach (var row in rows) {
        double[] p = new double[allowedInputVariables.Count];
        for (int i = 0; i < nCols; i++) {
          p[i] = dataset.GetDoubleValue(allowedInputVariables[i], row);
        }
        clusterPoints[clusterValues[clusterValueIndex++]].Add(p);
      }
      // calculate cluster means
      foreach (var pair in clusterPoints) {
        double[] mean = new double[nCols];
        foreach (var p in pair.Value) {
          for (int i = 0; i < nCols; i++) {
            mean[i] += p[i];
          }
        }
        for (int i = 0; i < nCols; i++) {
          mean[i] /= pair.Value.Count;
        }
        clusterMeans[pair.Key] = mean;
      }
      // calculate distances
      double allCenterDistances = 0;
      foreach (var pair in clusterMeans) {
        double[] mean = pair.Value;
        double centerDistances = 0;
        foreach (var clusterPoint in clusterPoints[pair.Key]) {
          double centerDistance = 0;
          for (int i = 0; i < nCols; i++) {
            double d = mean[i] - clusterPoint[i];
            d = d * d;
            centerDistance += d;
          }
          centerDistances += centerDistance;
        }
        allCenterDistances += centerDistances;
      }
      return allCenterDistances;
    }
  }
}
