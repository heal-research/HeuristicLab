#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public class TypeEqualityComparer<T> : IEqualityComparer<T> {

    bool IEqualityComparer<T>.Equals(T x, T y) {
      return x.GetType().Equals(y.GetType());
    }

    int IEqualityComparer<T>.GetHashCode(T obj) {
      if (obj == null) return 0;
      return obj.GetType().GetHashCode();
    }
  }
}
