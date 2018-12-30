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
using HEAL.Fossil;

namespace HeuristicLab.Common {
  [StorableType("c4231b4c-94ac-4b4a-bd99-3deea73237de")]
  public class ReferenceEqualityComparer : IEqualityComparer<object> {
    bool IEqualityComparer<object>.Equals(object x, object y) {
      return object.ReferenceEquals(x, y);
    }

    int IEqualityComparer<object>.GetHashCode(object obj) {
      if (obj == null) return 0;
      return obj.GetHashCode();
    }
  }
}
