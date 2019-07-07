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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Gradient boosted trees model")]
  [Content(typeof(IGradientBoostedTreesModel), true)]
  public partial class GradientBoostedTreesModelView : ItemView {
    #region Getter/Setter
    public new IGradientBoostedTreesModel Content {
      get { return (IGradientBoostedTreesModel)base.Content; }
      set { base.Content = value; }
    }
    #endregion

    #region Ctor
    public GradientBoostedTreesModelView()
      : base() {
      InitializeComponent();
    }
    #endregion

    #region Events
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listView.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        listView.Items.Clear();
      } else {
        viewHost.Content = null;
        listView.BeginUpdate();
        listView.Items.Clear();
        int i = 1;
        listView.Items.AddRange(
          new ListViewItem(Content.Models.First().ToString()) { Tag = Content.Models.First() }.ToEnumerable()
          .Union(Content.Models.Skip(1).Select(v => new ListViewItem("Model " + i++) { Tag = v }))
          .ToArray()
        );
        listView.EndUpdate();
      }
    }


    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        var item = listView.SelectedItems[0];
        viewHost.Content = ConvertModel(item);
      }
    }

    private void listView_DoubleClick(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        var item = listView.SelectedItems[0];
        var content = ConvertModel(item);
        if (content != null) { MainFormManager.MainForm.ShowContent(content); }
      }
    }
    #endregion

    #region Helper Methods
    private IContent ConvertModel(ListViewItem item) {
      if (item.Tag is RegressionTreeModel) {
        return (item.Tag as RegressionTreeModel).CreateSymbolicRegressionModel();
      } else if (item.Tag is IRegressionModel) {
        return item.Tag as IRegressionModel;
      } else {
        return null;
      }
    }
    #endregion
  }
}
