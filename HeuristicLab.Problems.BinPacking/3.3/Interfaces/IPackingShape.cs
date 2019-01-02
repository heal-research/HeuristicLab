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

using System;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Problems.BinPacking {
  [StorableType("612b4eb0-946e-4690-ba43-8dd0e5520e8b")]
  // a packing shape represents a physical object of correct dimension (2d or 3d) 
  // packing bins (containers) and packing items are both packing shapes
  public interface IPackingShape : IItem, IComparable {
    int Volume { get; }
    void ApplyHorizontalOrientation();
  }
}
