#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.SparseMatrix {
  public class MatrixRow<KeyType, ValueType> {
    private Dictionary<KeyType, ValueType> values;
    public MatrixRow() {
      values = new Dictionary<KeyType, ValueType>();
    }

    public void Set(KeyType key, ValueType value) {
      values.Add(key, value);
    }

    public bool ContainsKey(KeyType key) {
      return values.ContainsKey(key);
    }

    public ValueType Get(KeyType key) {
      if (key == null || !values.ContainsKey(key)) return default(ValueType);
      return values[key];
    }

    public IEnumerable<KeyValuePair<KeyType, ValueType>> Values {
      get { return values; }
    }
  }
}