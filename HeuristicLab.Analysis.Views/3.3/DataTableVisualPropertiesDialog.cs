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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Analysis.Views {
  public partial class DataTableVisualPropertiesDialog : Form {
    protected bool SuppressEvents { get; set; }
    protected DataTable Content { get; private set; }
    private DataTableVisualProperties originalDataTableVPs;
    private Dictionary<string, DataRowVisualProperties> originalDataRowVPs;

    private HashSet<string> modifiedDisplayNames;
    public IEnumerable<string> RowsWithModifiedDisplayNames { get { return modifiedDisplayNames.AsEnumerable(); } }

    public DataTableVisualPropertiesDialog(DataTable dataTable) {
      InitializeComponent();
      #region Prepare controls
      upButton.Text = string.Empty;
      upButton.Image = VSImageLibrary.ArrowUp;
      downButton.Text = string.Empty;
      downButton.Image = VSImageLibrary.ArrowDown;
      seriesListView.Items.Clear();
      seriesListView.SmallImageList = new ImageList();
      seriesListView.SmallImageList.Images.Add(VSImageLibrary.Graph);
      #endregion

      Content = dataTable;
      originalDataTableVPs = (DataTableVisualProperties)Content.VisualProperties.Clone();
      originalDataRowVPs = new Dictionary<string, DataRowVisualProperties>();
      foreach (DataRow row in Content.Rows)
        originalDataRowVPs.Add(row.Name, (DataRowVisualProperties)row.VisualProperties.Clone());

      dataTableVisualPropertiesControl.Content = Content.VisualProperties;

      modifiedDisplayNames = new HashSet<string>();
      FillSeriesListView();
      RegisterContentEvents();
    }

    private void RegisterContentEvents() {
      foreach (DataRow row in Content.Rows) {
        row.VisualProperties.PropertyChanged += new PropertyChangedEventHandler(Row_VisualProperties_PropertyChanged);
      }
    }

    private void DeregisterContentEvents() {
      foreach (DataRow row in Content.Rows) {
        row.VisualProperties.PropertyChanged -= new PropertyChangedEventHandler(Row_VisualProperties_PropertyChanged);
      }
    }

    protected override void OnClosing(CancelEventArgs e) {
      DeregisterContentEvents();
      Application.DoEvents();
      base.OnClosing(e);
    }

    private void Row_VisualProperties_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      foreach (DataRow row in Content.Rows) {
        if (e.PropertyName == "DisplayName" && row.VisualProperties == sender) {
          modifiedDisplayNames.Add(row.Name);
          break;
        }
      }
    }

    private void seriesListView_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents) {
        if (seriesListView.SelectedItems.Count != 1) {
          dataRowVisualPropertiesControl.Content = null;
          upButton.Enabled = downButton.Enabled = false;
        } else {
          string rowName = seriesListView.SelectedItems[0].Text;
          dataRowVisualPropertiesControl.Content = Content.Rows[rowName].VisualProperties;
          upButton.Enabled = seriesListView.SelectedIndices[0] > 0;
          downButton.Enabled = seriesListView.SelectedIndices[0] < seriesListView.Items.Count - 1;
        }
      }
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = DialogResult.Cancel;
      foreach (DataRow row in Content.Rows) {
        row.VisualProperties = originalDataRowVPs[row.Name];
      }
      modifiedDisplayNames.Clear();
      Content.VisualProperties = originalDataTableVPs;
      Close();
    }

    private void upButton_Click(object sender, System.EventArgs e) {
      if (seriesListView.SelectedIndices.Count == 1 && seriesListView.SelectedIndices[0] > 0) {
        int index = seriesListView.SelectedIndices[0];
        SuppressEvents = true;
        try {
          seriesListView.BeginUpdate();
          ListViewItem selectedSeriesItem = seriesListView.Items[index];
          seriesListView.Items.RemoveAt(index);
          ListViewItem temp = seriesListView.Items[index - 1];
          seriesListView.Items.RemoveAt(index - 1);
          seriesListView.Items.Insert(index - 1, selectedSeriesItem);
          seriesListView.Items.Insert(index, temp);
          seriesListView.SelectedIndices.Clear();
          seriesListView.EndUpdate();
        } finally { SuppressEvents = false; }
        seriesListView.SelectedIndices.Add(index - 1);
        UpdateAllSeriesPositions();
      }
    }

    private void downButton_Click(object sender, System.EventArgs e) {
      if (seriesListView.SelectedIndices.Count == 1 && seriesListView.SelectedIndices[0] < seriesListView.Items.Count - 1) {
        int index = seriesListView.SelectedIndices[0];
        SuppressEvents = true;
        try {
          seriesListView.BeginUpdate();
          ListViewItem temp = seriesListView.Items[index + 1];
          seriesListView.Items.RemoveAt(index + 1);
          ListViewItem selectedSeriesItem = seriesListView.Items[index];
          seriesListView.Items.RemoveAt(index);
          seriesListView.Items.Insert(index, temp);
          seriesListView.Items.Insert(index + 1, selectedSeriesItem);
          seriesListView.SelectedIndices.Clear();
          seriesListView.EndUpdate();
        } finally { SuppressEvents = false; }
        seriesListView.SelectedIndices.Add(index + 1);
        UpdateAllSeriesPositions();
      }
    }

    #region Helpers
    private void FillSeriesListView() {
      seriesListView.SelectedIndices.Clear();
      foreach (DataRow row in Content.Rows) {
        seriesListView.Items.Add(new ListViewItem(row.Name, 0));
      }
      if (seriesListView.Items.Count > 0)
        seriesListView.SelectedIndices.Add(0);
    }

    private void UpdateAllSeriesPositions() {
      Dictionary<string, DataRow> rows = Content.Rows.ToDictionary(x => x.Name);
      Content.Rows.Clear();
      for (int i = 0; i < seriesListView.Items.Count; i++) {
        ListViewItem item = seriesListView.Items[i];
        Content.Rows.Add(rows[item.Text]);
      }
    }
    #endregion
  }
}
