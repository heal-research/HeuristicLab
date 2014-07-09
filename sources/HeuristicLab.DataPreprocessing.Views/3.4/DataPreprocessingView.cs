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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("DataPreprocessing View")]
  [Content(typeof(PreprocessingContext), true)]
  [Content(typeof(IPreprocessingContext), false)]
  public partial class DataPreprocessingView : ItemView {

    public DataPreprocessingView() {
      InitializeComponent();
    }

    public new IPreprocessingContext Content {
      get { return (IPreprocessingContext)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        var data = Content.Data;
        var searchLogic = new SearchLogic(data);
        var statisticsLogic = new StatisticsLogic(data, searchLogic);
        var manipulationLogic = new ManipulationLogic(data, searchLogic, statisticsLogic);
        var filterLogic = new FilterLogic(data);

        var viewShortcuts = new ItemList<IViewShortcut> {
          new DataGridContent(data, manipulationLogic, filterLogic),
          new StatisticsContent(statisticsLogic),

          new LineChartContent(data),
          new HistogramContent(data),
          new ScatterPlotContent(data),
          new CorrelationMatrixContent(Content),
          new DataCompletenessChartContent(searchLogic),
          
          new FilterContent(filterLogic),
          new ManipulationContent(manipulationLogic, searchLogic, filterLogic),
          new TransformationContent(data, filterLogic)
        };

        viewShortcutListView.Content = viewShortcuts.AsReadOnly();

        viewShortcutListView.ItemsListView.Items[0].Selected = true;
        viewShortcutListView.Select();

      } else {
        viewShortcutListView.Content = null;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Data.FilterChanged += Data_FilterChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Data.FilterChanged -= Data_FilterChanged;
    }

    void Data_FilterChanged(object sender, EventArgs e) {
      lblFilterActive.Visible = Content.Data.IsFiltered;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      viewShortcutListView.Enabled = Content != null;
      applyInNewTabButton.Enabled = Content != null;
      exportProblemButton.Enabled = Content != null && Content.Problem != null;
      undoButton.Enabled = Content != null;
    }

    private void exportProblemButton_Click(object sender, EventArgs e) {
      var problem = Content.ExportProblem();

      var saveFileDialog = new SaveFileDialog {
        Title = "Save Item",
        DefaultExt = "hl",
        Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*",
        FilterIndex = 2
      };

      var content = problem as IStorableContent;
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        bool compressed = saveFileDialog.FilterIndex != 1;
        ContentManager.Save(content, saveFileDialog.FileName, compressed);
      }
    }

    private void applyInNewTabButton_Click(object sender, EventArgs e) {
      var item = Content.Export();

      MainFormManager.MainForm.ShowContent(item);
    }

    private void undoButton_Click(object sender, EventArgs e) {
      Content.Data.Undo();
    }
  }
}