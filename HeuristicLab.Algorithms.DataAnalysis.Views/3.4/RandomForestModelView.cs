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

using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;                       
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Random forest model")]
  [Content(typeof(IRandomForestModel), true)]
  public partial class RandomForestModelView : ItemView {

    public new IRandomForestModel Content {
      get { return (IRandomForestModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    public RandomForestModelView()
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
        var rfModel = Content;
        var numTrees = rfModel.NumberOfTrees;
        for (int i = 0; i < numTrees; i++) {
          listBox.Items.Add(i + 1);
        }
      }
    }

    private void listBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (listBox.SelectedItem == null) viewHost.Content = null;
      else {
        var idx = (int)listBox.SelectedItem;
        viewHost.Content = CreateModel(idx);
      }
    }

    private void listBox_DoubleClick(object sender, System.EventArgs e) {
      var selectedItem = listBox.SelectedItem;
      if (selectedItem == null) return;
      var idx = (int)listBox.SelectedItem;
      MainFormManager.MainForm.ShowContent(CreateModel(idx));
    }

    private IContent CreateModel(int idx) {
      idx -= 1;
      var rfModel = Content;
      var rfClassModel = rfModel as IClassificationModel; // rfModel is always a IRegressionModel and a IClassificationModel
      var targetVariable = rfClassModel.TargetVariable;
      if (rfModel == null) return null;
      if (idx < 0 || idx >= rfModel.NumberOfTrees)
        return null;
      var syModel = new SymbolicRegressionModel(targetVariable, rfModel.ExtractTree(idx),
          new SymbolicDataAnalysisExpressionTreeLinearInterpreter());
      return syModel;
    }
  }
}
