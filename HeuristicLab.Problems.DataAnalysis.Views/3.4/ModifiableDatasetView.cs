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

using System.Windows.Forms;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("ModifiableDataset View")]
  [Content(typeof(ModifiableDataset), true)]
  public partial class ModifiableDatasetView : StringConvertibleMatrixView {
    public new ModifiableDataset Content {
      get { return (ModifiableDataset)base.Content; }
      set {
        if (base.Content != value)
          base.Content = value;
      }
    }

    public ModifiableDatasetView() {
      InitializeComponent();
    }

    #region register/deregister content events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      dataGridView.CellContentDoubleClick += dataGridView_cellContentDoubleClicked;
      dataGridView.CellLeave += dataGridView_cellLeave;
    }

    protected override void DeregisterContentEvents() {
      dataGridView.CellContentDoubleClick -= dataGridView_cellContentDoubleClicked;
      dataGridView.CellLeave -= dataGridView_cellLeave;
      base.DeregisterContentEvents();
    }
    #endregion

    #region event handlers
    private void dataGridView_cellContentDoubleClicked(object sender, DataGridViewCellEventArgs args) {
      var currentCell = dataGridView[args.ColumnIndex, args.RowIndex];
      dataGridView.ReadOnly = false;
      currentCell.ReadOnly = false;
      dataGridView.CurrentCell = currentCell;
      dataGridView.BeginEdit(true);
    }

    private void dataGridView_cellLeave(object sender, DataGridViewCellEventArgs args) {
      var currentCell = dataGridView[args.ColumnIndex, args.RowIndex];
      dataGridView.ReadOnly = true;
      currentCell.ReadOnly = true;
      dataGridView.EndEdit();
    }
    #endregion
  }
}
