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
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Preprocessing Feature Correlation View")]
  [Content(typeof(CorrelationMatrixContent), true)]
  public partial class PreprocessingFeatureCorrelationView : AsynchronousContentView {
    public new CorrelationMatrixContent Content {
      get { return (CorrelationMatrixContent)base.Content; }
      set { base.Content = value; }
    }

    public PreprocessingFeatureCorrelationView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += Data_Changed;
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= Data_Changed;
      base.DeregisterContentEvents();
    }

    private void Data_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      correlationView.Content = Content != null ? Content.ProblemData : null;
    }

    #region Check Variables
    private void checkAllButton_Click(object sender, System.EventArgs e) {
      SetVisibility(x => true);
    }
    private void checkInputsTargetButton_Click(object sender, System.EventArgs e) {
      var ppd = Content.PreprocessingData;
      SetVisibility(x => ppd.InputVariables.Contains(x) || ppd.TargetVariable == x);
    }
    private void uncheckAllButton_Click(object sender, System.EventArgs e) {
      SetVisibility(x => false);
    }
    private void SetVisibility(Func<string, bool> check) {
      var dataView = (EnhancedStringConvertibleMatrixView)correlationView.Controls.Find("DataView", searchAllChildren: true).Single();
      var ppd = Content.PreprocessingData;
      var visibilities = ppd.VariableNames.Where((v, i) => ppd.VariableHasType<double>(i)).Select(check).ToList();
      if (dataView.Content.Rows != dataView.Content.Columns || dataView.Content.Rows != visibilities.Count)
        return;

      dataView.ColumnVisibility = visibilities;
      dataView.RowVisibility = visibilities;
      dataView.UpdateColumnHeaders();
      dataView.UpdateRowHeaders();
    }
    #endregion
  }
}