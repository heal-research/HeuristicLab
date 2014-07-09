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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator that relinks paths between traveling salesman solutions using a multiple guiding strategy.
  /// </summary>
  /// <remarks>
  /// The operator incrementally changes the initiating solution towards the guiding solution by correcting edges as needed. For each city it choses the best edge from all guiding solutions.
  /// </remarks>
  [Item("TSPMultipleGuidesPathRelinker", "An operator that relinks paths between traveling salesman solutions using a multiple guiding strategy. The operator incrementally changes the initiating solution towards the guiding solution by correcting edges as needed. For each city it choses the best edge from all guiding solutions.")]
  [StorableClass]
  public sealed class TSPMultipleGuidesPathRelinker : SingleObjectivePathRelinker {
    #region Parameter properties
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    #endregion

    #region Properties
    public DistanceMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private TSPMultipleGuidesPathRelinker(bool deserializing) : base(deserializing) { }
    private TSPMultipleGuidesPathRelinker(TSPMultipleGuidesPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public TSPMultipleGuidesPathRelinker()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPMultipleGuidesPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem[] guides, DistanceMatrix distances, PercentValue n) {
      if (!(initiator is Permutation) || guides.Any(x => !(x is Permutation)))
        throw new ArgumentException("Cannot relink path because some of the provided solutions have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      Permutation v1 = initiator.Clone() as Permutation;
      Permutation[] targets = new Permutation[guides.Length];
      Array.Copy(guides, targets, guides.Length);

      if (targets.Any(x => x.Length != v1.Length))
        throw new ArgumentException("At least one solution is of different length.");

      IList<Permutation> solutions = new List<Permutation>();
      for (int i = 0; i < v1.Length; i++) {
        int currCityIndex = i;
        int bestCityIndex = (i + 1) % v1.Length;
        double currDistance = distances[v1[currCityIndex], v1[bestCityIndex]];
        // check each guiding solution
        targets.ToList().ForEach(solution => {
          // locate current city
          var node = solution.Select((x, index) => new { Id = x, Index = index }).Single(x => x.Id == v1[currCityIndex]);
          int pred = solution[(node.Index - 1 + solution.Length) % solution.Length];
          int succ = solution[(node.Index + 1) % solution.Length];
          // get distances to neighbors
          var results = new[] { pred, succ }.Select(x => new { Id = x, Distance = distances[x, node.Id] });
          var bestCity = results.Where(x => x.Distance < currDistance).OrderBy(x => x.Distance).FirstOrDefault();
          if (bestCity != null) {
            bestCityIndex = v1.Select((x, index) => new { Id = x, Index = index }).Single(x => x.Id == bestCity.Id).Index;
            currDistance = bestCity.Distance;
          }
        });
        Invert(v1, currCityIndex + 1, bestCityIndex);
        solutions.Add(v1.Clone() as Permutation);
      }

      IList<IItem> selection = new List<IItem>();
      if (solutions.Count > 0) {
        int noSol = (int)(solutions.Count * n.Value);
        if (noSol <= 0) noSol++;
        double stepSize = (double)solutions.Count / (double)noSol;
        for (int i = 0; i < noSol; i++)
          selection.Add(solutions.ElementAt((int)((i + 1) * stepSize - stepSize * 0.5)));
      }

      return new ItemArray<IItem>(selection);
    }

    private static void Invert(Permutation sol, int i, int j) {
      if (i != j)
        for (int a = 0; a < Math.Abs(i - j) / 2; a++)
          if (sol[(i + a) % sol.Length] != sol[(j - a + sol.Length) % sol.Length]) {
            // XOR swap
            sol[(i + a) % sol.Length] ^= sol[(j - a + sol.Length) % sol.Length];
            sol[(j - a + sol.Length) % sol.Length] ^= sol[(i + a) % sol.Length];
            sol[(i + a) % sol.Length] ^= sol[(j - a + sol.Length) % sol.Length];
          }
    }

    protected override ItemArray<IItem> Relink(ItemArray<IItem> parents, PercentValue n) {
      if (parents.Length < 2)
        throw new ArgumentException("The number of parents is smaller than 2.");
      return Apply(parents[0], parents.Skip(1).ToArray(), DistanceMatrix, n);
    }
  }
}
