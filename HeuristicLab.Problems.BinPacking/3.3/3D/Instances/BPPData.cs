#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.BinPacking3D.Instances {

  /// <summary>
  /// Represents an instance which contains bin packing problem data.
  /// </summary>
  public class BPPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of items.
    /// </summary>
    public int NumItems { get { return Items == null ? 0 : Items.Length; } }

    /// <summary>
    /// Assigned packing shape
    /// </summary>
    public PackingShape BinShape { get; set; }

    /// <summary>
    /// Array with assigned packing items
    /// </summary>
    public PackingItem[] Items { get; set; }
    
    /// <summary>
    /// Optional! The quality of the best-known solution.
    /// </summary>
    public double? BestKnownQuality { get; set; }
  }
}
