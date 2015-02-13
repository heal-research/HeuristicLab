#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  public class LinkageTree {
    private readonly int[][][] occurances;
    private readonly List<int>[] clusters;
    private List<int> clusterOrdering;
    private readonly int length;
    private readonly IRandom rand;
    private bool rebuildRequired = false;

    public LinkageTree(int length, IRandom rand) {
      this.length = length;
      this.rand = rand;
      occurances = new int[length][][];

      // Create a lower triangular matrix without the diagonal
      for (int i = 1; i < length; i++) {
        occurances[i] = new int[i][];
        for (int j = 0; j < i; j++) {
          occurances[i][j] = new int[4];
        }
      }
      clusters = new List<int>[2 * length - 1];
      for (int i = 0; i < clusters.Length; i++) {
        clusters[i] = new List<int>();
      }
      clusterOrdering = new List<int>();

      // first "length" clusters just contain a single gene
      for (int i = 0; i < length; i++) {
        clusters[i].Add(i);
      }
    }

    public void Add(BinaryVector solution) {
      if (solution.Length != length) throw new ArgumentException("The individual has not the correct length.");
      for (int i = 1; i < solution.Length; i++) {
        for (int j = 0; j < i; j++) {
          // Updates the entry of the 4 long array based on the two bits

          var pattern = (Convert.ToByte(solution[j]) << 1) + Convert.ToByte(solution[i]);
          occurances[i][j][pattern]++;
        }
      }
      rebuildRequired = true;
    }

    // While "total" always has an integer value, it is a double to reduce
    // how often type casts are needed to prevent integer divison
    // In the GECCO paper, calculates Equation 2
    private static double NegativeEntropy(int[] counts, double total) {
      double sum = 0;
      for (int i = 0; i < counts.Length; i++) {
        if (counts[i] != 0) {
          sum += ((counts[i] / total) * Math.Log(counts[i] / total));
        }
      }
      return sum;
    }

    // Uses the frequency table to calcuate the entropy distance between two indices.
    // In the GECCO paper, calculates Equation 1
    private int[] bits = new int[4];
    private double EntropyDistance(int i, int j) {
      // This ensures you are using the lower triangular part of "occurances"
      if (i < j) {
        int temp = i;
        i = j;
        j = temp;
      }
      var entry = occurances[i][j];
      // extracts the occurrences of the individual bits
      bits[0] = entry[0] + entry[2];  // i zero
      bits[1] = entry[1] + entry[3];  // i one
      bits[2] = entry[0] + entry[1];  // j zero
      bits[3] = entry[2] + entry[3];  // j one
      double total = bits[0] + bits[1];
      // entropy of the two bits on their own
      double separate = NegativeEntropy(bits, total);
      // entropy of the two bits as a single unit
      double together = NegativeEntropy(entry, total);
      // If together there is 0 entropy, the distance is zero
      if (together.IsAlmost(0)) {
        return 0.0;
      }
      return 2 - (separate / together);
    }



    // Performs O(N^2) clustering based on the method described in:
    // "Optimal implementations of UPGMA and other common clustering algorithms"
    // by I. Gronau and S. Moran
    // In the GECCO paper, Figure 2 is a simplified version of this algorithm.
    private double[][] distances;
    private void Rebuild() {
      if (distances == null) {
        distances = new double[clusters.Length * 2 - 1][];
        for (int i = 0; i < distances.Length; i++)
          distances[i] = new double[clusters.Length * 2 - 1];
      }


      // Keep track of which clusters have not been merged
      var topLevel = new List<int>(length);
      for (int i = 0; i < length; i++)
        topLevel.Add(i);

      bool[] useful = new bool[clusters.Length];
      for (int i = 0; i < useful.Length; i++)
        useful[i] = true;

      // Store the distances between all clusters
      for (int i = 1; i < length; i++) {
        for (int j = 0; j < i; j++) {
          distances[i][j] = EntropyDistance(clusters[i][0], clusters[j][0]);
          // make it symmetric
          distances[j][i] = distances[i][j];
        }
      }
      // Each iteration we add some amount to the path, and remove the last
      // two elements.  This keeps track of how much of usable is in the path.
      int end_of_path = 0;

      // build all clusters of size greater than 1
      for (int index = length; index < clusters.Length; index++) {
        // Shuffle everything not yet in the path
        topLevel.ShuffleInPlace(rand, end_of_path, topLevel.Count - 1);

        // if nothing in the path, just add a random usable node
        if (end_of_path == 0) {
          end_of_path = 1;
        }
        while (end_of_path < topLevel.Count) {
          // last node in the path
          int final = topLevel[end_of_path - 1];

          // best_index stores the location of the best thing in the top level
          int best_index = end_of_path;
          double min_dist = distances[final][topLevel[best_index]];
          // check all options which might be closer to "final" than "topLevel[best_index]"
          for (int option = end_of_path + 1; option < topLevel.Count; option++) {
            if (distances[final][topLevel[option]] < min_dist) {
              min_dist = distances[final][topLevel[option]];
              best_index = option;
            }
          }
          // If the current last two in the path are minimally distant
          if (end_of_path > 1 && min_dist >= distances[final][topLevel[end_of_path - 2]]) {
            break;
          }

          // move the best to the end of the path
          topLevel.Swap(end_of_path, best_index);
          end_of_path++;
        }
        // Last two elements in the path are the clusters to join
        int first = topLevel[end_of_path - 2];
        int second = topLevel[end_of_path - 1];

        // Only keep a cluster if the distance between the joining clusters is > zero
        bool keep = !distances[first][second].IsAlmost(0.0);
        useful[first] = keep;
        useful[second] = keep;

        // create the new cluster
        clusters[index] = clusters[first].Concat(clusters[second]).ToList();
        // Calculate distances from all clusters to the newly created cluster
        int i = 0;
        int end = topLevel.Count - 1;
        while (i <= end) {
          int x = topLevel[i];
          // Moves 'first' and 'second' to after "end" in topLevel
          if (x == first || x == second) {
            topLevel.Swap(i, end);
            end--;
            continue;
          }
          // Use the previous distances to calculate the joined distance
          double first_distance = distances[first][x];
          first_distance *= clusters[first].Count;
          double second_distance = distances[second][x];
          second_distance *= clusters[second].Count;
          distances[x][index] = ((first_distance + second_distance)
              / (clusters[first].Count + clusters[second].Count));
          // make it symmetric
          distances[index][x] = distances[x][index];
          i++;
        }

        // Remove first and second from the path
        end_of_path -= 2;
        topLevel.RemoveAt(topLevel.Count - 1);
        topLevel[topLevel.Count - 1] = index;
      }
      // Extract the useful clusters
      clusterOrdering.Clear();
      // Add all useful clusters. The last one is never useful.
      for (int i = 0; i < useful.Length - 1; i++) {
        if (useful[i]) clusterOrdering.Add(i);
      }

      // Shuffle before sort to ensure ties are broken randomly
      clusterOrdering.ShuffleInPlace(rand);
      clusterOrdering = clusterOrdering.OrderBy(i => clusters[i].Count).ToList();
    }

    public IEnumerable<List<int>> Clusters {
      get {
        // Just in time rebuilding
        if (rebuildRequired) Rebuild();
        foreach (var index in clusterOrdering) {
          // Send out the clusters in the desired order
          yield return clusters[index];
        }
      }
    }
  }
}