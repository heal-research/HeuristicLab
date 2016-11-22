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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Scatter Plot Single View")]
  [Content(typeof(ScatterPlotContent), true)]
  public partial class ScatterPlotSingleView : ItemView {

    public new ScatterPlotContent Content {
      get { return (ScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotSingleView() {
      InitializeComponent();
    }

    public void InitData() {
      IEnumerable<string> variables = Content.PreprocessingData.GetDoubleVariableNames();

      // add variables to combo boxes
      comboBoxXVariable.Items.Clear();
      comboBoxYVariable.Items.Clear();
      comboBoxColor.Items.Clear();
      comboBoxXVariable.Items.AddRange(variables.ToArray());
      comboBoxYVariable.Items.AddRange(variables.ToArray());
      comboBoxColor.Items.Add("-");
      for (int i = 0; i < Content.PreprocessingData.Columns; ++i) {
        if (Content.PreprocessingData.VariableHasType<double>(i)) {
          double distinctValueCount = Content.PreprocessingData.GetValues<double>(i).GroupBy(x => x).Count();
          if (distinctValueCount <= 20)
            comboBoxColor.Items.Add(Content.PreprocessingData.GetVariableName(i));
        }
      }

      // use x and y variable from content
      if (Content.SelectedXVariable != null && Content.SelectedYVariable != null && Content.SelectedColorVariable != null) {
        comboBoxXVariable.SelectedItem = Content.SelectedXVariable;
        comboBoxYVariable.SelectedItem = Content.SelectedYVariable;
        comboBoxColor.SelectedItem = Content.SelectedColorVariable;
      } else {
        if (variables.Count() >= 2) {
          comboBoxXVariable.SelectedIndex = 0;
          comboBoxYVariable.SelectedIndex = 1;
          comboBoxColor.SelectedIndex = 0;
          UpdateScatterPlot();
        }
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
      }
    }

    private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateScatterPlot();
    }

    private void UpdateScatterPlot() {
      if (comboBoxXVariable.SelectedItem != null && comboBoxYVariable.SelectedItem != null && comboBoxColor.SelectedItem != null) {
        //get scatter plot with selected x and y variable
        ScatterPlot scatterPlot = Content.CreateScatterPlot(
          (string)comboBoxXVariable.SelectedItem,
          (string)comboBoxYVariable.SelectedItem,
          (string)comboBoxColor.SelectedItem);
        scatterPlotView.Content = scatterPlot;

        //save selected x and y variable in content
        this.Content.SelectedXVariable = (string)comboBoxXVariable.SelectedItem;
        this.Content.SelectedYVariable = (string)comboBoxYVariable.SelectedItem;
        this.Content.SelectedColorVariable = (string)comboBoxColor.SelectedItem;
      }
    }
  }
}
