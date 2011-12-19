#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("DataTableHistory View")]
  [Content(typeof(DataTableHistory), true)]
  public partial class DataTableHistoryView : MovieView<DataTable>, IConfigureableView {
    public DataTableHistoryView() {
      InitializeComponent();
      itemsGroupBox.Text = "Data Table";
    }

    public void ShowConfiguration() {
      DataTable current = viewHost.Content as DataTable;
      if (current == null) return;
      using (DataTableVisualPropertiesDialog dialog = new DataTableVisualPropertiesDialog(current)) {
        if (dialog.ShowDialog() != DialogResult.OK) return;
        Dictionary<string, bool> changeDisplayName = new Dictionary<string, bool>();
        foreach (DataRow row in current.Rows) {
          var answer = MessageBox.Show("Change display name for series " + row.Name + " to " + row.VisualProperties.DisplayName + " for all frames?", "Confirm change", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
          if (answer == DialogResult.Cancel) return;
          changeDisplayName[row.Name] = (answer == DialogResult.Yes);
        }
        foreach (DataTable dt in Content) {
          if (current == dt) continue;
          dt.VisualProperties = (DataTableVisualProperties)current.VisualProperties.Clone();
          foreach (DataRow row in current.Rows) {
            if (!dt.Rows.ContainsKey(row.Name)) continue;
            string oldDisplayName = dt.Rows[row.Name].VisualProperties.DisplayName;
            var props = (DataRowVisualProperties)row.VisualProperties.Clone();
            if (!changeDisplayName[row.Name]) props.DisplayName = oldDisplayName;
            dt.Rows[row.Name].VisualProperties = props;
          }
        }
      }
    }

  }
}
