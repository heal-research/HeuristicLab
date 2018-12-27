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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.Query {
  [View("OKB Query")]
  [Content(typeof(QueryClient), true)]
  public sealed partial class QueryView : AsynchronousContentView {
    private CancellationTokenSource cancellationTokenSource;
    private CombinedFilterView combinedFilterView;

    public new QueryClient Content {
      get { return (QueryClient)base.Content; }
      set { base.Content = value; }
    }

    public QueryView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Refreshing -= new EventHandler(Content_Refreshing);
      Content.Refreshed -= new EventHandler(Content_Refreshed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Refreshing += new EventHandler(Content_Refreshing);
      Content.Refreshed += new EventHandler(Content_Refreshed);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      CreateFilterView();
      runCollectionView.Content = null;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      resultsGroupBox.Enabled = combinedFilterView != null;
    }

    #region Load Results
    private void LoadResultsAsync(int batchSize) {
      bool includeBinaryValues = includeBinaryValuesCheckBox.Checked;
      IEnumerable<ValueName> valueNames = constraintsCheckedListBox.CheckedItems.Cast<ValueName>();

      Cursor = Cursors.AppStarting;
      resultsInfoLabel.Text = "Loading Results ...";
      resultsProgressBar.Value = 0;
      resultsProgressBar.Step = batchSize;
      abortButton.Enabled = true;
      resultsInfoPanel.Visible = true;
      splitContainer.Enabled = false;
      cancellationTokenSource = new CancellationTokenSource();
      CancellationToken cancellationToken = cancellationTokenSource.Token;

      Task task = Task.Factory.StartNew(() => {
        var ids = QueryClient.Instance.GetRunIds(combinedFilterView.Content);
        int idsCount = ids.Count();

        Invoke(new Action(() => {
          resultsInfoLabel.Text = "Loaded 0 of " + idsCount.ToString() + " Results ...";
          resultsProgressBar.Maximum = idsCount;
        }));

        RunCollection runs = new RunCollection();
        runCollectionView.Content = runs;
        while (ids.Count() > 0) {
          cancellationToken.ThrowIfCancellationRequested();
          if (AllValueNamesChecked()) {
            runs.AddRange(QueryClient.Instance.GetRuns(ids.Take(batchSize), includeBinaryValues).Select(x => QueryClient.Instance.ConvertToOptimizationRun(x)));
          } else {
            runs.AddRange(QueryClient.Instance.GetRunsWithValues(ids.Take(batchSize), includeBinaryValues, valueNames).Select(x => QueryClient.Instance.ConvertToOptimizationRun(x)));
          }
          ids = ids.Skip(batchSize);
          Invoke(new Action(() => {
            resultsInfoLabel.Text = "Loaded " + runs.Count + " of " + idsCount.ToString() + " Results ...";
            resultsProgressBar.PerformStep();
          }));
        }
      }, cancellationToken);
      task.ContinueWith(t => {
        Invoke(new Action(() => {
          cancellationTokenSource.Dispose();
          cancellationTokenSource = null;
          resultsInfoPanel.Visible = false;
          splitContainer.Enabled = true;
          this.Cursor = Cursors.Default;
          SetEnabledStateOfControls();
          try {
            t.Wait();
          } catch (AggregateException ex) {
            try {
              ex.Flatten().Handle(x => x is OperationCanceledException);
            } catch (AggregateException remaining) {
              if (remaining.InnerExceptions.Count == 1) ErrorHandling.ShowErrorDialog(this, "Refresh results failed.", remaining.InnerExceptions[0]);
              else ErrorHandling.ShowErrorDialog(this, "Refresh results failed.", remaining);
            }
          }
        }));
      });
    }
    #endregion

    private void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        filtersInfoPanel.Visible = true;
        splitContainer.Enabled = false;
      }
    }
    private void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        CreateFilterView();
        CreateConstraintsView();
        filtersInfoPanel.Visible = false;
        splitContainer.Enabled = true;
        Cursor = Cursors.Default;
        SetEnabledStateOfControls();
      }
    }

    private void refreshFiltersButton_Click(object sender, EventArgs e) {
      Content.RefreshAsync(new Action<Exception>((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Refresh failed.", ex)));
    }

    private void refreshResultsButton_Click(object sender, EventArgs e) {
      LoadResultsAsync(10);
    }

    private void abortButton_Click(object sender, EventArgs e) {
      if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
      abortButton.Enabled = false;
    }

    private void CreateFilterView() {
      combinedFilterView = null;
      filterPanel.Controls.Clear();
      if ((Content != null) && (Content.Filters != null)) {
        CombinedFilter filter = Content.Filters.OfType<CombinedFilter>().Where(x => x.Operation == BooleanOperation.And).FirstOrDefault();
        if (filter != null) {
          combinedFilterView = (CombinedFilterView)MainFormManager.CreateView(typeof(CombinedFilterView));
          combinedFilterView.Content = (CombinedFilter)filter.Clone();
          Control control = (Control)combinedFilterView;
          control.Dock = DockStyle.Fill;
          filterPanel.Controls.Add(control);
        }
      }
    }

    private void CreateConstraintsView() {
      constraintsCheckedListBox.Items.Clear();
      constraintsCheckedListBox.Items.AddRange(QueryClient.Instance.ValueNames.ToArray());
      SetCheckedState(true);
    }

    private void SetCheckedState(bool val) {
      for (int i = 0; i < constraintsCheckedListBox.Items.Count; i++) {
        constraintsCheckedListBox.SetItemChecked(i, val);
      }
    }

    private bool AllValueNamesChecked() {
      return constraintsCheckedListBox.Items.Count == constraintsCheckedListBox.CheckedItems.Count;
    }

    private void selectAllButton_Click(object sender, EventArgs e) {
      SetCheckedState(true);
    }

    private void deselectAllButton_Click(object sender, EventArgs e) {
      SetCheckedState(false);
    }
  }
}
