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

using System;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Mono {
  #region Mono Compatibility
  //work around mono's missing ObjectEqualityComparer
  [Serializable]
  public class ObjectEqualityComparer<T> : EqualityComparer<T> {
    public override int GetHashCode(T obj) {
      return EqualityComparer<T>.Default.GetHashCode(obj);
    }
    public override bool Equals(T a, T b) {
      return EqualityComparer<T>.Default.Equals(a, b);
    }
  }
  #endregion
}

