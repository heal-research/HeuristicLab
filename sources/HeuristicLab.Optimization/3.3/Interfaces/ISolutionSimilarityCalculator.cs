#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// An interface which represents an operator for similarity calculation.
  /// </summary>
  public interface ISolutionSimilarityCalculator : IItem, IEqualityComparer<IScope> {
    string SolutionVariableName { get; set; }
    string QualityVariableName { get; set; }

    /// <summary>
    /// Calculates the similarity of two solutions.
    /// </summary>
    /// <param name="leftSolution">The first scope that contains a solution.</param>
    /// <param name="rightSolution">The second scope that contains a solution.</param>
    /// <returns>A double between zero and one. Zero means the two solutions differ in every aspect, one indicates that the two solutions are the same.</returns>
    double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution);

    /// <summary>
    /// Calculates the similarity of solutions in one scope.
    /// </summary>
    /// <param name="solutionCrowd">The scope that contains solutions.</param>
    /// <returns>A similarity matrix. Zero means the two solutions differ in every aspect, one indicates that the two solutions are the same.</returns>
    double[][] CalculateSolutionCrowdSimilarity(IScope solutionCrowd);

    /// <summary>
    /// Calculates the similarity of solutions in two scopes.
    /// </summary>
    /// <param name="leftSolutionCrowd">The first scope that contains solutions.</param>
    /// <param name="rightSolutionCrowd">The second scope that contains solutions.</param>
    /// <returns>A similarity matrix. Zero means the two solutions differ in every aspect, one indicates that the two solutions are the same.</returns>
    double[][] CalculateSolutionCrowdSimilarity(IScope leftSolutionCrowd, IScope rightSolutionCrowd);
  }
}
