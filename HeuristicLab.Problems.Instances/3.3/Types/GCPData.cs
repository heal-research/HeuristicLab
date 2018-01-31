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

namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the graph coloring problem (GCP).
  /// </summary>
  public class GCPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of nodes in the graph
    /// </summary>
    public int Nodes { get; set; }
    /// <summary>
    /// An Nx2 adjacency list with N = Nodes
    /// </summary>
    public int[,] Adjacencies { get; set; }

    /// <summary>
    /// Optional! An array of length N with N = |Nodes|
    /// </summary>
    public int[] BestKnownColoring { get; set; }
    /// <summary>
    /// Optional! The least amount of colors that would not result in conflicts.
    /// The amount of colors in <see cref="BestKnownColoring"/> if it is given as well.
    /// </summary>
    public int? BestKnownColors { get; set; }
  }
}
