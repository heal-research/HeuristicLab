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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;

namespace HeuristicLab.Data {
  public interface IStringConvertibleMatrix : IContent {
    int Rows { get; set; }
    int Columns { get; set; }
    IEnumerable<string> ColumnNames { get; set; }
    IEnumerable<string> RowNames { get; set; }

    bool SortableView { get; set; }
    bool ReadOnly { get; }

    bool Validate(string value, out string errorMessage);
    string GetValue(int rowIndex, int columnIndex);
    bool SetValue(string value, int rowIndex, int columnIndex);

    event EventHandler ColumnsChanged;
    event EventHandler RowsChanged;
    event EventHandler ColumnNamesChanged;
    event EventHandler RowNamesChanged;
    event EventHandler SortableViewChanged;
    event EventHandler<EventArgs<int, int>> ItemChanged;
    event EventHandler Reset;

  }
}
