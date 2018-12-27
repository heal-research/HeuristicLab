#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TravelingSalesman {
  // BackwardsCompatibility3.3
  #region Backwards compatible code, remove with 3.4
  /// <summary>
  /// An operator for analyzing the diversity of solutions of Traveling Salesman Problems given in path representation.
  /// </summary>
  [Obsolete]
  [NonDiscoverableType]
  [Item("TSPPopulationDiversityAnalyzer", "An operator for analyzing the diversity of solutions of Traveling Salesman Problems given in path representation.")]
  [StorableClass]
  public sealed class TSPPopulationDiversityAnalyzer : PopulationDiversityAnalyzer<Permutation> {
    [StorableConstructor]
    private TSPPopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    private TSPPopulationDiversityAnalyzer(TSPPopulationDiversityAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public TSPPopulationDiversityAnalyzer() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPPopulationDiversityAnalyzer(this, cloner);
    }

    protected override double[,] CalculateSimilarities(Permutation[] solutions) {
      int count = solutions.Length;
      double[,] similarities = new double[count, count];
      int[][] edges = new int[count][];

      for (int i = 0; i < count; i++)
        edges[i] = CalculateEdgesVector(solutions[i]);

      for (int i = 0; i < count; i++) {
        similarities[i, i] = 1;
        for (int j = i + 1; j < count; j++) {
          similarities[i, j] = CalculateSimilarity(edges[i], edges[j]);
          similarities[j, i] = similarities[i, j];
        }
      }
      return similarities;
    }

    private int[] CalculateEdgesVector(Permutation permutation) {
      // transform path representation into adjacency representation
      int[] edgesVector = new int[permutation.Length];
      for (int i = 0; i < permutation.Length - 1; i++)
        edgesVector[permutation[i]] = permutation[i + 1];
      edgesVector[permutation[permutation.Length - 1]] = permutation[0];
      return edgesVector;
    }

    private double CalculateSimilarity(int[] edgesA, int[] edgesB) {
      // calculate relative number of identical edges
      int identicalEdges = 0;
      for (int i = 0; i < edgesA.Length; i++) {
        if ((edgesA[i] == edgesB[i]) || (edgesA[edgesB[i]] == i))
          identicalEdges++;
      }
      return ((double)identicalEdges) / edgesA.Length;
    }
  }
  #endregion
}
