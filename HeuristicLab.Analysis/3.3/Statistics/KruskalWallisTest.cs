#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Analysis.Statistics {
  public static class KruskalWallisTest {
    /// <summary>
    /// Performs the Kruskal-Wallis test and returns the p-Value. 
    /// (source based on R's kruskal.test(), GNU GPL)
    /// </summary>
    public static double Test(double[][] data) {
      double[] g;
      double[] x = FlattenArray(data, out g);
      int n = x.Length;
      int parameter = data.Length - 1;

      int[] r = Rank(x);
      double[][] ties = CountDuplicates(x);
      double statistic = CalculateStatistic(r, g);
      double tiesCorrection = CalculateTiesCorrection(ties);
      statistic = ((12 * statistic / (n * (n + 1)) - 3 * (n + 1)) /
                  (1 - tiesCorrection / (Math.Pow(n, 3) - n)));

      return alglib.chisquarecdistribution(parameter, statistic);
    }

    private static double CalculateStatistic(int[] r, double[] g) {
      double result = 0.0;
      double lastG = g[0];
      double curSum = 0.0;
      int cnt = 0;
      for (int i = 0; i < r.Length; i++) {
        if (lastG.IsAlmost(g[i])) {
          curSum += r[i];
          cnt++;
        } else {
          double sum = Math.Pow(curSum, 2);
          sum /= cnt;
          result += sum;

          lastG = g[i];
          curSum = r[i];
          cnt = 1;
        }
      }

      double lastSum = Math.Pow(curSum, 2);
      lastSum /= cnt;
      result += lastSum;

      return result;
    }

    private static double CalculateTiesCorrection(double[][] ties) {
      double sum = 0.0;
      for (int i = 0; i < ties[1].Length; i++) {
        double tic = Math.Pow(ties[1][i], 3) - ties[1][i];
        sum += tic;
      }
      return sum;
    }

    private static double[] FlattenArray(double[][] x, out double[] indizes) {
      int compLenght = 0;
      for (int i = 0; i < x.Length; i++) {
        compLenght += x[i].Length;
      }

      double[] result = new double[compLenght];
      indizes = new double[compLenght];

      int resultPos = 0;
      for (int i = 0; i < x.Length; i++) {
        Array.Copy(x[i], 0, result, resultPos, x[i].Length);

        for (int j = resultPos; j < resultPos + x[i].Length; j++) {
          indizes[j] = i;
        }
        resultPos += x[i].Length;
      }

      return result;
    }

    private static double[][] CountDuplicates(double[] x) {
      List<double> number = new List<double>();
      List<double> cnt = new List<double>();
      double[] sortedX = new double[x.Length];

      Array.Copy(x, sortedX, x.Length);
      Array.Sort(sortedX);

      double last = x[0];
      number.Add(x[0]);
      cnt.Add(1);

      for (int i = 1; i < x.Length; i++) {
        if (!last.IsAlmost(x[i])) {
          number.Add(x[i]);
          last = x[i];
          cnt.Add(1);
        } else {
          cnt[cnt.Count - 1] += 1;
        }
      }

      double[][] result = new double[2][];
      result[0] = number.ToArray();
      result[1] = cnt.ToArray();

      return result;
    }

    private static int[] Rank(double[] x) {
      double[] keys = new double[x.Length];
      int[] items = new int[x.Length];
      int[] ranks = new int[x.Length];

      Array.Copy(x, keys, x.Length);
      for (int i = 0; i < x.Length; i++) items[i] = i;

      Array.Sort(keys, items);

      for (int i = 0; i < x.Length; i++) {
        ranks[items[i]] = i + 1;
      }
      return ranks;
    }
  }
}
