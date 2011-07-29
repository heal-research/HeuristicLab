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

using System;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("RegressionEnsembleSolution View")]
  [Content(typeof(RegressionEnsembleSolution), true)]
  public partial class RegressionEnsembleSolutionView : RegressionSolutionView {
    public RegressionEnsembleSolutionView() {
      InitializeComponent();
      AddViewListViewItem(typeof(RegressionEnsembleSolutionModelView), HeuristicLab.Common.Resources.VSImageLibrary.Function);
    }

    public new RegressionEnsembleSolution Content {
      get { return (RegressionEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (Content != null && itemsListView.SelectedItems.Count == 1 && itemsListView.SelectedItems[0].Tag == typeof(RegressionEnsembleSolutionModelView)) {
        Type viewType = (Type)itemsListView.SelectedItems[0].Tag;
        var view = MainFormManager.MainForm.ShowContent(Content.RegressionSolutions, viewType);
        view.ReadOnly = false;
        view.Locked = Locked;
      } else
        base.itemsListView_DoubleClick(sender, e);
    }

    protected override void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null && itemsListView.SelectedItems.Count == 1 && itemsListView.SelectedItems[0].Tag == typeof(RegressionEnsembleSolutionModelView)) {
        detailsGroupBox.Enabled = true;
        Type viewType = (Type)itemsListView.SelectedItems[0].Tag;
        viewHost.ViewType = viewType;
        viewHost.Content = Content.RegressionSolutions;
        viewHost.ReadOnly = false;
        viewHost.Locked = Locked;
      } else
        base.itemsListView_SelectedIndexChanged(sender, e);
    }
  }
}
