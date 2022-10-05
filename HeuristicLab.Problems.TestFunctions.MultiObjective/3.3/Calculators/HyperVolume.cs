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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  public static class Hypervolume {

    /// <summary>
    /// The Hyprevolume-metric is defined as the Hypervolume enclosed between a given reference point,
    /// that is fixed for every evaluation function and the evaluated front.
    /// 
    /// Example:
    /// r is the reference Point at (1|1) and every Point p is part of the evaluated front
    /// The filled Area labled HV is the 2 dimensional Hypervolume enclosed by this front. 
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
    ///  Please note that in this example both dimensions are minimized. The reference Point need to be dominated by EVERY point in the evaluated front 
    /// 
    /// </summary>
    /// 
    public static double Calculate(IEnumerable<double[]> front, double[] referencePoint, bool[] maximization) {
      front = NonDominatedSelect.GetDominatingVectors(front, referencePoint, maximization, false);
      if (!front.Any()) throw new ArgumentException("No point in the front dominates the referencePoint");
      if (maximization.Length == 2)
        return Calculate2D(front, referencePoint, maximization);

      if (Array.TrueForAll(maximization, x => !x))
        return CalculateMulitDimensional(front, referencePoint);
      else throw new NotImplementedException("Hypervolume calculation for more than two dimensions is supported only with minimization problems.");
    }


    private static double Calculate2D(IEnumerable<double[]> front, double[] referencePoint, bool[] maximization) {
      if (front == null) throw new ArgumentNullException("Front must not be null.");
      if (!front.Any()) throw new ArgumentException("Front must not be empty.");

      if (referencePoint == null) throw new ArgumentNullException("ReferencePoint must not be null.");
      if (referencePoint.Length != 2) throw new ArgumentException("ReferencePoint must have exactly two dimensions.");

      double[][] set = front.ToArray();
      if (set.Any(s => s.Length != 2)) throw new ArgumentException("Points in front must have exactly two dimensions.");

      Array.Sort<double[]>(set, new Utilities.DimensionComparer(0, maximization[0]));

      double sum = 0;
      for (int i = 0; i < set.Length - 1; i++) {
        sum += Math.Abs((set[i][0] - set[i + 1][0])) * Math.Abs((set[i][1] - referencePoint[1]));
      }

      double[] lastPoint = set[set.Length - 1];
      sum += Math.Abs(lastPoint[0] - referencePoint[0]) * Math.Abs(lastPoint[1] - referencePoint[1]);

      return sum;
    }



    private static double CalculateMulitDimensional(IEnumerable<double[]> front, double[] referencePoint) {
      if (referencePoint == null || referencePoint.Length < 3) throw new ArgumentException("ReferencePoint unfit for complex Hypervolume calculation");

      int objectives = referencePoint.Length;
      var fronList = front.ToList();
      fronList.StableSort(new Utilities.DimensionComparer(objectives - 1, false));

      double[] regLow = Enumerable.Repeat(1E15, objectives).ToArray();
      foreach (double[] p in fronList) {
        for (int i = 0; i < regLow.Length; i++) {
          if (p[i] < regLow[i]) regLow[i] = p[i];
        }
      }

      return Stream(regLow, referencePoint, fronList, 0, referencePoint[objectives - 1], (int)Math.Sqrt(fronList.Count), objectives);
    }

    private static double Stream(double[] regionLow, double[] regionUp, List<double[]> front, int split, double cover, int sqrtNoPoints, int objectives) {
      double coverOld = cover;
      int coverIndex = 0;
      int coverIndexOld = -1;
      int c;
      double result = 0;

      double dMeasure = GetMeasure(regionLow, regionUp, objectives);
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

      bool allPiles = true;
      int[] piles = new int[coverIndex];
      for (int i = 0; i < coverIndex; i++) {
        piles[i] = IsPile(front[i], regionLow, regionUp, objectives);
        if (piles[i] == -1) {
          allPiles = false;
          break;
        }
      }

      if (allPiles) {
        double[] trellis = new double[regionUp.Length];
        for (int j = 0; j < trellis.Length; j++) trellis[j] = regionUp[j];
        double current = 0;
        double next = 0;
        int i = 0;
        do {
          current = front[i][objectives - 1];
          do {
            if (front[i][piles[i]] < trellis[piles[i]]) trellis[piles[i]] = front[i][piles[i]];
            i++;
            if (i < coverIndex) next = front[i][objectives - 1];
            else { next = cover; break; }
          } while (next == current);
          result += ComputeTrellis(regionLow, regionUp, trellis, objectives) * (next - current);
        } while (next != cover);
      } else {
        double bound = -1;
        double[] boundaries = new double[coverIndex];
        double[] noBoundaries = new double[coverIndex];
        int boundIdx = 0;
        int noBoundIdx = 0;

        do {
          for (int i = 0; i < coverIndex; i++) {
            int contained = ContainesBoundary(front[i], regionLow, split);
            if (contained == 0) boundaries[boundIdx++] = front[i][split];
            else if (contained == 1) noBoundaries[noBoundIdx++] = front[i][split];
          }
          if (boundIdx > 0) bound = GetMedian(boundaries, boundIdx);
          else if (noBoundIdx > sqrtNoPoints) bound = GetMedian(noBoundaries, noBoundIdx);
          else split++;
        } while (bound == -1.0);

        List<double[]> pointsChildLow, pointsChildUp;
        pointsChildLow = new List<double[]>();
        pointsChildUp = new List<double[]>();
        double[] regionUpC = new double[regionUp.Length];
        for (int j = 0; j < regionUpC.Length; j++) regionUpC[j] = regionUp[j];
        double[] regionLowC = new double[regionLow.Length];
        for (int j = 0; j < regionLowC.Length; j++) regionLowC[j] = regionLow[j];

        for (int i = 0; i < coverIndex; i++) {
          if (PartCovers(front[i], regionUpC, objectives)) pointsChildUp.Add(front[i]);
          if (PartCovers(front[i], regionUp, objectives)) pointsChildLow.Add(front[i]);
        }
        //this could/should be done in Parallel

        if (pointsChildUp.Count > 0) result += Stream(regionLow, regionUpC, pointsChildUp, split, cover, sqrtNoPoints, objectives);
        if (pointsChildLow.Count > 0) result += Stream(regionLowC, regionUp, pointsChildLow, split, cover, sqrtNoPoints, objectives);
      }
      return result;
    }

    private static double GetMedian(double[] vector, int length) {
      return vector.Take(length).Median();
    }

    private static double ComputeTrellis(double[] regionLow, double[] regionUp, double[] trellis, int objectives) {
      bool[] bs = new bool[objectives - 1];
      for (int i = 0; i < bs.Length; i++) bs[i] = true;

      double result = 0;
      uint noSummands = BinarayToInt(bs);
      int oneCounter; double summand;
      for (uint i = 1; i <= noSummands; i++) {
        summand = 1;
        IntToBinary(i, bs);
        oneCounter = 0;
        for (int j = 0; j < objectives - 1; j++) {
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
      for (int j = 0; j < bs.Length; j++) bs[j] = false;
      uint rest = i;
      int idx = 0;
      while (rest != 0) {
        bs[idx] = rest % 2 == 1;
        rest = rest / 2;
        idx++;
      }

    }

    private static uint BinarayToInt(bool[] bs) {
      uint result = 0;
      for (int i = 0; i < bs.Length; i++) {
        result += bs[i] ? ((uint)1 << i) : 0;
      }
      return result;
    }

    private static int IsPile(double[] cuboid, double[] regionLow, double[] regionUp, int objectives) {
      int pile = cuboid.Length;
      for (int i = 0; i < objectives - 1; i++) {
        if (cuboid[i] > regionLow[i]) {
          if (pile != objectives) return 1;
          pile = i;
        }
      }
      return pile;
    }

    private static double GetMeasure(double[] regionLow, double[] regionUp, int objectives) {
      double volume = 1;
      for (int i = 0; i < objectives - 1; i++) {
        volume *= (regionUp[i] - regionLow[i]);
      }
      return volume;
    }

    private static int ContainesBoundary(double[] cub, double[] regionLow, int split) {
      if (regionLow[split] >= cub[split]) return -1;
      else {
        for (int j = 0; j < split; j++) {
          if (regionLow[j] < cub[j]) return 1;
        }
      }
      return 0;
    }

    private static bool PartCovers(double[] v, double[] regionUp, int objectives) {
      for (int i = 0; i < objectives - 1; i++) {
        if (v[i] >= regionUp[i]) return false;
      }
      return true;
    }

    private static bool Covers(double[] v, double[] regionLow, int objectives) {
      for (int i = 0; i < objectives - 1; i++) {
        if (v[i] > regionLow[i]) return false;
      }
      return true;
    }


  }
}

