#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Random;
using HeuristicLab.Core;

namespace HeuristicLab.StatisticalAnalysis {
  public class MannWhitneyWilcoxonTest : IEditable {

    public IView CreateView() {
      return CreateEditor();
    }

    public IEditor CreateEditor() {
      return new MannWhitneyWilcoxonTestControl();
    }

    /// <summary>
    /// Calculates the p-value of a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test).
    /// The test performs a ranking of the data and returns the p-value indicating the level of significance
    /// at which the hypothesis H0 can be rejected.
    /// Caution: This method approximates the ranks in p1 and p2 with a normal distribution and should only be called
    /// when the sample size is at least 10 in both samples.
    /// 
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <returns>The p-value of the test</returns>
    public static double TwoTailedTest(double[] p1, double[] p2) {
      double rank = MannWhitneyWilcoxonTest.CalculateRankSumForP1(p1, p2);
      return MannWhitneyWilcoxonTest.ApproximatePValue(rank, p1.Length, p2.Length, true);
    }

    /// <summary>
    /// Calculates whether a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test)
    /// would reject the hypothesis H0 that the given two populations stem from the same distribution.
    /// The alternative hypothesis would be that they come from different distributions.
    /// The test performs a ranking of the data and decides based on the ranking whehter to reject the
    /// hypothesis or not.
    /// 
    /// If there are less than 20 samples in each population this decision is based on a table lookup.
    /// If one array consists of more than 20 samples, it will approximate the distribution by a normal
    /// distribution. In the case of the table lookup, alpha is restricted to 0.01, 0.05, and 0.1.
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <param name="alpha">The significance level. If p1 and p2 are both smaller or equal than 20, the decision is based on three tables that represent significance at 0.01, 0.05, and 0.1.</param>
    /// <returns>True if H0 (p1 equals p2) can be rejected, False otherwise</returns>
    public static bool TwoTailedTest(double[] p1, double[] p2, double alpha) {
      return MannWhitneyWilcoxonTest.TwoTailedTest(p1, p2, alpha, 20);
    }

    /// <summary>
    /// Calculates whether a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test)
    /// would reject the hypothesis H0 that the given two populations stem from the same distribution.
    /// The alternative hypothesis would be that they come from different distributions.
    /// The test performs a ranking of the data and decides based on the ranking whehter to reject the
    /// hypothesis or not.
    /// 
    /// If there are less than <paramref name="approximationLevel"/> samples in each population this
    /// decision is based on a table lookup.
    /// If one array consists of more than <paramref name="approximationLevel"/> samples, it will
    /// approximate the distribution by a normal distribution. In the case of the table lookup,
    /// alpha is restricted to 0.01, 0.05, and 0.1.
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <param name="alpha">The significance level. If p1 and p2 are both smaller or equal than 20,
    /// the decision is based on three tables that represent significance at 0.01, 0.05, and 0.1.</param>
    /// <param name="approximationLevel">Defines at which sample size to use normal approximation,
    /// instead of the table lookup. If a higher value than 20 is specified, the value 20 will be
    /// used instead, if 0 is specified the result will be compuated always by approximation.</param>
    /// <returns>True if H0 (p1 equals p2) can be rejected, False otherwise</returns>
    public static bool TwoTailedTest(double[] p1, double[] p2, double alpha, int approximationLevel) {
      Array.Sort<double>(p1);
      Array.Sort<double>(p2);
      int n1 = p1.Length, n2 = p2.Length;

      double R1 = MannWhitneyWilcoxonTest.CalculateRankSumForP1(p1, p2);

      if (n1 <= Math.Min(approximationLevel, 20) && n2 <= Math.Min(approximationLevel, 20)) {
        return MannWhitneyWilcoxonTest.AbsolutePValue(R1, n1, n2, alpha, true);
      } else { // normal approximation
        return MannWhitneyWilcoxonTest.ApproximatePValue(R1, n1, n2, true) <= alpha;
      }
    }

