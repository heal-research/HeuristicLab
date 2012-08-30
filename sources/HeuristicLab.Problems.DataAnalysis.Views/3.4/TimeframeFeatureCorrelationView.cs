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

using System;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Timeframe Feature Correlation View")]
  [Content(typeof(FeatureCorrelation), false)]
  public partial class TimeframeFeatureCorrelationView : FeatureCorrelationView {

    public new FeatureCorrelation Content {
      get { return (FeatureCorrelation)base.Content; }
      set { base.Content = value; }
    }

    public TimeframeFeatureCorrelationView() {
      InitializeComponent();
      TimeframeComboBox.DataSource = Enumerable.Range(0, 16).ToList<int>();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        VariableSelectionComboBox.DataSource = Content.RowNames.ToList();
      }
    }

    protected void VariableSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      CalculateCorrelation();
    }
    protected void TimeframeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      CalculateCorrelation();
    }

    protected override void CalculateCorrelation() {
      string calc = (string)CorrelationCalcComboBox.SelectedItem;
      string partition = (string)PartitionComboBox.SelectedItem;
      string variable = (string)VariableSelectionComboBox.SelectedItem;
      if (calc != null && partition != null && variable != null) {
        DataGridView.Columns.Clear();
        DataGridView.Enabled = false;
        int frames = (int)TimeframeComboBox.SelectedItem;
        Content.CalculateTimeframeElements(calc, partition, variable, frames);
      }
    }

    protected override void UpdateColumnHeaders() {
      for (int i = 0; i < DataGridView.ColumnCount; i++) {
        DataGridView.Columns[i].HeaderText = i.ToString();
      }
    }
  }
}