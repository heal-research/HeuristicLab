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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Transformation View")]
  [Content(typeof(TransformationContent), true)]
  public partial class TransformationView : AsynchronousContentView {

    public new TransformationContent Content {
      get { return (TransformationContent)base.Content; }
      set { base.Content = value; }
    }

    public TransformationView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        transformationListView.Content = null;
      } else {
        transformationListView.Content = Content.CheckedTransformationList;
        CheckFilters();
      }
    }

    /// <summary>
    /// Adds eventhandlers to the current instance.
    /// </summary>
    protected override void RegisterContentEvents() {
      Content.FilterLogic.FilterChanged += FilterLogic_FilterChanged;
    }

    /// <summary>
    /// Removes the eventhandlers from the current instance.
    /// </summary>
    protected override void DeregisterContentEvents() {
      Content.FilterLogic.FilterChanged -= FilterLogic_FilterChanged;
    }

    void FilterLogic_FilterChanged(object sender, EventArgs e) {
      if (Content != null) {
        CheckFilters();
      }
    }

    private void CheckFilters() {
      if (Content.FilterLogic.IsFiltered) {
        applyButton.Enabled = false;
        lblFilterNotice.Visible = true;
      } else {
        applyButton.Enabled = true;
        lblFilterNotice.Visible = false;
      }
    }

    private void applyButton_Click(object sender, EventArgs e) {
      var transformations = Content.CheckedTransformationList.CheckedItems.Select(x => x.Value);

      if (transformations.Any(x => ((Transformation)x).ColumnParameter.Value == null)) {
        MessageBox.Show(this, "Parameter \"Column\" of a selected Transformation is not set.", "Applying Transformations...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var transformator = new PreprocessingTransformator(Content.Data);
      bool preserve = preserveColumnsCheckbox.CheckState == CheckState.Checked;
      string errorMsg;
      bool success = transformator.ApplyTransformations(transformations, preserve, out errorMsg);
      if (success) {
        Content.CheckedTransformationList.Clear();
        MessageBox.Show(this, "Transformations applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } else {
        MessageBox.Show(this,
          "Error in Transformation.\nValue is copied when transformion of cell failed.\n" + errorMsg,
          "Transformation failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
  }
}