    /// <summary>
    /// Calculates the p-value of a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test).
    /// The test performs a ranking of the data and returns the p-value indicating the level of significance
    /// at which the hypothesis H0 can be rejected.
    /// Caution: This method approximates the ranks in p1 and p2 with a normal distribution and should only be called
    /// when the sample size is at least 10 in both samples.
    /// 
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <returns>The p-value of the test</returns>
    public static double TwoTailedTest(int[] p1, int[] p2) {
      double rank = MannWhitneyWilcoxonTest.CalculateRankSumForP1(p1, p2);
      return MannWhitneyWilcoxonTest.ApproximatePValue(rank, p1.Length, p2.Length, true);
    }

    /// <summary>
    /// Calculates whether a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test)
    /// would reject the hypothesis H0 that the given two populations stem from the same distribution.
    /// The alternative hypothesis would be that they come from different distributions.
    /// The test performs a ranking of the data and decides based on the ranking whehter to reject the
    /// hypothesis or not.
    /// 
    /// If there are less than 20 samples in each population this decision is based on a table lookup.
    /// If one array consists of more than 20 samples, it will approximate the distribution by a normal
    /// distribution. In the case of the table lookup, alpha is restricted to 0.01, 0.05, and 0.1.
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <param name="alpha">The significance level. If p1 and p2 are both smaller or equal than 20, the decision is based on three tables that represent significance at 0.01, 0.05, and 0.1.</param>
    /// <returns>True if H0 (p1 equals p2) can be rejected, False otherwise</returns>
    public static bool TwoTailedTest(int[] p1, int[] p2, double alpha) {
      return MannWhitneyWilcoxonTest.TwoTailedTest(p1, p2, alpha, 20);
    }

    /// <summary>
    /// Calculates whether a 2-tailed Mann Whitney Wilcoxon U-test (also known as Mann Whitney rank sum test)
    /// would reject the hypothesis H0 that the given two populations stem from the same distribution.
    /// The alternative hypothesis would be that they come from different distributions.
    /// The test performs a ranking of the data and decides based on the ranking whehter to reject the
    /// hypothesis or not.
    /// 
    /// If there are less than <paramref name="approximationLevel"/> samples in each population this
    /// decision is based on a table lookup.
    /// If one array consists of more than <paramref name="approximationLevel"/> samples, it will
    /// approximate the distribution by a normal distribution. In the case of the table lookup,
    /// alpha is restricted to 0.01, 0.05, and 0.1.
    /// </summary>
    /// <param name="p1">Array with samples from population 1</param>
    /// <param name="p2">Array with samples from population 2</param>
    /// <param name="alpha">The significance level. If p1 and p2 are both smaller or equal than 20,
    /// the decision is based on three tables that represent significance at 0.01, 0.05, and 0.1.</param>
    /// <param name="approximationLevel">Defines at which sample size to use normal approximation,
    /// instead of the table lookup. If a higher value than 20 is specified, the value 20 will be
    /// used instead, if 0 is specified the result will be compuated always by approximation.</param>
    /// <returns>True if H0 (p1 equals p2) can be rejected, False otherwise</returns>
    public static bool TwoTailedTest(int[] p1, int[] p2, double alpha, int approximationLevel) {
      Array.Sort<int>(p1);
      Array.Sort<int>(p2);
      int n1 = p1.Length, n2 = p2.Length;
      
      double R1 = MannWhitneyWilcoxonTest.CalculateRankSumForP1(p1, p2);

      if (n1 <= Math.Min(approximationLevel, 20) && n2 <= Math.Min(approximationLevel, 20)) {
        return MannWhitneyWilcoxonTest.AbsolutePValue(R1, n1, n2, alpha, true);
      } else {
        return MannWhitneyWilcoxonTest.ApproximatePValue(R1, n1, n2, true) <= alpha;
      }
    }

    private static bool AbsolutePValue(double rank, int nRank, int nOther, double alpha, bool twoTailed) {
      double U1 = rank - (double)(nRank * (nRank + 1) / 2);
      double U2 = (double)(nRank * nOther) - U1;
      if (alpha <= 0.01) {
        return (Math.Min(U1, U2) <= table001[nRank - 1, nOther - 1]);
      } else if (alpha <= 0.05) {
        return (Math.Min(U1, U2) <= table005[nRank - 1, nOther - 1]);
      } else if (alpha <= 0.1) {
        return (Math.Min(U1, U2) <= table01[nRank - 1, nOther - 1]);
      } else throw new ArgumentException("ERROR in MannWhitneyWilcoxonTest: alpha must be <= 0.1");
    }

