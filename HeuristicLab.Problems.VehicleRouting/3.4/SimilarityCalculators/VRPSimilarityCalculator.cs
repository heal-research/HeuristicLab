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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which performs similarity calculation between two VRP solutions.
  /// </summary>
  /// <remarks>
  /// The operator calculates the similarity based on the number of edges the two solutions have in common.
  /// </remarks>
  [Item("VRPSimilarityCalculator", "An operator which performs similarity calculation between two VRP solutions.")]
  [StorableClass]
  public sealed class VRPSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    #region Properties
    [Storable]
    public IVRPProblemInstance ProblemInstance { get; set; }
    #endregion

    private VRPSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private VRPSimilarityCalculator(VRPSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      this.ProblemInstance = cloner.Clone(original.ProblemInstance);
    }
    public VRPSimilarityCalculator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(PotvinEncoding left, PotvinEncoding right) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided solutions or both are null.");
      if (left == right) return 1.0;

      // extract edges from first solution
      var edges1 = new List<Tuple<int, int>>();
      foreach (Tour tour in left.Tours) {
        edges1.Add(new Tuple<int, int>(0, tour.Stops[0]));
        for (int i = 0; i < tour.Stops.Count - 1; i++)
          edges1.Add(new Tuple<int, int>(tour.Stops[i], tour.Stops[i + 1]));
        edges1.Add(new Tuple<int, int>(tour.Stops[tour.Stops.Count - 1], 0));
      }

      // extract edges from second solution
      var edges2 = new List<Tuple<int, int>>();
      foreach (Tour tour in right.Tours) {
        edges2.Add(new Tuple<int, int>(0, tour.Stops[0]));
        for (int i = 0; i < tour.Stops.Count - 1; i++)
          edges2.Add(new Tuple<int, int>(tour.Stops[i], tour.Stops[i + 1]));
        edges2.Add(new Tuple<int, int>(tour.Stops[tour.Stops.Count - 1], 0));
      }

      if (edges1.Count + edges2.Count == 0)
        throw new ArgumentException("Cannot calculate diversity because no tours exist.");

      int identicalEdges = 0;
      foreach (var edge in edges1) {
        if (edges2.Any(x => x.Equals(edge)))
          identicalEdges++;
      }

      return identicalEdges * 2.0 / (edges1.Count + edges2.Count);
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var sol1 = leftSolution.Variables[SolutionVariableName].Value as IVRPEncoding;
      var sol2 = rightSolution.Variables[SolutionVariableName].Value as IVRPEncoding;

      var potvinSol1 = sol1 is PotvinEncoding ? sol1 as PotvinEncoding : PotvinEncoding.ConvertFrom(sol1, ProblemInstance);
      var potvinSol2 = sol2 is PotvinEncoding ? sol2 as PotvinEncoding : PotvinEncoding.ConvertFrom(sol2, ProblemInstance);

      return CalculateSimilarity(potvinSol1, potvinSol2);
    }
  }
}
