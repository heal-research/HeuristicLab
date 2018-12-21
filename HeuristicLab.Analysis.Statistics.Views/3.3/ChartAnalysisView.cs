#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Analysis.Statistics.Views {
  [View("Chart Analysis", "HeuristicLab.Analysis.Statistics.Views.InfoResources.ChartAnalysisInfo.rtf")]
  [Content(typeof(RunCollection), false)]
  public sealed partial class ChartAnalysisView : ItemView {
    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    private List<IRun> runs;
    private IProgress progress;
    private bool valuesAdded = false;
    private bool suppressUpdates = false;
    private SemaphoreSlim sem = new SemaphoreSlim(1, 1);

    public ChartAnalysisView() {
      InitializeComponent();

      stringConvertibleMatrixView.DataGridView.RowHeaderMouseDoubleClick += DataGridView_RowHeaderMouseDoubleClick;

      var fittingAlgs = ApplicationManager.Manager.GetInstances<IFitting>();
      foreach (var fit in fittingAlgs) {
        fittingComboBox.Items.Add(fit);
      }
      fittingComboBox.SelectedIndex = 0;
    }

    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        stringConvertibleMatrixView.DataGridView.RowHeaderMouseDoubleClick -= DataGridView_RowHeaderMouseDoubleClick;
        components.Dispose();
      }

      base.Dispose(disposing);
    }

    #region Content Events
    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateComboboxes();
      UpdateCaption();
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Chart Analysis" : ViewAttribute.GetViewName(GetType());
    }

    private void UpdateComboboxes() {
      if (Content != null) {
        UpdateDataTableComboBox();
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ColumnsChanged += Content_ColumnsChanged;
      Content.RowsChanged += Content_RowsChanged;
      Content.CollectionReset += Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ColumnsChanged -= Content_ColumnsChanged;
      Content.RowsChanged -= Content_RowsChanged;
      Content.CollectionReset -= Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
    }

    void Content_RowsChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_RowsChanged, sender, e);
      else {
        RebuildDataTableAsync();
      }
    }

    void Content_ColumnsChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ColumnsChanged, sender, e);
      else {
        UpdateDataTableComboBox();
        RebuildDataTableAsync();
      }
    }

    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IRun>>)Content_CollectionReset, sender, e);
      else {
        UpdateComboboxes();
        RebuildDataTableAsync();
      }
    }

    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_UpdateOfRunsInProgressChanged, sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;

        if (!suppressUpdates && !valuesAdded) {
          UpdateDataTableComboBox();
          RebuildDataTableAsync();
        }
        if (valuesAdded) {
          valuesAdded = false;
        }
      }
    }
    #endregion

    #region events
    private void DataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.RowIndex >= 0) {
        IRun run = runs[stringConvertibleMatrixView.GetRowIndex(e.RowIndex)];
        IContentView view = MainFormManager.MainForm.ShowContent(run);
        if (view != null) {
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
        }
      }
    }

    private void dataTableComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateDataRowComboBox();
    }

    private void dataRowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      RebuildDataTableAsync();
    }

    private void addLineToChart_Click(object sender, EventArgs e) {
      Progress.Show(this, "Adding fitted lines to charts...", ProgressMode.Indeterminate);

      string resultName = (string)dataTableComboBox.SelectedItem;
      string rowName = (string)dataRowComboBox.SelectedItem;

      var task = Task.Factory.StartNew(() => AddLineToChart(resultName, rowName));

      task.ContinueWith((t) => {
        Progress.Hide(this);
        ErrorHandling.ShowErrorDialog("An error occured while adding lines to charts. ", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);

      task.ContinueWith((t) => {
        Progress.Hide(this);
      }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    private void AddLineToChart(string resultName, string rowName) {
      foreach (IRun run in runs) {
        DataTable resTable = (DataTable)run.Results[resultName];
        DataRow row = resTable.Rows[rowName];
        var values = row.Values.ToArray();

        var fittingAlg = fittingComboBox.SelectedItem as IFitting;
        DataRow newRow = fittingAlg.CalculateFittedLine(values);
        newRow.Name = row.Name + " (" + fittingAlg + ")";

        if (!resTable.Rows.ContainsKey(newRow.Name))
          resTable.Rows.Add(newRow);
      }
    }

    private void addValuesButton_Click(object sender, EventArgs e) {
      string resultName = (string)dataTableComboBox.SelectedItem;
      string rowName = (string)dataRowComboBox.SelectedItem;
      DoubleMatrix sm = (DoubleMatrix)stringConvertibleMatrixView.Content;

      Content.UpdateOfRunsInProgress = true;
      for (int i = 0; i < runs.Count(); i++) {
        IRun run = runs[i];

        for (int j = 0; j < sm.ColumnNames.Count(); j++) {
          if (stringConvertibleMatrixView.DataGridView.Columns[j].Visible) {
            string newResultName = resultName + " " + rowName + " " + sm.ColumnNames.ElementAt(j);
            if (!run.Results.ContainsKey(newResultName)) {
              run.Results.Add(new KeyValuePair<string, IItem>(newResultName, new DoubleValue(sm[i, j])));
            }
          }
        }
      }
      valuesAdded = true;
      Content.UpdateOfRunsInProgress = false;
    }
    #endregion

    private void UpdateDataRowComboBox() {
      string selectedItem = (string)this.dataRowComboBox.SelectedItem;

      dataRowComboBox.Items.Clear();
      var resultName = (string)dataTableComboBox.SelectedItem;
      var dataTables = from run in Content
                       where run.Results.ContainsKey(resultName)
                       select run.Results[resultName] as DataTable;
      var rowNames = (from dataTable in dataTables
                      from row in dataTable.Rows
                      select row.Name).Distinct().ToArray();

      dataRowComboBox.Items.AddRange(rowNames);
      if (selectedItem != null && dataRowComboBox.Items.Contains(selectedItem)) {
        dataRowComboBox.SelectedItem = selectedItem;
      } else if (dataRowComboBox.Items.Count > 0) {
        dataRowComboBox.SelectedItem = dataRowComboBox.Items[0];
      }
    }

    private void UpdateDataTableComboBox() {
      string selectedItem = (string)this.dataTableComboBox.SelectedItem;

      dataTableComboBox.Items.Clear();
      var dataTables = (from run in Content
                        from result in run.Results
                        where result.Value is DataTable
                        select result.Key).Distinct().ToArray();

      dataTableComboBox.Items.AddRange(dataTables);
      if (selectedItem != null && dataTableComboBox.Items.Contains(selectedItem)) {
        dataTableComboBox.SelectedItem = selectedItem;
      } else if (dataTableComboBox.Items.Count > 0) {
        dataTableComboBox.SelectedItem = dataTableComboBox.Items[0];
      }
    }

    private void RebuildDataTableAsync() {
      string resultName = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(resultName)) return;

      string rowName = (string)dataRowComboBox.SelectedItem;

      var task = Task.Factory.StartNew(() => {
        sem.Wait();
        progress = Progress.Show(this, "Calculating values...");
        RebuildDataTable(resultName, rowName);
      });

      task.ContinueWith((t) => {
        Progress.Hide(this);
        ErrorHandling.ShowErrorDialog("An error occured while calculating values. ", t.Exception);
        sem.Release();
      }, TaskContinuationOptions.OnlyOnFaulted);

      task.ContinueWith((t) => {
        Progress.Hide(this);
        sem.Release();
      }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    private void RebuildDataTable(string resultName, string rowName) {
      LinearLeastSquaresFitting llsFitting = new LinearLeastSquaresFitting();
      string[] columnNames = new string[] { "Count", "Minimum", "Maximum", "Average", "Median", "Standard Deviation", "Variance", "25th Percentile", "75th Percentile",
        "Avg. of Upper 25 %", " Avg. of Lower 25 %", "Avg. of First 25 %", "Avg. of Last 25 %", "Slope", "Intercept", "Average Relative Error" };

      runs = Content.Where(x => x.Results.ContainsKey(resultName) && x.Visible).ToList();
      DoubleMatrix dt = new DoubleMatrix(runs.Count(), columnNames.Count());
      dt.RowNames = runs.Select(x => x.Name);
      dt.ColumnNames = columnNames;

      int i = 0;
      foreach (Run run in runs) {
        DataTable resTable = (DataTable)run.Results[resultName];
        dt.SortableView = true;
        DataRow row = resTable.Rows[rowName];
        var values = row.Values.ToArray();

        double cnt = values.Count();
        double min = values.Min();
        double max = values.Max();
        double avg = values.Average();
        double median = values.Median();
        double stdDev = values.StandardDeviation();
        double variance = values.Variance();
        double percentile25 = values.Quantile(0.25);
        double percentile75 = values.Quantile(0.75);
        double lowerAvg = values.Count() > 4 ? values.OrderBy(x => x).Take((int)(values.Count() * 0.25)).Average() : double.NaN;
        double upperAvg = values.Count() > 4 ? values.OrderByDescending(x => x).Take((int)(values.Count() * 0.25)).Average() : double.NaN;
        double firstAvg = values.Count() > 4 ? values.Take((int)(values.Count() * 0.25)).Average() : double.NaN;
        double lastAvg = values.Count() > 4 ? values.Skip((int)(values.Count() * 0.75)).Average() : double.NaN;
        double slope, intercept, r;
        llsFitting.Calculate(values, out slope, out intercept);
        r = llsFitting.CalculateError(values, slope, intercept);

        dt[i, 0] = cnt;
        dt[i, 1] = min;
        dt[i, 2] = max;
        dt[i, 3] = avg;
        dt[i, 4] = median;
        dt[i, 5] = stdDev;
        dt[i, 6] = variance;
        dt[i, 7] = percentile25;
        dt[i, 8] = percentile75;
        dt[i, 9] = upperAvg;
        dt[i, 10] = lowerAvg;
        dt[i, 11] = firstAvg;
        dt[i, 12] = lastAvg;
        dt[i, 13] = slope;
        dt[i, 14] = intercept;
        dt[i, 15] = r;

        i++;
        progress.ProgressValue = ((double)runs.Count) / i;
      }
      stringConvertibleMatrixView.Content = dt;

      for (i = 0; i < runs.Count(); i++) {
        stringConvertibleMatrixView.DataGridView.Rows[i].DefaultCellStyle.ForeColor = runs[i].Color;
      }
    }
  }
}