    private static double ApproximatePValue(double rank, int nRank, int nOther, bool twoTailed) {
      double U1 = rank - (double)(nRank * (nRank + 1) / 2);
      double U2 = (double)(nRank * nOther) - U1;

      double mu = nRank * nOther / 2;
      double sigma = Math.Sqrt(nRank * nOther * (nRank + nOther + 1) / 12);
      double z = (Math.Min(U1, U2) - mu) / sigma;
      return ProbabilityOfZValue(z);
    }

    // FIXME: when there's equality among some samples within a population the rank also needs to be averaged
    private static double CalculateRankSumForP1(int[] p1, int[] p2) {
      int rank = 0, p2Idx = 0;
      int n1 = p1.Length, n2 = p2.Length;
      double R1 = 0;

      for (int i = 0; i < n1; i++) {
        rank++;
        if (p1[i] < p2[p2Idx]) {
          R1 += rank;
        } if (p1[i] == p2[p2Idx]) {
          int startRank = rank;
          rank++;
          int commonRank = startRank + rank;
          int starti = i;
          while (i + 1 < n1 && Math.Abs(p1[i + 1] - p2[p2Idx]) < 1e-07) {
            i++;
            rank++;
            commonRank += rank;
          }
          while (p2Idx + 1 < n2 && Math.Abs(p1[i] - p2[p2Idx + 1]) < 1e-07) {
            p2Idx++;
            rank++;
            commonRank += rank;
          }
          p2Idx++;
          R1 += (i - starti + 1) * commonRank / (rank - startRank + 1);
        } else {
          p2Idx++;
          i--;
        }
        if (p2Idx == n2) {
          i++; rank++; // calculate the rest of the ranks for p1
          while (i < n1) {
            R1 += rank;
            rank++;
            i++;
          }
          break;
        }
      }
      return R1;
    }

    private static double CalculateRankSumForP2(int[] p1, int[] p2) {
      int n1 = p1.Length, n2 = p2.Length;
      double completeRankSum = (double)((n1 + n2 + 1) * (n1 + n2)) / 2.0;
      return completeRankSum - CalculateRankSumForP1(p1, p2);
    }

    // FIXME: when there's equality among some samples within a population the rank also needs to be averaged
    private static double CalculateRankSumForP1(double[] p1, double[] p2) {
      int rank = 0, n1 = p1.Length, n2 = p2.Length, p2Idx = 0;
      double R1 = 0;

      for (int i = 0; i < n1; i++) {
        rank++;
        if (Math.Abs(p1[i] - p2[p2Idx]) < 1e-07) {
          int startRank = rank;
          rank++;
          int commonRank = startRank + rank;
          int starti = i;
          while (i + 1 < n1 && Math.Abs(p1[i + 1] - p2[p2Idx]) < 1e-07) {
            i++;
            rank++;
            commonRank += rank;
          }
          while (p2Idx + 1 < n2 && Math.Abs(p1[i] - p2[p2Idx + 1]) < 1e-07) {
            p2Idx++;
            rank++;
            commonRank += rank;
          }
          p2Idx++;
          R1 += (i - starti + 1) * commonRank / (rank - startRank + 1);
        } else if (p1[i] < p2[p2Idx]) {
          R1 += rank;
        } else {
          p2Idx++;
          i--;
        }
        if (p2Idx == n2) {
          i++; rank++; // calculate the rest of the ranks for p1
          while (i < n1) {
            R1 += rank;
            rank++;
            i++;
          }
          break;
        }
      }
      return R1;
    }

    private static double CalculateRankSumForP2(double[] p1, double[] p2) {
      int n1 = p1.Length, n2 = p2.Length;
      double completeRankSum = (double)((n1 + n2 + 1) * (n1 + n2)) / 2.0;
      return completeRankSum - CalculateRankSumForP1(p1, p2);
    }

