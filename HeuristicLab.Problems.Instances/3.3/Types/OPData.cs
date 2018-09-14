#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Problems.Instances.Types {
  /// <summary>
  /// Describes instances of the Orienteering Problem (OP).
  /// </summary>
  public class OPData : TSPData {
    /// <summary>
    /// The maximum distance constraint for a Orienteering solution.
    /// </summary>
    public double MaximumDistance { get; set; }
    /// <summary>
    /// The scores of the points.
    /// </summary>
    public double[] Scores { get; set; }
    /// <summary>
    /// The penalty for each visited vertex.
    /// </summary>
    public double PointVisitingCosts { get; set; }
    /// <summary>
    /// Index of the starting point
    /// </summary>
    public int StartingPoint { get; set; }
    /// <summary>
    /// Index of the ending point.
    /// </summary>
    public int TerminalPoint { get; set; }
  }
}