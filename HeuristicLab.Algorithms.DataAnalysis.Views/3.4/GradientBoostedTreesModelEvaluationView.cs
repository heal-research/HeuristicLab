﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("GBT Model Evaluation")]
  [Content(typeof(GradientBoostedTreesSolution), false)]
  public partial class GradientBoostedTreesModelEvaluationView : DataAnalysisSolutionEvaluationView {
    public new GradientBoostedTreesSolution Content {
      get { return (GradientBoostedTreesSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    public GradientBoostedTreesModelEvaluationView()
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
        viewHost.Content = ConvertModel(model);
      }
    }

    private void listBox_DoubleClick(object sender, System.EventArgs e) {
      var selectedItem = listBox.SelectedItem;
      if (selectedItem == null) return;
      MainFormManager.MainForm.ShowContent(ConvertModel(selectedItem));
    }

    private IContent ConvertModel(object model) {
      var treeModel = model as RegressionTreeModel;
      if (treeModel != null)
        return treeModel.CreateSymbolicRegressionSolution(Content.ProblemData);
      else {
        var regModel = model as IRegressionModel;
        return regModel;
      }
    }
  }
}
