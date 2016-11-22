#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the Quadratic Assignment Problem (QAP).
  /// </summary>
  public class QAPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of facilities (and also the number of locations)
    /// </summary>
    public int Dimension { get; set; }
    /// <summary>
    /// An NxN Matrix with N = |Faciliies|
    /// </summary>
    public double[,] Distances { get; set; }
    /// <summary>
    /// An NxN Matrix with N = |Faciliies|
    /// </summary>
    public double[,] Weights { get; set; }

    /// <summary>
    /// Optional! An array of length N with N = |Facilities|
    /// </summary>
    public int[] BestKnownAssignment { get; set; }
    /// <summary>
    /// Optional! The quality value of the <see cref="BestKnownAssignment"/>
    /// </summary>
    public double? BestKnownQuality { get; set; }
  }
}
