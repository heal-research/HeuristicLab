#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Collections {
  public struct IndexedItem<T> {
    private readonly int index;
    public int Index {
      get { return index; }
    }
    private readonly T value;
    public T Value {
      get { return value; }
    }

    public IndexedItem(int index, T value) {
      this.index = index;
      this.value = value;
    }

    public override string ToString() {
      return "[" + index + ": " + (value == null ? "null" : value.ToString()) + "]";
    }
  }
}