    // taken from: http://www.fourmilab.ch/rpkp/experiments/analysis/zCalc.html (in public domain)
    private static double ProbabilityOfZValue(double z) {
      double y, x, w, zMax = 6.0;
      if (Math.Abs(z) < 1e-07) {
        x = 0.0;
      } else {
        y = 0.5 * Math.Abs(z);
        if (y > (zMax * 0.5)) {
          x = 1.0;
        } else if (y < 1.0) {
          w = y * y;
          x = ((((((((0.000124818987 * w
                   - 0.001075204047) * w + 0.005198775019) * w
                   - 0.019198292004) * w + 0.059054035642) * w
                   - 0.151968751364) * w + 0.319152932694) * w
                   - 0.531923007300) * w + 0.797884560593) * y * 2.0;
        } else {
          y -= 2.0;
          x = (((((((((((((-0.000045255659 * y
                         + 0.000152529290) * y - 0.000019538132) * y
                         - 0.000676904986) * y + 0.001390604284) * y
                         - 0.000794620820) * y - 0.002034254874) * y
                         + 0.006549791214) * y - 0.010557625006) * y
                         + 0.011630447319) * y - 0.009279453341) * y
                         + 0.005353579108) * y - 0.002141268741) * y
                         + 0.000535310849) * y + 0.999936657524;
        }
      }
      return (z > 0.0) ? ((x + 1.0) * 0.5) : ((1.0 - x) * 0.5);
    }

    #region probability tables
    // table stems from
    // Roy C. Milton. An Extended Table of Critical Values for the Mann-Whitney (Wilcoxon) Two-Sample Statistic. Journal of the American Statistical Association, Vol. 59, No. 307 (Sep., 1964), pp. 925-934.
    private static int[,] table01 = new int[20, 20] {
      /*         1   2   3   4   5   6   7   8   9  10  11  12  13  14   15   16   17   18   19   20*/
      /*  1 */{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  -1,  -1,  -1,  -1,   0,   0},
      /*  2 */{ -1, -1, -1, -1,  0,  0,  0,  1,  1,  1,  1,  2,  2,  3,   3,   3,   3,   4,   4,   4},
      /*  3 */{ -1, -1,  0,  0,  1,  2,  2,  3,  4,  4,  5,  5,  6,  7,   7,   8,   9,   9,  10,  11},
      /*  4 */{ -1, -1,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11,  12,  14,  15,  16,  17,  18},
      /*  5 */{ -1,  0,  1,  2,  4,  5,  6,  8,  9, 11, 12, 13, 15, 16,  18,  19,  20,  22,  23,  25},
      /*  6 */{ -1,  0,  2,  3,  5,  7,  8, 10, 12, 14, 16, 17, 19, 21,  23,  25,  26,  28,  30,  32},
      /*  7 */{ -1,  0,  2,  4,  6,  8, 11, 13, 15, 17, 19, 21, 24, 26,  28,  30,  33,  35,  37,  39},
      /*  8 */{ -1,  1,  3,  5,  8, 10, 13, 15, 18, 20, 23, 26, 28, 31,  33,  36,  39,  41,  44,  47},
      /*  9 */{ -1,  1,  4,  6,  9, 12, 15, 18, 21, 24, 27, 30, 33, 36,  39,  42,  45,  48,  51,  54},
      /* 10 */{ -1,  1,  4,  7, 11, 14, 17, 20, 24, 27, 31, 34, 37, 41,  44,  48,  51,  55,  58,  62},
      /* 11 */{ -1,  1,  5,  8, 12, 16, 19, 23, 27, 31, 34, 38, 42, 46,  50,  54,  57,  61,  65,  69},
      /* 12 */{ -1,  2,  5,  9, 13, 17, 21, 26, 30, 34, 38, 42, 47, 51,  55,  60,  64,  68,  72,  77},
      /* 13 */{ -1,  2,  6, 10, 15, 19, 24, 28, 33, 37, 42, 47, 51, 56,  61,  65,  70,  75,  80,  84},
      /* 14 */{ -1,  3,  7, 11, 16, 21, 26, 31, 36, 41, 46, 51, 56, 61,  66,  71,  77,  82,  87,  92},
      /* 15 */{ -1,  3,  7, 12, 18, 23, 28, 33, 39, 44, 50, 55, 61, 66,  72,  77,  83,  88,  94, 100},
      /* 16 */{ -1,  3,  8, 14, 19, 25, 30, 36, 42, 48, 54, 60, 65, 71,  77,  83,  89,  95, 101, 107},
      /* 17 */{ -1,  3,  9, 15, 20, 26, 33, 39, 45, 51, 57, 64, 70, 77,  83,  89,  96, 102, 109, 115},
      /* 18 */{ -1,  4,  9, 16, 22, 28, 35, 41, 48, 55, 61, 68, 75, 82,  88,  95, 102, 109, 116, 123},
      /* 19 */{  0,  4, 10, 17, 23, 30, 37, 44, 51, 58, 65, 72, 80, 87,  94, 101, 109, 116, 123, 130},
      /* 20 */{  0,  4, 11, 18, 25, 32, 39, 47, 54, 62, 69, 77, 84, 92, 100, 107, 115, 123, 130, 138}};

