#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Optimization {
  public static class HypervolumeCalculator {
    public static double[] CalculateNadirPoint(IEnumerable<double[]> qualities, bool[] maximization) {
      var res = maximization.Select(m => m ? double.MaxValue : double.MinValue).ToArray();
      foreach (var quality in qualities)
        for (var i = 0; i < quality.Length; i++)
          if (maximization[i] == res[i] > quality[i])
            res[i] = quality[i];
      return res;
    }

    /// <summary>
    /// The Hypervolume-metric is defined as the HypervolumeCalculator enclosed between a given reference point,
    /// that is fixed for every evaluation function and the evaluated qualities.
    /// 
    /// Example:
    /// r is the reference point at (1|1) and every point p is part of the evaluated qualities
    /// The filled area labled HV is the 2 dimensional HypervolumeCalculator enclosed by this qualities. 
    /// 
    /// (0|1)                (1|1)
    ///   +      +-------------r
    ///   |      |###### HV ###|
    ///   |      p------+######|
    ///   |             p+#####|
    ///   |              |#####|
    ///   |              p-+###|
    ///   |                p---+
    ///   |                 
    ///   +--------------------1
    /// (0|0)                (1|0)
    /// 
    ///  Please note that in this example both dimensions are minimized. The reference point needs to be dominated by EVERY point in the evaluated qualities 
    /// 
    /// </summary>
    /// 
    public static double CalculateHypervolume(IList<double[]> qualities, double[] referencePoint, bool[] maximization) {
      qualities = qualities.Where(vec => DominationCalculator.Dominates(vec, referencePoint, maximization, false) == DominationResult.Dominates).ToArray();
      if (qualities.Count== 0) return 0; //TODO computation for negative hypervolume?
      if (maximization.Length == 2)
        return Calculate2D(qualities, referencePoint, maximization);

      if (Array.TrueForAll(maximization, x => !x))
        return CalculateMultiDimensional(qualities, referencePoint);
      throw new NotImplementedException("HypervolumeCalculator calculation for more than two dimensions is supported only with minimization problems.");
    }


    /// <summary>
    /// Caluclates the Hypervolume for a 2 dimensional problem
    /// </summary>
    /// <param name="front">All points within the front need to be Non-Dominated and need to dominate the reference point</param>
    /// <param name="referencePoint"></param>
    /// <param name="maximization"></param>
    /// <returns></returns>
    public static double Calculate2D(IList<double[]> front, double[] referencePoint, bool[] maximization) {
      if (front == null) throw new ArgumentNullException("front");
      if (referencePoint == null) throw new ArgumentNullException("referencePoint");
      if (maximization == null) throw new ArgumentNullException("maximization");
      if (!front.Any()) throw new ArgumentException("Front must not be empty.");
      if (referencePoint.Length != 2) throw new ArgumentException("ReferencePoint must have exactly two dimensions.");

      var set = front.ToArray();
      if (set.Any(s => s.Length != 2)) throw new ArgumentException("Points in qualities must have exactly two dimensions.");

      Array.Sort(set, new DimensionComparer(0, maximization[0]));

      double sum = 0;
      for (var i = 0; i < set.Length - 1; i++)
        sum += Math.Abs(set[i][0] - set[i + 1][0]) * Math.Abs(set[i][1] - referencePoint[1]);
      var lastPoint = set[set.Length - 1];
      sum += Math.Abs(lastPoint[0] - referencePoint[0]) * Math.Abs(lastPoint[1] - referencePoint[1]);

      return sum;
    }

    public static double CalculateMultiDimensional(IList<double[]> front, double[] referencePoint) {
      if (referencePoint == null || referencePoint.Length < 3) throw new ArgumentException("ReferencePoint unfit for complex HypervolumeCalculator calculation");

      var objectives = referencePoint.Length;
      var fronList = front.ToList();
      fronList.StableSort(new DimensionComparer(objectives - 1, false));

      var regLow = Enumerable.Repeat(1E15, objectives).ToArray();
      foreach (var p in fronList) {
        for (var i = 0; i < regLow.Length; i++) {
          if (p[i] < regLow[i]) regLow[i] = p[i];
        }
      }

      return Stream(regLow, referencePoint, fronList, 0, referencePoint[objectives - 1], (int)Math.Sqrt(fronList.Count), objectives);
    }


    //within Stream a number of equality comparisons on double values are performed
    //this is intentional and required
    private static double Stream(double[] regionLow, double[] regionUp, List<double[]> front, int split, double cover, int sqrtNoPoints, int objectives) {
      var coverOld = cover;
      var coverIndex = 0;
      var coverIndexOld = -1;
      int c;
      double result = 0;

      var dMeasure = GetMeasure(regionLow, regionUp, objectives);
      while (cover == coverOld && coverIndex < front.Count()) {
        if (coverIndexOld == coverIndex) break;
        coverIndexOld = coverIndex;
        if (Covers(front[coverIndex], regionLow, objectives)) {
          cover = front[coverIndex][objectives - 1];
          result += dMeasure * (coverOld - cover);
        } else coverIndex++;
      }

      for (c = coverIndex; c > 0; c--) if (front[c - 1][objectives - 1] == cover) coverIndex--;
      if (coverIndex == 0) return result;

      var allPiles = true;
      var piles = new int[coverIndex];
      for (var i = 0; i < coverIndex; i++) {
        piles[i] = IsPile(front[i], regionLow, objectives);
        if (piles[i] == -1) {
          allPiles = false;
          break;
        }
      }

      if (allPiles) {
        var trellis = new double[regionUp.Length];
        for (var j = 0; j < trellis.Length; j++) trellis[j] = regionUp[j];
        double next;
        var i = 0;
        do {
          var current = front[i][objectives - 1];
          do {
            if (front[i][piles[i]] < trellis[piles[i]]) trellis[piles[i]] = front[i][piles[i]];
            i++;
            if (i < coverIndex) next = front[i][objectives - 1];
            else {
              next = cover;
              break;
            }
          }
          while (next == current);
          result += ComputeTrellis(regionLow, regionUp, trellis, objectives) * (next - current);
        }
        while (next != cover);
      } else {
        double bound = -1;
        var boundaries = new double[coverIndex];
        var noBoundaries = new double[coverIndex];
        var boundIdx = 0;
        var noBoundIdx = 0;

        do {
          for (var i = 0; i < coverIndex; i++) {
            var contained = ContainesBoundary(front[i], regionLow, split);
            if (contained == 0) boundaries[boundIdx++] = front[i][split];
            else if (contained == 1) noBoundaries[noBoundIdx++] = front[i][split];
          }
          if (boundIdx > 0) bound = GetMedian(boundaries, boundIdx);
          else if (noBoundIdx > sqrtNoPoints) bound = GetMedian(noBoundaries, noBoundIdx);
          else split++;
        }
        while (bound == -1.0);

        var pointsChildLow = new List<double[]>();
        var pointsChildUp = new List<double[]>();
        var regionUpC = new double[regionUp.Length];
        for (var j = 0; j < regionUpC.Length; j++) regionUpC[j] = regionUp[j];
        var regionLowC = new double[regionLow.Length];
        for (var j = 0; j < regionLowC.Length; j++) regionLowC[j] = regionLow[j];

        for (var i = 0; i < coverIndex; i++) {
          if (PartCovers(front[i], regionUpC, objectives)) pointsChildUp.Add(front[i]);
          if (PartCovers(front[i], regionUp, objectives)) pointsChildLow.Add(front[i]);
        }

        if (pointsChildUp.Count > 0) result += Stream(regionLow, regionUpC, pointsChildUp, split, cover, sqrtNoPoints, objectives);
        if (pointsChildLow.Count > 0) result += Stream(regionLowC, regionUp, pointsChildLow, split, cover, sqrtNoPoints, objectives);
      }
      return result;
    }

    private static double GetMedian(double[] vector, int length) {
      return vector.Take(length).Median();
    }

    private static double ComputeTrellis(double[] regionLow, double[] regionUp, double[] trellis, int objectives) {
      var bs = new bool[objectives - 1];
      for (var i = 0; i < bs.Length; i++) bs[i] = true;

      double result = 0;
      var noSummands = BinarayToInt(bs);
      for (uint i = 1; i <= noSummands; i++) {
        double summand = 1;
        IntToBinary(i, bs);
        var oneCounter = 0;
        for (var j = 0; j < objectives - 1; j++) {
          if (bs[j]) {
            summand *= regionUp[j] - trellis[j];
            oneCounter++;
          } else {
            summand *= regionUp[j] - regionLow[j];
          }
        }
        if (oneCounter % 2 == 0) result -= summand;
        else result += summand;
      }
      return result;
    }

    private static void IntToBinary(uint i, bool[] bs) {
      for (var j = 0; j < bs.Length; j++) bs[j] = false;
      var rest = i;
      var idx = 0;
      while (rest != 0) {
        bs[idx] = rest % 2 == 1;
        rest = rest / 2;
        idx++;
      }
    }

    private static uint BinarayToInt(bool[] bs) {
      uint result = 0;
      for (var i = 0; i < bs.Length; i++) {
        result += bs[i] ? ((uint)1 << i) : 0;
      }
      return result;
    }

    private static int IsPile(double[] cuboid, double[] regionLow, int objectives) {
      var pile = cuboid.Length;
      for (var i = 0; i < objectives - 1; i++) {
        if (cuboid[i] > regionLow[i]) {
          if (pile != objectives) return 1;
          pile = i;
        }
      }
      return pile;
    }

    private static double GetMeasure(double[] regionLow, double[] regionUp, int objectives) {
      double volume = 1;
      for (var i = 0; i < objectives - 1; i++) {
        volume *= (regionUp[i] - regionLow[i]);
      }
      return volume;
    }

    private static int ContainesBoundary(double[] cub, double[] regionLow, int split) {
      if (regionLow[split] >= cub[split]) return -1;
      else {
        for (var j = 0; j < split; j++) {
          if (regionLow[j] < cub[j]) return 1;
        }
      }
      return 0;
    }

    private static bool PartCovers(double[] v, double[] regionUp, int objectives) {
      for (var i = 0; i < objectives - 1; i++) {
        if (v[i] >= regionUp[i]) return false;
      }
      return true;
    }

    private static bool Covers(double[] v, double[] regionLow, int objectives) {
      for (var i = 0; i < objectives - 1; i++) {
        if (v[i] > regionLow[i]) return false;
      }
      return true;
    }

    private class DimensionComparer : IComparer<double[]> {
      private readonly int dimension;
      private readonly int descending;

      public DimensionComparer(int dimension, bool descending) {
        this.dimension = dimension;
        this.descending = descending ? -1 : 1;
      }

      public int Compare(double[] x, double[] y) {
        return x[dimension].CompareTo(y[dimension]) * descending;
      }
    }
  }
}