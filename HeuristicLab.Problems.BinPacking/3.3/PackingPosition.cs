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

using HEAL.Fossil;
using HeuristicLab.Core;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking {
  [StorableType("8ED65A4B-6F85-47EA-90C6-A4B47533E429")]
  // PackingPosition is immutable (and handled as value types concerning Equals and GetHashCode)
  public abstract class PackingPosition : Item, IPackingPosition {
    /// <summary>
    /// The number of the bin to which the current item is assigned is stored in this variable. Counting starts at 0 !!!
    /// </summary>    
    [Storable]
    private readonly int assignedBin;
    public int AssignedBin {
      get { return assignedBin; }
    }

    [Storable]
    private readonly bool rotated;
    /// <summary>
    /// Determines if the positioned item is to be rotate by 90 degrees or not. (TODO: which axis?)
    /// </summary>
    public bool Rotated {
      get { return rotated; }
    }

    [StorableConstructor]
    protected PackingPosition(StorableConstructorFlag _) : base(_) { }
    protected PackingPosition(PackingPosition original, Cloner cloner)
      : base(original, cloner) {
      assignedBin = original.AssignedBin;
      rotated = original.Rotated;
    }

    protected PackingPosition(int assignedBin, bool rotated) {
      this.assignedBin = assignedBin;
      this.rotated = rotated;
    }

    public override string ToString() {
      return string.Format("[{0}-{1}]", AssignedBin, Rotated);
    }

    public override bool Equals(object obj) {
      var other = obj as PackingPosition;
      if (other != null)
        return (other.AssignedBin == this.AssignedBin && other.Rotated == this.Rotated);
      else return false;
    }

    public override int GetHashCode() {
      return 31 * AssignedBin * (Rotated ? 3 : 7);
    }
  }
}