    // table stems from http://math.usask.ca/~laverty/S245/Tables/wmw.pdf
    private static int[,] table005 = new int[20, 20] {
      /*         1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16   17   18   19   20*/
      /*  1 */{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  -1,  -1,  -1,  -1},
      /*  2 */{ -1, -1, -1, -1, -1, -1, -1,  0,  0,  0,  0,  1,  1,  1,  1,  1,   2,   2,   2,   2},
      /*  3 */{ -1, -1, -1, -1,  0,  1,  1,  2,  2,  3,  3,  4,  4,  5,  5,  6,   6,   7,   7,   8},
      /*  4 */{ -1, -1, -1,  0,  1,  2,  3,  4,  4,  5,  6,  7,  8,  9, 10, 11,  11,  12,  13,  13},
      /*  5 */{ -1, -1,  0,  1,  2,  3,  5,  6,  7,  8,  9, 11, 12, 13, 14, 15,  17,  18,  19,  20},
      /*  6 */{ -1, -1,  1,  2,  3,  5,  6,  8, 10, 11, 13, 14, 16, 17, 19, 21,  22,  24,  25,  27},
      /*  7 */{ -1, -1,  1,  3,  5,  6,  8, 10, 12, 14, 16, 18, 20, 22, 24, 26,  28,  30,  32,  34},
      /*  8 */{ -1,  0,  2,  4,  6,  8, 10, 13, 15, 17, 19, 22, 24, 26, 29, 31,  34,  36,  38,  41},
      /*  9 */{ -1,  0,  2,  4,  7, 10, 12, 15, 17, 21, 23, 26, 28, 31, 34, 37,  39,  42,  45,  48},
      /* 10 */{ -1,  0,  3,  5,  8, 11, 14, 17, 20, 23, 26, 29, 33, 36, 39, 42,  45,  48,  52,  55},
      /* 11 */{ -1,  0,  3,  6,  9, 13, 16, 19, 23, 26, 30, 33, 37, 40, 44, 47,  51,  55,  58,  62},
      /* 12 */{ -1,  1,  4,  7, 11, 14, 18, 22, 26, 29, 33, 37, 41, 45, 49, 53,  57,  61,  65,  69},
      /* 13 */{ -1,  1,  4,  8, 12, 16, 20, 24, 28, 33, 37, 41, 45, 50, 54, 59,  63,  67,  72,  76},
      /* 14 */{ -1,  1,  5,  9, 13, 17, 22, 26, 31, 36, 40, 45, 50, 55, 59, 64,  67,  74,  78,  83},
      /* 15 */{ -1,  1,  5, 10, 14, 19, 24, 29, 34, 39, 44, 49, 54, 59, 64, 70,  75,  80,  85,  90},
      /* 16 */{ -1,  1,  6, 11, 15, 21, 26, 31, 37, 42, 47, 53, 59, 64, 70, 75,  81,  86,  92,  98},
      /* 17 */{ -1,  2,  6, 11, 17, 22, 28, 34, 39, 45, 51, 57, 63, 67, 75, 81,  87,  93,  99, 105},
      /* 18 */{ -1,  2,  7, 12, 18, 24, 30, 36, 42, 48, 55, 61, 67, 74, 80, 86,  93,  99, 106, 112},
      /* 19 */{ -1,  2,  7, 13, 19, 25, 32, 38, 45, 52, 58, 65, 72, 78, 85, 92,  99, 106, 113, 119},
      /* 20 */{ -1,  2,  8, 14, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 98, 105, 112, 119, 127}};

