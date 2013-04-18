#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.Data.Views {
  public partial class EnhancedStringConvertibleMatrixView : StringConvertibleMatrixView {
    private bool[] columnVisibility, rowVisibility;

    // sets the visibility of its columns for the next time it is updated
    public IEnumerable<bool> ColumnVisibility {
      get { return columnVisibility; }
      set { columnVisibility = value.ToArray(); }
    }
    // sets the visibility of its rows for the next time it is updated
    public IEnumerable<bool> RowVisibility {
      get { return rowVisibility; }
      set { rowVisibility = value.ToArray(); }
    }

    public double Maximum { get; set; }
    public double Minimum { get; set; }

    public new DoubleMatrix Content {
      get { return (DoubleMatrix)base.Content; }
      set { base.Content = value; }
    }

    public EnhancedStringConvertibleMatrixView() {
      InitializeComponent();
    }

    public void ResetVisibility() {
      columnVisibility = null;
      rowVisibility = null;
    }

    protected override void UpdateColumnHeaders() {
      base.UpdateColumnHeaders();
      if (columnVisibility != null && Content != null && columnVisibility.Count() == dataGridView.ColumnCount) {
        int i = 0;
        foreach (var visibility in columnVisibility) {
          dataGridView.Columns[i].Visible = visibility;
          i++;
        }
      }
    }
    protected override void UpdateRowHeaders() {
      if (Content == null) return;
      if (rowVisibility != null && rowVisibility.Count() != dataGridView.RowCount) return;

      for (int index = 0; index < dataGridView.RowCount; index++) {
        dataGridView.Rows[index].HeaderCell.Value = Content.RowNames.ElementAt(virtualRowIndices[index]);

        if (rowVisibility != null) {
          dataGridView.Rows[index].Visible = rowVisibility.ElementAt(virtualRowIndices[index]);
        } else {
          dataGridView.Rows[index].Visible = true;
        }
      }
    }

    protected virtual void ShowHideRows_Click(object sender, EventArgs e) {
      var dialog = new StringConvertibleMatrixRowVisibilityDialog(this.dataGridView.Rows.Cast<DataGridViewRow>());
      dialog.ShowDialog();
      RowVisibility = dialog.Visibility;
    }

    protected override void ShowHideColumns_Click(object sender, EventArgs e) {
      var dialog = new StringConvertibleMatrixColumnVisibilityDialog(this.dataGridView.Columns.Cast<DataGridViewColumn>());
      dialog.ShowDialog();
      ColumnVisibility = dialog.Visibility;
    }

    protected void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
      if (Content == null) return;
      if (e.RowIndex < 0) return;
      if (e.ColumnIndex < 0) return;
      if (e.State.HasFlag(DataGridViewElementStates.Selected)) return;
      if (!e.PaintParts.HasFlag(DataGridViewPaintParts.Background)) return;

      int rowIndex = virtualRowIndices[e.RowIndex];
      Color backColor = GetDataPointColor(Content[rowIndex, e.ColumnIndex], Minimum, Maximum);
      using (Brush backColorBrush = new SolidBrush(backColor)) {
        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
      }
      e.PaintContent(e.CellBounds);
      e.Handled = true;
    }

    protected virtual Color GetDataPointColor(double value, double min, double max) {
      if (double.IsNaN(value)) {
        return Color.DarkGray;
      }
      IList<Color> colors = ColorGradient.Colors;
      int index = (int)((colors.Count - 1) * (value - min) / (max - min));
      if (index >= colors.Count) index = colors.Count - 1;
      if (index < 0) index = 0;
      return colors[index];
    }
  }
}
