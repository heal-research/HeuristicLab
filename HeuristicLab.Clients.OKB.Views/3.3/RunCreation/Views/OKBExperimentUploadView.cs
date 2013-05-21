#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [View("OKBExperimentUpload View")]
  [Content(typeof(IOptimizer), false)]
  public partial class OKBExperimentUploadView : ItemView {
    public new IOptimizer Content {
      get { return (IOptimizer)base.Content; }
      set { base.Content = value; }
    }

    private const string algorithmTypeParameterName = "Algorithm Type";
    private const string problemTypeParameterName = "Problem Type";
    private const string algorithmNameParameterName = "Algorithm Name";
    private const string problemNameParameterName = "Problem Name";
    private const int algorithmColumnIndex = 3;
    private const int problemColumnIndex = 6;

    private List<IRun> runs = new List<IRun>();
    private List<Problem> problems = new List<Problem>();
    private List<Algorithm> algorithms = new List<Algorithm>();
    Algorithm selectedAlgorithm = null;
    Problem selectedProblem = null;
    private ProgressView progressView;
    private Progress progress;

    public OKBExperimentUploadView() {
      InitializeComponent();
      progress = new Progress() {
        CanBeCanceled = false,
        ProgressState = ProgressState.Finished
      };
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ClearRuns();
      } else {
        AddRuns(Content);
      }
    }

    private void AddRuns(IItem item) {
      if (item is Experiment) {
        runs.AddRange((item as Experiment).Runs);
        DisplayRuns((item as Experiment).Runs);
      } else if (item is RunCollection) {
        runs.AddRange((item as RunCollection));
        DisplayRuns((item as RunCollection));
      } else if (item is IOptimizer) {
        runs.AddRange((item as IOptimizer).Runs);
        DisplayRuns((item as IOptimizer).Runs);
      } else if (item is IRun) {
        runs.Add(item as IRun);
        RunCollection tmp = new RunCollection();
        tmp.Add(item as IRun);
        DisplayRuns(tmp);
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      RunCreationClient.Instance.Refreshing += new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed += new EventHandler(RunCreationClient_Refreshed);
      progressView = new ProgressView(this, progress);
    }

    protected override void DeregisterContentEvents() {
      RunCreationClient.Instance.Refreshing -= new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed -= new EventHandler(RunCreationClient_Refreshed);
      if (progressView != null) {
        progressView.Content = null;
        progressView.Dispose();
        progressView = null;
      }
      base.DeregisterContentEvents();
    }

    private void DisplayError(Exception ex) {
      PluginInfrastructure.ErrorHandling.ShowErrorDialog("An error occured while retrieving algorithm and problem information from the OKB.", ex);
    }

    private void DisplayRuns(RunCollection runs) {
      if (RunCreationClient.Instance.Algorithms == null || RunCreationClient.Instance.Algorithms.Count() == 0) {
        Action a = new Action(delegate {
          RunCreationClient.Instance.Refresh();
          CreateUI(runs);
        });

        Task.Factory.StartNew(a).ContinueWith((t) => { DisplayError(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
      } else {
        CreateUI(runs);
      }
    }

    private void CreateUI(RunCollection runs) {
      if (InvokeRequired) {
        Invoke(new Action<RunCollection>(CreateUI), runs);
      } else {
        if (problems.Count == 0)
          problems.AddRange(RunCreationClient.Instance.Problems);
        if (algorithms.Count == 0)
          algorithms.AddRange(RunCreationClient.Instance.Algorithms);

        IItem algorithmType;
        IItem problemType;
        IItem algorithmName;
        IItem problemName;

        DataGridViewComboBoxColumn cmbAlgorithm = dataGridView.Columns[algorithmColumnIndex] as DataGridViewComboBoxColumn;
        cmbAlgorithm.DataSource = algorithms;
        cmbAlgorithm.DisplayMember = "Name";

        DataGridViewComboBoxColumn cmbProblem = dataGridView.Columns[problemColumnIndex] as DataGridViewComboBoxColumn;
        cmbProblem.DataSource = problems;
        cmbProblem.DisplayMember = "Name";

        foreach (IRun run in runs) {
          int idx = dataGridView.Rows.Add(run.Name);
          DataGridViewRow curRow = dataGridView.Rows[idx];
          curRow.Tag = run;

          if (run.Parameters.TryGetValue(algorithmTypeParameterName, out algorithmType)) {
            HeuristicLab.Data.StringValue algStr = algorithmType as HeuristicLab.Data.StringValue;
            if (algStr != null) {
              curRow.Cells[1].Value = algStr;
            }
          }

          if (run.Parameters.TryGetValue(algorithmNameParameterName, out algorithmName)) {
            HeuristicLab.Data.StringValue algStr = algorithmName as HeuristicLab.Data.StringValue;
            if (algStr != null) {
              curRow.Cells[2].Value = algStr;
            }
          }

          if (run.Parameters.TryGetValue(problemTypeParameterName, out problemType)) {
            HeuristicLab.Data.StringValue prbStr = problemType as HeuristicLab.Data.StringValue;
            if (prbStr != null) {
              curRow.Cells[4].Value = prbStr;
            }
          }

          if (run.Parameters.TryGetValue(problemNameParameterName, out problemName)) {
            HeuristicLab.Data.StringValue prbStr = problemName as HeuristicLab.Data.StringValue;
            if (prbStr != null) {
              curRow.Cells[5].Value = prbStr;
            }
          }
        }
      }
    }

    private void ClearRuns() {
      if (InvokeRequired) {
        Invoke(new Action(ClearRuns));
      } else {
        dataGridView.Rows.Clear();
        runs.Clear();
      }
    }

    private void RunCreationClient_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshing), sender, e);
      } else {
        progress.Status = "Refreshing algorithms and problems...";
        progress.ProgressState = ProgressState.Started;
      }
    }

    private void RunCreationClient_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshed), sender, e);
      } else {
        progress.Finish();
        SetEnabledStateOfControls();
      }
    }

    private void btnUpload_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(UploadAsync);
      task.ContinueWith((t) => {
        progress.Finish();
        PluginInfrastructure.ErrorHandling.ShowErrorDialog("An exception occured while uploading the runs to the OKB.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void UploadAsync() {
      progress.Status = "Uploading runs to OKB...";
      progress.ProgressValue = 0;
      progress.ProgressState = ProgressState.Started;
      double count = dataGridView.Rows.Count;
      int i = 0;
      foreach (DataGridViewRow row in dataGridView.Rows) {
        selectedAlgorithm = algorithms.Where(x => x.Name == row.Cells[algorithmColumnIndex].Value.ToString()).FirstOrDefault();
        selectedProblem = problems.Where(x => x.Name == row.Cells[problemColumnIndex].Value.ToString()).FirstOrDefault();
        if (selectedAlgorithm == null || selectedProblem == null) {
          throw new ArgumentException("Can't retrieve the algorithm/problem to upload");
        }

        OKBRun run = new OKBRun(selectedAlgorithm.Id, selectedProblem.Id, row.Tag as IRun, UserInformation.Instance.User.Id);
        run.Store();
        i++;
        progress.ProgressValue = ((double)i) / count;
      }
      progress.Finish();
      ClearRuns();
    }

    private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView[e.ColumnIndex, e.RowIndex].Value != null) {
        string curVal = dataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
        selectedAlgorithm = algorithms.Where(x => x.Name == curVal).FirstOrDefault();
        selectedProblem = problems.Where(x => x.Name == curVal).FirstOrDefault();

        if (selectedAlgorithm != null || selectedProblem != null) {
          Point pos = this.PointToClient(Cursor.Position);
          contextMenu.Show(this, pos);
        }
      }
    }

    private void setColumnToThisValueToolStripMenuItem_Click(object sender, EventArgs e) {
      if (selectedAlgorithm != null) {
        for (int i = 0; i < dataGridView.Rows.Count; i++) {
          var row = dataGridView.Rows[i];
          row.Cells[algorithmColumnIndex].Value = selectedAlgorithm.Name;
        }
      } else if (selectedProblem != null) {
        for (int i = 0; i < dataGridView.Rows.Count; i++) {
          var row = dataGridView.Rows[i];
          row.Cells[problemColumnIndex].Value = selectedProblem.Name;
        }
      }
      selectedAlgorithm = null;
      selectedProblem = null;
    }

    private void dataGridView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IRun
        || e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IOptimizer) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }

    private void dataGridView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem optimizer = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IItem;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) optimizer = (IItem)optimizer.Clone();
        AddRuns(optimizer);
      }
    }

    private void clearButton_Click(object sender, EventArgs e) {
      ClearRuns();
    }
  }
}