    // table from: http://math.usask.ca/~laverty/S245/Tables/wmw.pdf
    private static int[,] table001 = new int[20, 20] {
      /*         1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19   20*/
      /*  1 */{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  -1},
      /*  2 */{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  0,   0},
      /*  3 */{ -1, -1, -1, -1, -1, -1, -1, -1,  0,  0,  0,  1,  1,  1,  2,  2,  2,  2,  3,   3},
      /*  4 */{ -1, -1, -1, -1, -1,  0,  0,  1,  1,  2,  2,  3,  3,  4,  5,  5,  6,  6,  7,   8},
      /*  5 */{ -1, -1, -1, -1,  0,  1,  1,  2,  3,  4,  5,  6,  7,  7,  8,  9, 10, 11, 12,  13},
      /*  6 */{ -1, -1, -1,  0,  1,  2,  3,  4,  5,  6,  7,  9, 10, 11, 12, 13, 15, 16, 17,  18},
      /*  7 */{ -1, -1, -1,  0,  1,  3,  4,  6,  7,  9, 10, 12, 13, 15, 16, 18, 19, 21, 22,  24},
      /*  8 */{ -1, -1, -1,  1,  2,  4,  6,  7,  9, 11, 13, 15, 17, 18, 20, 22, 24, 26, 28,  30},
      /*  9 */{ -1, -1,  0,  1,  3,  5,  7,  9, 11, 13, 16, 18, 20, 22, 24, 27, 29, 31, 33,  36},
      /* 10 */{ -1, -1,  0,  2,  4,  6,  9, 11, 13, 16, 18, 21, 24, 26, 29, 31, 34, 37, 39,  42},
      /* 11 */{ -1, -1,  0,  2,  5,  7, 10, 13, 16, 18, 21, 24, 27, 30, 33, 36, 39, 42, 45,  46},
      /* 12 */{ -1, -1,  1,  3,  6,  9, 12, 15, 18, 21, 24, 27, 31, 34, 37, 41, 44, 47, 51,  54},
      /* 13 */{ -1, -1,  1,  3,  7, 10, 13, 17, 20, 24, 27, 31, 34, 38, 42, 45, 49, 53, 56,  60},
      /* 14 */{ -1, -1,  1,  4,  7, 11, 15, 18, 22, 26, 30, 34, 38, 42, 46, 50, 54, 58, 63,  67},
      /* 15 */{ -1, -1,  2,  5,  8, 12, 16, 20, 24, 29, 33, 37, 42, 46, 51, 55, 60, 64, 69,  73},
      /* 16 */{ -1, -1,  2,  5,  9, 13, 18, 22, 27, 31, 36, 41, 45, 50, 55, 60, 65, 70, 74,  79},
      /* 17 */{ -1, -1,  2,  6, 10, 15, 19, 24, 29, 34, 39, 44, 49, 54, 60, 65, 70, 75, 81,  86},
      /* 18 */{ -1, -1,  2,  6, 11, 16, 21, 26, 31, 37, 42, 47, 53, 58, 64, 70, 75, 81, 87,  92},
      /* 19 */{ -1,  0,  3,  7, 12, 17, 22, 28, 33, 39, 45, 51, 56, 63, 69, 74, 81, 87, 93,  99},
      /* 20 */{ -1,  0,  3,  8, 13, 18, 24, 30, 36, 42, 46, 54, 60, 67, 73, 79, 86, 92, 99, 105}};
    #endregion

  }
}
