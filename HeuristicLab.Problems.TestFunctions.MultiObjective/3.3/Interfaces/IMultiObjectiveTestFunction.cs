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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  /// <summary>
  /// An interface which represents an multi objective test functions.
  /// </summary>
  public interface IMultiObjectiveTestFunction : INamedItem {
    bool[] Maximization(int objectives);
    double[,] Bounds(int objectives);

    IEnumerable<double[]> OptimalParetoFront(int objectives);
    double OptimalHypervolume(int objectives);
    double[] ReferencePoint(int objectives);

    int MinimumSolutionLength { get; }
    int MaximumSolutionLength { get; }
    int MinimumObjectives { get; }
    int MaximumObjectives { get; }

    double[] Evaluate(RealVector point, int objectives);
  }
}
