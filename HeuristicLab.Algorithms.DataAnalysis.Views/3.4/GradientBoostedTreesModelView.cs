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

using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Gradient boosted tree model")]
  [Content(typeof(GradientBoostedTreesSolution), false)]
  public partial class GradientBoostedTreesModelView : DataAnalysisSolutionEvaluationView {
    public override Image ViewImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    public new GradientBoostedTreesSolution Content {
      get { return (GradientBoostedTreesSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    public GradientBoostedTreesModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        listBox.Items.Clear();
      } else {
        viewHost.Content = null;
        listBox.Items.Clear();
        foreach (var e in Content.Model.Models) {
          listBox.Items.Add(e);
        }
      }
    }

    private void listBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      var model = listBox.SelectedItem;
      if (model == null) viewHost.Content = null;
      else {
        var treeModel = model as RegressionTreeModel;
        if (treeModel != null)
          viewHost.Content = treeModel.CreateSymbolicRegressionSolution(Content.ProblemData);
        else {
          var regModel = model as IRegressionModel;
          viewHost.Content = regModel;
        }
      }
    }
  }
}
