#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.OKB.Administration {
  [Item("Algorithm", "An OKB algorithm.")]
  public partial class Algorithm {
    protected Algorithm(Algorithm original, Cloner cloner)
      : base(original, cloner) {
      PlatformId = original.PlatformId;
      AlgorithmClassId = original.AlgorithmClassId;
      DataTypeName = original.DataTypeName;
      DataTypeTypeName = original.DataTypeTypeName;
    }
    public Algorithm() {
      Name = "New Algorithm";
      PlatformId = 1;
      AlgorithmClassId = 1;
      DataTypeName = "Unknown";
      DataTypeTypeName = "Unknown";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Algorithm(this, cloner);
    }
  }
}
