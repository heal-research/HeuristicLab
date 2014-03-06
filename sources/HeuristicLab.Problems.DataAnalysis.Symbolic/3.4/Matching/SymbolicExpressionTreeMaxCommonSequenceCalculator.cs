using System;
using System.Collections.Generic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Matching {
  public class MaxCommonSequenceCalculator<T, U>
    where T : class
    where U : IEqualityComparer<T> {

    public U Comparer { get; set; }
    private int[,] matrix;
    private List<T> sequence;

    /// <summary>
    /// Calculate the maximal common subsequence between arrays a and b and return it
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public IEnumerable<T> Calculate(T[] a, T[] b) {
      int m = a.Length;
      int n = b.Length;

      if (m == 0 || n == 0) return null;
      sequence = new List<T>();
      matrix = new int[m + 1, n + 1];

      for (int i = 0; i <= m; ++i) {
        for (int j = 0; j <= n; ++j) {
          if (i == 0 || j == 0) {
            matrix[i, j] = 0;
          } else if (Comparer.Equals(a[i - 1], b[j - 1])) {
            matrix[i, j] = matrix[i - 1, j - 1] + 1;
          } else {
            matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
          }
        }
      }
      recon(a, b, n, m);
      return sequence;
    }

    private void recon(T[] a, T[] b, int i, int j) {
      while (true) {
        if (i == 0 || j == 0) return;
        if (Comparer.Equals(a[i - 1], b[j - 1])) {
          recon(a, b, i - 1, j - 1);
          sequence.Add(a[i - 1]);
        } else if (matrix[i - 1, j] > matrix[i, j - 1]) {
          i = i - 1;
          continue;
        } else {
          j = j - 1;
          continue;
        }
        break;
      }
    }
  }
}
