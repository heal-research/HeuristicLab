#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.LinearAssignment.Views {
  [View("LAPAssignmentView")]
  [Content(typeof(LAPAssignment), IsDefaultView = true)]
  public partial class LAPAssignmentView : ItemView {
    private ViewHost assignmentViewHost;

    public new LAPAssignment Content {
      get { return (LAPAssignment)base.Content; }
      set { base.Content = value; }
    }

    public LAPAssignmentView() {
      InitializeComponent();
      assignmentViewHost = new ViewHost();
      assignmentViewHost.Dock = DockStyle.Fill;
      assignmentViewHost.ViewsLabelVisible = true;
      splitContainer.Panel2.Controls.Add(assignmentViewHost);
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= new PropertyChangedEventHandler(Content_PropertyChanged);
      if (Content.Assignment != null) Content.Assignment.ItemChanged -= new EventHandler<EventArgs<int>>(Assignment_ItemChanged);
      if (Content.RowNames != null) Content.RowNames.ItemChanged += new EventHandler<EventArgs<int>>(Names_ItemChanged);
      if (Content.ColumnNames != null) Content.ColumnNames.ItemChanged -= new EventHandler<EventArgs<int>>(Names_ItemChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += new PropertyChangedEventHandler(Content_PropertyChanged);
      if (Content.Assignment != null) Content.Assignment.ItemChanged += new EventHandler<EventArgs<int>>(Assignment_ItemChanged);
      if (Content.RowNames != null) Content.RowNames.ItemChanged += new EventHandler<EventArgs<int>>(Names_ItemChanged);
      if (Content.ColumnNames != null) Content.ColumnNames.ItemChanged += new EventHandler<EventArgs<int>>(Names_ItemChanged);
    }

    private void Assignment_ItemChanged(object sender, EventArgs<int> e) {
      if (sender != Content.Assignment)
        ((Permutation)sender).ItemChanged -= new EventHandler<EventArgs<int>>(Assignment_ItemChanged);
      else UpdateAssignmentMatrix();
    }

    private void Names_ItemChanged(object sender, EventArgs<int> e) {
      UpdateAssignmentMatrix();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        qualityView.Content = null;
        assignmentViewHost.Content = null;
        assignmentDataGridView.Rows.Clear();
      } else {
        qualityView.Content = Content.Quality;
        assignmentViewHost.Content = Content.Assignment;
        UpdateAssignmentMatrix();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      assignmentDataGridView.Enabled = Content != null;
    }

    #region Event Handlers
    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch (e.PropertyName) {
        case "Quality": qualityView.Content = Content.Quality;
          break;
        case "Assignment":
          if (Content.Assignment != null)
            Content.Assignment.ItemChanged += new EventHandler<EventArgs<int>>(Assignment_ItemChanged);
          assignmentViewHost.Content = Content.Assignment;
          UpdateAssignmentMatrix();
          break;
        case "RowNames":
          if (Content.RowNames != null)
            Content.RowNames.ItemChanged += new EventHandler<EventArgs<int>>(Names_ItemChanged);
          UpdateAssignmentMatrix();
          break;
        case "ColumnNames":
          if (Content.ColumnNames != null)
            Content.ColumnNames.ItemChanged += new EventHandler<EventArgs<int>>(Names_ItemChanged);
          UpdateAssignmentMatrix();
          break;
        default: break;
      }
    }
    #endregion

    private void UpdateAssignmentMatrix() {
      assignmentDataGridView.Rows.Clear();
      if (Content.Assignment != null) {
        string rowName, colName;
        var rows = new DataGridViewRow[Content.Assignment.Length];
        for (int i = 0; i < Content.Assignment.Length; i++) {
          if (Content.RowNames != null && Content.RowNames.Length > i)
            rowName = Content.RowNames[i];
          else rowName = "Row " + (i + 1).ToString();
          if (Content.ColumnNames != null && Content.ColumnNames.Length > Content.Assignment[i])
            colName = Content.ColumnNames[Content.Assignment[i]];
          else colName = "Column " + (Content.Assignment[i] + 1).ToString();
          rows[i] = new DataGridViewRow();
          rows[i].CreateCells(assignmentDataGridView, new string[] { rowName, colName });
        }
        assignmentDataGridView.Rows.AddRange(rows);
        assignmentDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
      }
    }

  }
}
