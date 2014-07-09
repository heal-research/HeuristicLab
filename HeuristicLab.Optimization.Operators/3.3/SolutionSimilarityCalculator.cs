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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// A base class for items that perform similarity calculation between two solutions.
  /// </summary>
  [Item("SimilarityCalculator", "A base class for items that perform similarity calculation between two solutions.")]
  [StorableClass]
  public abstract class SolutionSimilarityCalculator : Item, ISolutionSimilarityCalculator {
    [StorableConstructor]
    protected SolutionSimilarityCalculator(bool deserializing) : base(deserializing) { }
    protected SolutionSimilarityCalculator(SolutionSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    protected SolutionSimilarityCalculator() : base() { }

    public double[][] CalculateSolutionCrowdSimilarity(IScope leftSolutionCrowd, IScope rightSolutionCrowd) {
      if (leftSolutionCrowd == null || rightSolutionCrowd == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided crowds or both are null.");

      var leftIndividuals = leftSolutionCrowd.SubScopes;
      var rightIndividuals = rightSolutionCrowd.SubScopes;

      if (!leftIndividuals.Any() || !rightIndividuals.Any())
        throw new ArgumentException("Cannot calculate similarity because one of the provided crowds or both are empty.");

      var similarityMatrix = new double[leftIndividuals.Count][];
      for (int i = 0; i < leftIndividuals.Count; i++) {
        similarityMatrix[i] = new double[rightIndividuals.Count];
        for (int j = 0; j < rightIndividuals.Count; j++) {
          similarityMatrix[i][j] = CalculateSolutionSimilarity(leftIndividuals[i], rightIndividuals[j]);
        }
      }

      return similarityMatrix;
    }

    public double[][] CalculateSolutionCrowdSimilarity(IScope solutionCrowd) {
      if (solutionCrowd == null)
        throw new ArgumentException("Cannot calculate similarity because the provided crowd is null.");

      var individuals = solutionCrowd.SubScopes;

      if (!individuals.Any())
        throw new ArgumentException("Cannot calculate similarity because the provided crowd is empty.");

      var similarityMatrix = new double[individuals.Count][];
      for (int i = 0; i < individuals.Count; i++) similarityMatrix[i] = new double[individuals.Count];

      for (int i = 0; i < individuals.Count; i++) {
        for (int j = i; j < individuals.Count; j++) {
          similarityMatrix[i][j] = similarityMatrix[j][i] = CalculateSolutionSimilarity(individuals[i], individuals[j]);
        }
      }

      return similarityMatrix;
    }

    public abstract double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution);
    public abstract bool Equals(IScope x, IScope y);
    public abstract int GetHashCode(IScope obj);
  }
}
