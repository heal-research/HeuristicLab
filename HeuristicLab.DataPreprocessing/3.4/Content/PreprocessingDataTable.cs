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

using System;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprossingDataTable", "A table of data values.")]
  public class PreprocessingDataTable : DataTable {

    public PreprocessingDataTable()
      : base() {
      SelectedRows = new NamedItemCollection<DataRow>();
    }
    public PreprocessingDataTable(string name)
      : base(name) {
      SelectedRows = new NamedItemCollection<DataRow>();
    }

    protected PreprocessingDataTable(PreprocessingDataTable original, Cloner cloner)
      : base(original, cloner) {
      this.selectedRows = cloner.Clone(original.selectedRows);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingDataTable(this, cloner);
    }

    private NamedItemCollection<DataRow> selectedRows;
    public NamedItemCollection<DataRow> SelectedRows {
      get { return selectedRows; }
      private set {
        if (selectedRows != null) throw new InvalidOperationException("Rows already set");
        selectedRows = value;
      }
    }

  }
}
