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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Manipulation Chart View")]
  [Content(typeof(ManipulationContent), true)]
  public partial class ManipulationView : ItemView {

    private Action[] validators;
    private Action[] manipulations;

    public ManipulationView() {
      InitializeComponent();
      cmbReplaceWith.SelectedIndex = 0;
      tabsData.Appearance = TabAppearance.FlatButtons;
      tabsData.ItemSize = new Size(0, 1);
      tabsData.SizeMode = TabSizeMode.Fixed;
      tabsPreview.Appearance = TabAppearance.FlatButtons;
      tabsPreview.ItemSize = new Size(0, 1);
      tabsPreview.SizeMode = TabSizeMode.Fixed;

      validators = new Action[] { 
        ()=>validateDeleteColumnsInfo(),
        ()=>validateDeleteColumnsVariance(),
        ()=>validateDeleteRowsInfo(),
        ()=>validateReplaceWith(),
        ()=>validateShuffle()
      };

      manipulations = new Action[] { 
        ()=>Content.ManipulationLogic.DeleteColumnsWithMissingValuesGreater(getDeleteColumnsInfo()),
        ()=>Content.ManipulationLogic.DeleteColumnsWithVarianceSmaller(getDeleteColumnsVariance()),
        ()=>Content.ManipulationLogic.DeleteRowsWithMissingValuesGreater(getRowsColumnsInfo()),
        ()=>replaceMissingValues(),
        ()=>Content.ManipulationLogic.Shuffle(shuffleSeparatelyCheckbox.Checked)
      };
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        cmbVariableNames.Items.Clear();
        foreach (var name in Content.ManipulationLogic.VariableNames) {
          cmbVariableNames.Items.Add(name);
        }
        cmbVariableNames.SelectedIndex = 0;
        CheckFilters();
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.FilterLogic.FilterChanged += FilterLogic_FilterChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.FilterLogic.FilterChanged -= FilterLogic_FilterChanged;
      base.DeregisterContentEvents();
    }

    private void FilterLogic_FilterChanged(object sender, EventArgs e) {
      if (Content != null) {
        CheckFilters();
      }
    }

    private void CheckFilters() {
      if (Content.FilterLogic.IsFiltered) {
        tabsPreview.SelectedIndex = 0;
        lstMethods.Enabled = false;
        tabsData.Enabled = false;
        tabsPreview.Enabled = false;
        lblPreviewInActive.Visible = true;
        btnApply.Enabled = false;
      } else {
        lblPreviewInActive.Visible = false;
        tabsData.Enabled = true;
        tabsPreview.Enabled = true;
        lstMethods.Enabled = true;
        lstMethods_SelectedIndexChanged(null, null);
      }
    }

    private double getDeleteColumnsInfo() {
      return double.Parse(txtDeleteColumnsInfo.Text);
    }

    private double getDeleteColumnsVariance() {
      return double.Parse(txtDeleteColumnsVariance.Text);
    }

    private double getRowsColumnsInfo() {
      return double.Parse(txtDeleteRowsInfo.Text);
    }

    private void replaceMissingValues() {
      var allIndices = Content.SearchLogic.GetMissingValueIndices();
      var columnIndex = cmbVariableNames.SelectedIndex;
      var columnIndices = new Dictionary<int, IList<int>>{
          {columnIndex,   allIndices[columnIndex]}
      };

      switch (cmbReplaceWith.SelectedIndex) {
        case 0: //Value
          Content.ManipulationLogic.ReplaceIndicesByValue(columnIndices, txtReplaceValue.Text);
          break;
        case 1: //Average
          Content.ManipulationLogic.ReplaceIndicesByAverageValue(columnIndices);
          break;
        case 2: //Median
          Content.ManipulationLogic.ReplaceIndicesByMedianValue(columnIndices);
          break;
        case 3: //Most Common
          Content.ManipulationLogic.ReplaceIndicesByMostCommonValue(columnIndices);
          break;
        case 4: //Random
          Content.ManipulationLogic.ReplaceIndicesByRandomValue(columnIndices);
          break;
      }
    }

    private void validateDeleteColumnsInfo() {
      validateDoubleTextBox(txtDeleteColumnsInfo.Text);
      if (btnApply.Enabled) {
        int count = Content.ManipulationLogic.ColumnsWithMissingValuesGreater(getDeleteColumnsInfo()).Count;
        int rowCount = Content.FilterLogic.PreprocessingData.Rows;
        lblPreviewColumnsInfo.Text = count + " column" + (count > 1 || count == 0 ? "s" : "") + " of " + rowCount + " (" + string.Format("{0:F2}%", 100d / rowCount * count) + ") were detected with more than " + txtDeleteColumnsInfo.Text + "% missing values.";
        if (count > 0) {
          lblPreviewColumnsInfo.Text += Environment.NewLine + Environment.NewLine + "Please press the button \"Apply Manipulation\" if you wish to delete those columns.";
        } else {
          btnApply.Enabled = false;
        }
      } else {
        lblPreviewColumnsInfo.Text = "Preview not possible yet - please input the limit above.";
      }
    }

    private void validateDeleteColumnsVariance() {
      validateDoubleTextBox(txtDeleteColumnsVariance.Text);
      if (btnApply.Enabled) {
        int count = Content.ManipulationLogic.ColumnsWithVarianceSmaller(getDeleteColumnsVariance()).Count;
        int rowCount = Content.FilterLogic.PreprocessingData.Rows;
        lblPreviewColumnsVariance.Text = count + " column" + (count > 1 || count == 0 ? "s" : "") + " of " + rowCount + " (" + string.Format("{0:F2}%", 100d / rowCount * count) + ") were detected with a variance smaller than " + txtDeleteColumnsVariance.Text + ".";
        if (count > 0) {
          lblPreviewColumnsVariance.Text += Environment.NewLine + Environment.NewLine + "Please press the button \"Apply Manipulation\" if you wish to delete those columns.";
        } else {
          btnApply.Enabled = false;
        }
      } else {
        lblPreviewColumnsVariance.Text = "Preview not possible yet - please input the limit for the variance above.";
      }
    }

    private void validateDeleteRowsInfo() {
      validateDoubleTextBox(txtDeleteRowsInfo.Text);
      if (btnApply.Enabled) {
        int count = Content.ManipulationLogic.RowsWithMissingValuesGreater(getRowsColumnsInfo()).Count;
        int rowCount = Content.FilterLogic.PreprocessingData.Rows;
        lblPreviewRowsInfo.Text = count + " row" + (count > 1 || count == 0 ? "s" : "") + " of " + rowCount + " (" + string.Format("{0:F2}%", 100d / rowCount * count) + ") were detected with more than " + txtDeleteRowsInfo.Text + "% missing values.";
        if (count > 0) {
          lblPreviewRowsInfo.Text += Environment.NewLine + Environment.NewLine + "Please press the button \"Apply Manipulation\" if you wish to delete those rows.";
        } else {
          btnApply.Enabled = false;
        }
      } else {
        lblPreviewRowsInfo.Text = "Preview not possible yet - please input the limit above.";
      }
    }

    private void validateReplaceWith() {
      btnApply.Enabled = false;
      string replaceWith = (string)cmbReplaceWith.SelectedItem;
      int columnIndex = cmbVariableNames.SelectedIndex;

      if (cmbReplaceWith.SelectedIndex == 0) {
        string errorMessage;
        string replaceValue = txtReplaceValue.Text;
        if (string.IsNullOrEmpty(replaceValue)) {
          lblPreviewReplaceMissingValues.Text = "Preview not possible yet - please input the text which will be used as replacement.";
        } else if (!Content.ManipulationLogic.PreProcessingData.Validate(txtReplaceValue.Text, out errorMessage, columnIndex)) {
          lblPreviewReplaceMissingValues.Text = "Preview not possible yet - " + errorMessage;
        } else {
          btnApply.Enabled = true;
        }
        replaceWith = "\"" + replaceValue + "\"";
      } else {
        btnApply.Enabled = true;
      }
      if (btnApply.Enabled) {
        var allIndices = Content.SearchLogic.GetMissingValueIndices();
        int count = allIndices[columnIndex].Count;
        int cellCount = Content.FilterLogic.PreprocessingData.Rows * Content.FilterLogic.PreprocessingData.Columns;
        lblPreviewReplaceMissingValues.Text = count + " cell" + (count > 1 || count == 0 ? "s" : "")
          + " of " + cellCount + " (" + string.Format("{0:F2}%", 100d / cellCount * count) + ") were detected with missing values which would be replaced with " + replaceWith;
        if (count > 0) {
          lblPreviewReplaceMissingValues.Text += Environment.NewLine + Environment.NewLine + "Please press the button \"Apply Manipulation\" if you wish to perform the replacement.";
        } else {
          btnApply.Enabled = false;
        }
      }
    }

    private void validateShuffle() {
      btnApply.Enabled = true;
      lblShuffleProperties.Enabled = false;
      lblShuffleProperties.Visible = false;
      shuffleSeparatelyCheckbox.Enabled = true;
      shuffleSeparatelyCheckbox.Visible = true;
    }

    public new ManipulationContent Content {
      get { return (ManipulationContent)base.Content; }
      set { base.Content = value; }
    }

    private void lstMethods_SelectedIndexChanged(object sender, System.EventArgs e) {
      int index = lstMethods.SelectedIndex;
      tabsData.SelectedIndex = index + 1;
      tabsPreview.SelectedIndex = index + 1;
      btnApply.Enabled = false;

      //in order that button is enabled if necessary input was already entered
      if (index >= 0) {
        validators[index]();
      }
    }

    private void btnApply_Click(object sender, System.EventArgs e) {
      manipulations[lstMethods.SelectedIndex]();
      switch (lstMethods.SelectedIndex) {
        case 0:
          lblPreviewColumnsInfo.Text = "columns successfully deleted.";
          break;
        case 1:
          lblPreviewColumnsVariance.Text = "columns successfully deleted.";
          break;
        case 2:
          lblPreviewRowsInfo.Text = "rows successfully deleted.";
          break;
        case 3:
          lblPreviewReplaceMissingValues.Text = "missing values successfully replaced.";
          btnApply.Enabled = false;
          break;
        case 4:
          lblPreviewShuffle.Text = "dataset shuffled successfully.";
          btnApply.Enabled = false;
          break;
      }
    }

    private void validateDoubleTextBox(String text) {
      btnApply.Enabled = false;
      if (!string.IsNullOrEmpty(text)) {
        double percent;
        if (Double.TryParse(text, out percent)) {
          btnApply.Enabled = true;
        }
      }
    }

    private void txtDeleteColumnsInfo_TextChanged(object sender, EventArgs e) {
      validateDeleteColumnsInfo();
    }

    private void txtDeleteColumnsVariance_TextChanged(object sender, EventArgs e) {
      validateDeleteColumnsVariance();
    }

    private void txtDeleteRowsInfo_TextChanged(object sender, EventArgs e) {
      validateDeleteRowsInfo();
    }

    private void cmbReplaceWith_SelectedIndexChanged(object sender, EventArgs e) {
      bool isReplaceWithValueSelected = cmbReplaceWith.SelectedIndex == 0;
      lblValueColon.Visible = isReplaceWithValueSelected;
      txtReplaceValue.Visible = isReplaceWithValueSelected;
      validateReplaceWith();
    }

    private void txtReplaceValue_TextChanged(object sender, EventArgs e) {
      validateReplaceWith();
    }
  }
}
