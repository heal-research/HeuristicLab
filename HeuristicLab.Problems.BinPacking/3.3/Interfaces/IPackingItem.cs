using HEAL.Attic;
#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.BinPacking {
  [StorableType("a0f46e8d-d18f-43c4-8b7d-66db0659b230")]
  // a packing item is one of the items that needs to be located in one of the packing bins (containers)
  public interface IPackingItem : IPackingShape {
    double Weight { get; set; }
    int Material { get; set; }
    /// <summary>
    /// Returns true if the "other" item can be stacked on this item.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool SupportsStacking(IPackingItem other);
  }
}
