#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("CombinedFilter View")]
  [Content(typeof(CombinedFilter), true)]
  public partial class CombinedFilterView : AsynchronousContentView {
    public new CombinedFilter Content {
      get { return (CombinedFilter)base.Content; }
      set { base.Content = value; }
    }

    public CombinedFilterView() {
      InitializeComponent();
    }

    protected override void OnInitialized(System.EventArgs e) {
      base.OnInitialized(e);
      filtersComboBox.DataSource = QueryClient.Instance.Filters.ToList();
      filtersComboBox.DisplayMember = "Label";
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      RemoveFilters();
      label.Text = string.Empty;
      if (Content != null) {
        string nl = Environment.NewLine;
        label.Text = Content.Operation == BooleanOperation.And ? "A" + nl + "N" + nl + "D" : "O" + nl + "R";

        foreach (Filter filter in Content.Filters)
          AddFilter(filter);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addButton.Enabled = Content != null && !ReadOnly && filtersComboBox.SelectedIndex != -1;
      filtersComboBox.Enabled = Content != null && !ReadOnly && filtersComboBox.Items.Count > 0;
      tableLayoutPanel.Enabled = Content != null && !ReadOnly;
    }

    private void AddFilter(Filter filter) {
      Content.Filters.Add(filter);
      int rowIndex = Content.Filters.Count;

      tableLayoutPanel.SuspendLayout();
      tableLayoutPanel.RowCount++;
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

      Button removeButton = new Button();
      removeButton.Size = new System.Drawing.Size(24, 24);
      removeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      removeButton.Tag = filter;
      removeButton.Click += new System.EventHandler(removeButton_Click);
      tableLayoutPanel.Controls.Add(removeButton, 0, rowIndex);

      ContentView filterView = (ContentView)MainFormManager.CreateDefaultView(filter.GetType());
      filterView.Content = filter;
      filterView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tableLayoutPanel.Controls.Add(filterView, 1, rowIndex);

      tableLayoutPanel.ResumeLayout();
    }
    private void RemoveFilter(Filter filter) {
      int rowIndex = Content.Filters.IndexOf(filter) + 1;
      Content.Filters.Remove(filter);

      tableLayoutPanel.SuspendLayout();
      Control removeButton = tableLayoutPanel.GetControlFromPosition(0, rowIndex);
      Control filterView = tableLayoutPanel.GetControlFromPosition(1, rowIndex);
      tableLayoutPanel.Controls.Remove(removeButton);
      removeButton.Dispose();
      tableLayoutPanel.Controls.Remove(filterView);
      filterView.Dispose();

      for (int i = rowIndex + 1; i < tableLayoutPanel.RowCount; i++) {
        tableLayoutPanel.SetRow(tableLayoutPanel.GetControlFromPosition(0, i), i - 1);
        tableLayoutPanel.SetRow(tableLayoutPanel.GetControlFromPosition(1, i), i - 1);
      }

      tableLayoutPanel.RowCount--;
      tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
      tableLayoutPanel.ResumeLayout();
    }
    private void RemoveFilters() {
      tableLayoutPanel.SuspendLayout();
      for (int i = tableLayoutPanel.RowCount - 1; i > 0; i--) {
        Control removeButton = tableLayoutPanel.GetControlFromPosition(0, i);
        Control filterView = tableLayoutPanel.GetControlFromPosition(1, i);
        tableLayoutPanel.Controls.Remove(removeButton);
        removeButton.Dispose();
        tableLayoutPanel.Controls.Remove(filterView);
        filterView.Dispose();
        tableLayoutPanel.RowCount--;
        tableLayoutPanel.RowStyles.RemoveAt(i);
      }
      tableLayoutPanel.ResumeLayout();
    }

    private void addButton_Click(object sender, System.EventArgs e) {
      Filter filter = filtersComboBox.SelectedItem as Filter;
      if (filter != null) AddFilter(filter.Clone());
    }
    private void removeButton_Click(object sender, System.EventArgs e) {
      Filter filter = ((Button)sender).Tag as Filter;
      if (filter != null) RemoveFilter(filter);
    }
  }
}
