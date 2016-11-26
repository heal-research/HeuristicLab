using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;

namespace HeuristicLab.Algorithms.MemPR.Util {
  /// <summary>
  /// Implements the Ckmeans.1d.dp method. It is described in the paper:
  /// Haizhou Wang and Mingzhou Song. 2011.
  /// Ckmeans.1d.dp: Optimal k-means Clustering in One Dimension by Dynamic Programming
  /// The R Journal Vol. 3/2, pp. 29-33.
  /// available at https://journal.r-project.org/archive/2011-2/RJournal_2011-2_Wang+Song.pdf
  /// </summary>
  public class CkMeans1D {
    /// <summary>
    /// Clusters the 1-dimensional data given in <paramref name="estimations"/>.
    /// </summary>
    /// <param name="estimations">The 1-dimensional data that should be clustered.</param>
    /// <param name="k">The maximum number of clusters.</param>
    /// <param name="clusterValues">A vector of the same length as estimations that assigns to each point a cluster id.</param>
    /// <returns>A sorted list of cluster centroids and corresponding cluster ids.</returns>
    public static SortedList<double, int> Cluster(double[] estimations, int k, out int[] clusterValues) {
      int nPoints = estimations.Length;
      var distinct = estimations.Distinct().OrderBy(x => x).ToArray();
      var max = distinct.Max();
      if (distinct.Length <= k) {
        var dict = distinct.Select((v, i) => new { Index = i, Value = v }).ToDictionary(x => x.Value, y => y.Index);
        for (int i = distinct.Length; i < k; i++)
          dict.Add(max + i - distinct.Length + 1, i);

        clusterValues = new int[nPoints];
        for (int i = 0; i < nPoints; i++)
          if (!dict.ContainsKey(estimations[i])) clusterValues[i] = 0;
          else clusterValues[i] = dict[estimations[i]];

        return new SortedList<double, int>(dict);
      }

      var n = distinct.Length;
      var D = new double[n, k];
      var B = new int[n, k];

      for (int m = 0; m < k; m++) {
        for (int j = m; j <= n - k + m; j++) {
          if (m == 0)
            D[j, m] = SumOfSquaredDistances(distinct, 0, j + 1);
          else {
            var minD = double.MaxValue;
            var minI = 0;
            for (int i = 1; i <= j; i++) {
              var d = D[i - 1, m - 1] + SumOfSquaredDistances(distinct, i, j + 1);
              if (d < minD) {
                minD = d;
                minI = i;
              }
            }
            D[j, m] = minD;
            B[j, m] = minI;
          }
        }
      }

      var centers = new SortedList<double, int>();
      var upper = B[n - 1, k - 1];
      var c = Mean(distinct, upper, n);
      centers.Add(c, k - 1);
      for (int i = k - 2; i >= 0; i--) {
        var lower = B[upper - 1, i];
        var c2 = Mean(distinct, lower, upper);
        centers.Add(c2, i);
        upper = lower;
      }

      clusterValues = new int[nPoints];
      for (int i = 0; i < estimations.Length; i++) {
        clusterValues[i] = centers.MinItems(x => Math.Abs(estimations[i] - x.Key)).First().Value;
      }

      return centers;
    }

    private static double SumOfSquaredDistances(double[] x, int start, int end) {
      if (start == end) throw new InvalidOperationException();
      if (start + 1 == end) return 0.0;
      double mean = 0.0;
      for (int i = start; i < end; i++) {
        mean += x[i];
      }
      mean /= (end - start);
      var sum = 0.0;
      for (int i = start; i < end; i++) {
        sum += (x[i] - mean) * (x[i] - mean);
      }
      return sum;
    }

    private static double Mean(double[] x, int start, int end) {
      if (start == end) throw new InvalidOperationException();
      double mean = 0.0;
      for (int i = start; i < end; i++) {
        mean += x[i];
      }
      mean /= (end - start);
      return mean;
    }
  }
}
