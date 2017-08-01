﻿#region License Information
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.DataPreprocessing.Views {
  public partial class RenameColumnsDialog : Form {
    public IList<string> ColumnNames {
      get {
        var names = new List<string>(dataGridView.RowCount);
        for (int i = 0; i < dataGridView.RowCount; i++) {
          names.Insert(i, (string)dataGridView[0, i].Value);
        }
        return names.AsReadOnly();
      }
    }

    public RenameColumnsDialog() : this(Enumerable.Empty<string>()) { }
    public RenameColumnsDialog(IEnumerable<string> names) {
      InitializeComponent();
      foreach (var name in names) {
        dataGridView.Rows.Add(name);
      }
    }
  }
}
