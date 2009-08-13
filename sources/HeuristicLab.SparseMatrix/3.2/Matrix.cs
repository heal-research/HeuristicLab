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
using System.Collections.ObjectModel;
using HeuristicLab.Core;

namespace HeuristicLab.SparseMatrix {   
  public class Matrix : ItemBase {
    public Matrix() {
      this.rows = new List<MatrixRow>();
    }

    public Matrix(IEnumerable<MatrixRow> rows)
      : this() {
      this.rows.AddRange(rows);
    }

    private List<MatrixRow> rows;
    public ReadOnlyCollection<MatrixRow> GetRows() {
      return rows.AsReadOnly();
    }

    public void AddRow(MatrixRow row) {
      this.rows.Add(row);
    }

    public void RemoveRow(MatrixRow row) {
      this.rows.Remove(row);
    }
    
  }
}
