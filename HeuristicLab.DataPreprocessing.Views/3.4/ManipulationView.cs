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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Manipulation Chart View")]
  [Content(typeof(ManipulationContent), true)]
  public partial class ManipulationView : ItemView {
    private Action[] validators;
    private Action[] manipulations;

    public new ManipulationContent Content {
      get { return (ManipulationContent)base.Content; }
      set { base.Content = value; }
    }

    public ManipulationView() {
      InitializeComponent();
      tabsData.Appearance = TabAppearance.FlatButtons;
      tabsData.ItemSize = new Size(0, 1);
      tabsData.SizeMode = TabSizeMode.Fixed;
      tabsPreview.Appearance = TabAppearance.FlatButtons;
      tabsPreview.ItemSize = new Size(0, 1);
      tabsPreview.SizeMode = TabSizeMode.Fixed;

      validators = new Action[] {
        () => ValidateDeleteColumnsInfo(),
        () => ValidateDeleteColumnsVariance(),
        () => ValidateDeleteRowsInfo(),
      };

      manipulations = new Action[] {
        () => Content.DeleteColumnsWithMissingValuesGreater(GetDeleteColumnsInfo()),
        () => Content.DeleteColumnsWithVarianceSmaller(GetDeleteColumnsVariance()),
        () => Content.DeleteRowsWithMissingValuesGreater(GetRowsColumnsInfo()),
      };
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        CheckFilters();
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PreprocessingData.FilterChanged += FilterLogic_FilterChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.PreprocessingData.FilterChanged -= FilterLogic_FilterChanged;
      base.DeregisterContentEvents();
    }

    private void FilterLogic_FilterChanged(object sender, EventArgs e) {
      if (Content != null) {
        CheckFilters();
      }
    }

    private void CheckFilters() {
      if (Content.PreprocessingData.IsFiltered) {
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

    private double GetDeleteColumnsInfo() {
      return double.Parse(txtDeleteColumnsInfo.Text);
    }

    private double GetDeleteColumnsVariance() {
      return double.Parse(txtDeleteColumnsVariance.Text);
    }

    private double GetRowsColumnsInfo() {
      return double.Parse(txtDeleteRowsInfo.Text);
    }

    private void ValidateDeleteColumnsInfo() {
      ValidateDoubleTextBox(txtDeleteColumnsInfo.Text);
      if (btnApply.Enabled) {
        var filteredColumns = Content.ColumnsWithMissingValuesGreater(GetDeleteColumnsInfo());
        int count = filteredColumns.Count;
        int columnCount = Content.PreprocessingData.Columns;
        lblPreviewColumnsInfo.Text = string.Format("{0} column{1} of {2} ({3}) were detected with more than {4}% missing values.", count, (count > 1 || count == 0 ? "s" : ""), columnCount, string.Format("{0:F2}%", 100d / columnCount * count), txtDeleteColumnsInfo.Text);

        //only display column names more than 0 and fewer than 50 are affected
        if (count > 0 && count < 50) {
          StringBuilder sb = new StringBuilder();
          sb.Append(Environment.NewLine);
          sb.Append("Columns: ");
          sb.Append(Content.PreprocessingData.VariableNames.ElementAt(filteredColumns.ElementAt(0)));
          for (int i = 1; i < filteredColumns.Count; i++) {
            string columnName = Content.PreprocessingData.VariableNames.ElementAt(filteredColumns.ElementAt(i));
            sb.Append(", ");
            sb.Append(columnName);
          }
          sb.Append(Environment.NewLine);
          sb.Append("Please press the button \"Apply Manipulation\" if you wish to delete those columns.");

          lblPreviewColumnsInfo.Text += sb.ToString();
        }

        btnApply.Enabled = count > 0;
      } else {
        lblPreviewColumnsInfo.Text = "Preview not possible yet - please input the limit above.";
      }
    }

    private void ValidateDeleteColumnsVariance() {
      ValidateDoubleTextBox(txtDeleteColumnsVariance.Text);
      if (btnApply.Enabled) {
        var filteredColumns = Content.ColumnsWithVarianceSmaller(GetDeleteColumnsVariance());
        int count = filteredColumns.Count;
        int columnCount = Content.PreprocessingData.Columns;
        lblPreviewColumnsVariance.Text = string.Format("{0} column{1} of {2} ({3}) were detected with a variance smaller than {4}.", count, (count > 1 || count == 0 ? "s" : ""), columnCount, string.Format("{0:F2}%", 100d / columnCount * count), txtDeleteColumnsVariance.Text);

        //only display column names more than 0 and fewer than 50 are affected
        if (count > 0 && count < 50) {
          StringBuilder sb = new StringBuilder();
          sb.Append(Environment.NewLine);
          sb.Append("Columns: ");
          sb.Append(Content.PreprocessingData.VariableNames.ElementAt(filteredColumns.ElementAt(0)));
          for (int i = 1; i < filteredColumns.Count; i++) {
            string columnName = Content.PreprocessingData.VariableNames.ElementAt(filteredColumns.ElementAt(i));
            sb.Append(", ");
            sb.Append(columnName);
          }
          sb.Append(Environment.NewLine);
          sb.Append("Please press the button \"Apply Manipulation\" if you wish to delete those columns.");

          lblPreviewColumnsVariance.Text += sb.ToString();
        }

        btnApply.Enabled = count > 0;
      } else {
        lblPreviewColumnsVariance.Text = "Preview not possible yet - please input the limit for the variance above.";
      }
    }

    private void ValidateDeleteRowsInfo() {
      ValidateDoubleTextBox(txtDeleteRowsInfo.Text);
      if (btnApply.Enabled) {
        int count = Content.RowsWithMissingValuesGreater(GetRowsColumnsInfo()).Count;
        int rowCount = Content.PreprocessingData.Rows;
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
      }
    }

    private void ValidateDoubleTextBox(String text) {
      btnApply.Enabled = false;
      if (!string.IsNullOrEmpty(text)) {
        double percent;
        if (Double.TryParse(text, NumberStyles.Number ^ NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out percent)) {
          btnApply.Enabled = true;
        }
      }
    }

    private void txtDeleteColumnsInfo_TextChanged(object sender, EventArgs e) {
      ValidateDeleteColumnsInfo();
    }

    private void txtDeleteColumnsVariance_TextChanged(object sender, EventArgs e) {
      ValidateDeleteColumnsVariance();
    }

    private void txtDeleteRowsInfo_TextChanged(object sender, EventArgs e) {
      ValidateDeleteRowsInfo();
    }
  }
}
