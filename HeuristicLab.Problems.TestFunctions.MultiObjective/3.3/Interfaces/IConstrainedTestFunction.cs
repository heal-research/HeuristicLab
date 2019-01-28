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

using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("D79FD6F1-CB7D-4374-8F82-A6A98B35E95D")]
  /// <summary>
  /// An interface which represents an evaluation operator for multi objective test functions.
  /// </summary>
  public interface IConstrainedTestFunction : INamedItem {

    /// <summary>
    /// checks whether a given solution violates the contraints of this function  
    /// </summary>
    /// <param name="point"></param>
    /// <param name="objectives"></param>
    /// <returns>a double array that holds the distances that describe how much every contraint is violated (0 is not violated) </returns>
    double[] CheckConstraints(RealVector point, int objectives);
  }
}
