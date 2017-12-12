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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using View = HeuristicLab.MainForm.WindowsForms.View;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [View("OKBExperimentUpload View")]
  public partial class OKBExperimentUploadView : View {

    private const string AlgorithmTypeParameterName = "Algorithm Type";
    private const string ProblemTypeParameterName = "Problem Type";
    private const string AlgorithmNameParameterName = "Algorithm Name";
    private const string ProblemNameParameterName = "Problem Name";

    private List<IRun> runs = new List<IRun>();
    private List<Problem> problems = new List<Problem>();
    private List<Algorithm> algorithms = new List<Algorithm>();
    Algorithm selectedAlgorithm = null;
    Problem selectedProblem = null;

    public OKBExperimentUploadView() {
      InitializeComponent();
      OKBAlgorithmColumn.ValueType = typeof(Algorithm);
      OKBAlgorithmColumn.ValueMember = "Name";
      OKBAlgorithmColumn.DisplayMember = "Name";
      OKBProblemColumn.ValueType = typeof(Problem);
      OKBProblemColumn.ValueMember = "Name";
      OKBProblemColumn.DisplayMember = "Name";
      RunCreationClient.Instance.Refreshing += RunCreationClient_Refreshing;
      RunCreationClient.Instance.Refreshed += RunCreationClient_Refreshed;
    }

    private void DisposeSpecific() {
      RunCreationClient.Instance.Refreshing -= RunCreationClient_Refreshing;
      RunCreationClient.Instance.Refreshed -= RunCreationClient_Refreshed;
    }

    private bool refreshing;

    protected override void SetEnabledStateOfControls() {
      if (InvokeRequired) { Invoke((Action)SetEnabledStateOfControls); return; }
      base.SetEnabledStateOfControls();
      btnUpload.Enabled = runs.Count > 0 && !refreshing;
    }

    public void AddRuns(IItem item) {
      if (InvokeRequired) { Invoke((Action<IItem>)AddRuns, item); return; }
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
      SetEnabledStateOfControls();
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
      if (InvokeRequired) { Invoke((Action<RunCollection>)CreateUI, runs); return; }
      if (problems.Count == 0)
        problems.AddRange(RunCreationClient.Instance.Problems);
      if (algorithms.Count == 0)
        algorithms.AddRange(RunCreationClient.Instance.Algorithms);

      IItem algorithmType;
      IItem problemType;
      IItem algorithmName;
      IItem problemName;

      OKBAlgorithmColumn.DataSource = algorithms;
      OKBProblemColumn.DataSource = problems;

      foreach (IRun run in runs) {
        int idx = dataGridView.Rows.Add(run.Name);
        DataGridViewRow curRow = dataGridView.Rows[idx];
        curRow.Tag = run;

        HeuristicLab.Data.StringValue algStr = null, algTypeStr = null, prbStr = null, prbTypeStr = null;
        if (run.Parameters.TryGetValue(AlgorithmNameParameterName, out algorithmName)) {
          algStr = algorithmName as HeuristicLab.Data.StringValue;
          if (algStr != null) {
            curRow.Cells[AlgorithmNameColumn.Name].Value = algStr;
          }
        }

        if (run.Parameters.TryGetValue(AlgorithmTypeParameterName, out algorithmType)) {
          algTypeStr = algorithmType as HeuristicLab.Data.StringValue;
          if (algTypeStr != null) {
            curRow.Cells[AlgorithmTypeColumn.Name].Value = algTypeStr;
          }
        }

        var uploadOk = false;
        if (algStr != null && algTypeStr != null) {
          var alg = algorithms.FirstOrDefault(x => x.DataType.Name == algTypeStr.Value && x.Name == algStr.Value);
          if (alg != null) {
            curRow.Cells[OKBAlgorithmColumn.Name].Value = alg.Name;
            uploadOk = true;
          }
        }

        if (run.Parameters.TryGetValue(ProblemNameParameterName, out problemName)) {
          prbStr = problemName as HeuristicLab.Data.StringValue;
          if (prbStr != null) {
            curRow.Cells[ProblemNameColumn.Name].Value = prbStr;
          }
        }

        if (run.Parameters.TryGetValue(ProblemTypeParameterName, out problemType)) {
          prbTypeStr = problemType as HeuristicLab.Data.StringValue;
          if (prbTypeStr != null) {
            curRow.Cells[ProblemTypeColumn.Name].Value = prbTypeStr;
          }
        }

        if (prbStr != null && prbTypeStr != null) {
          var prb = problems.FirstOrDefault(x => x.DataType.Name == prbTypeStr.Value && x.Name == prbStr.Value);
          if (prb != null) {
            curRow.Cells[OKBProblemColumn.Name].Value = prb.Name;
          } else uploadOk = false;
        }

        curRow.Cells[UploadColumn.Name].Value = uploadOk;
      }
    }

    public void ClearRuns() {
      if (InvokeRequired) { Invoke((Action)ClearRuns); return; }
      dataGridView.Rows.Clear();
      runs.Clear();
      SetEnabledStateOfControls();
    }

    private void RunCreationClient_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) { Invoke((Action<object, EventArgs>)RunCreationClient_Refreshing, sender, e); return; }
      var message = "Refreshing algorithms and problems...";
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, message);
      refreshing = true;
      SetEnabledStateOfControls();
    }

    private void RunCreationClient_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) { Invoke((Action<object, EventArgs>)RunCreationClient_Refreshed, sender, e); return; }
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
      refreshing = false;
      SetEnabledStateOfControls();
    }

    private void btnUpload_Click(object sender, EventArgs e) {
      var task = System.Threading.Tasks.Task.Factory.StartNew(UploadAsync);
      task.ContinueWith((t) => {
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
        PluginInfrastructure.ErrorHandling.ShowErrorDialog("An exception occured while uploading the runs to the OKB.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void UploadAsync() {
      var message = "Uploading runs to OKB...";
      IProgress progress = MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, message);
      double count = dataGridView.Rows.Count;
      int i = 0;
      foreach (DataGridViewRow row in dataGridView.Rows) {
        i++;
        if (!Convert.ToBoolean(row.Cells[UploadColumn.Name].Value)) continue;
        selectedAlgorithm = algorithms.FirstOrDefault(x => x.Name == row.Cells[OKBAlgorithmColumn.Name].Value.ToString());
        selectedProblem = problems.FirstOrDefault(x => x.Name == row.Cells[OKBProblemColumn.Name].Value.ToString());
        if (selectedAlgorithm == null || selectedProblem == null) {
          throw new ArgumentException("Can't retrieve the algorithm/problem to upload");
        }

        OKBRun run = new OKBRun(selectedAlgorithm.Id, selectedProblem.Id, row.Tag as IRun, UserInformation.Instance.User.Id);
        run.Store();
        progress.ProgressValue = ((double)i) / count;
      }
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
      ClearRuns();
    }

    private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView[e.ColumnIndex, e.RowIndex].Value != null) {
        string curVal = dataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
        selectedAlgorithm = algorithms.FirstOrDefault(x => x.Name == curVal);
        selectedProblem = problems.FirstOrDefault(x => x.Name == curVal);

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
          row.Cells[OKBAlgorithmColumn.Name].Value = selectedAlgorithm.Name;
        }
      } else if (selectedProblem != null) {
        for (int i = 0; i < dataGridView.Rows.Count; i++) {
          var row = dataGridView.Rows[i];
          row.Cells[OKBProblemColumn.Name].Value = selectedProblem.Name;
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
