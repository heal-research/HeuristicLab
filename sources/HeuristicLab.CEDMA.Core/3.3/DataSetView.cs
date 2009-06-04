#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CEDMA.Core {
  public partial class DataSetView : ViewBase {
    private DataSet dataSet;
    public DataSet DataSet {
      get { return dataSet; }
      set { dataSet = value; }
    }
    private Results results;
    public DataSetView() {
      InitializeComponent();
      Caption = "Data Set";
    }
    public DataSetView(DataSet dataSet)
      : this() {
      DataSet = dataSet;
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      editorGroupBox.Controls.Clear();
      Control problemControl = (Control)DataSet.Problem.CreateView();
      problemControl.Dock = DockStyle.Fill;
      editorGroupBox.Controls.Add(problemControl);
      PopulateViewComboBox();
      resultsButton.Enabled = viewComboBox.SelectedItem != null;
      if (dataSet.Activated) {
        activateButton.Enabled = false;
        editorGroupBox.Enabled = false;
        viewComboBox.Enabled = true;
        resultsButton.Enabled = true;
        progressBar.Enabled = true;
      } else {
        activateButton.Enabled = true;
        editorGroupBox.Enabled = true;
        viewComboBox.Enabled = false;
        resultsButton.Enabled = false;
        progressBar.Enabled = false;
      }
    }

    private void PopulateViewComboBox() {
      DiscoveryService service = new DiscoveryService();
      IResultsViewFactory[] factories = service.GetInstances<IResultsViewFactory>();
      viewComboBox.DataSource = factories;
      viewComboBox.ValueMember = "Name";
    }

    private void activateButton_Click(object sender, EventArgs e) {
      DataSet.Activate();
      activateButton.Enabled = false;
    }

    private void resultsButton_Click(object sender, EventArgs e) {
      if (results == null)
        ReloadResults();
      try {
        IResultsViewFactory factory = (IResultsViewFactory)viewComboBox.SelectedItem;
        BackgroundWorker worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        worker.WorkerSupportsCancellation = true;
        worker.ProgressChanged += delegate(object progressChangedSender, ProgressChangedEventArgs progressChangedArgs) {
          progressBar.Value = progressChangedArgs.ProgressPercentage;
        };
        worker.DoWork += delegate(object doWorkSender, DoWorkEventArgs doWorkArgs) {
          int n = results.GetEntries().Count();
          int i = 0;
          // preload list 
          foreach (var entry in results.GetEntries()) {
            i++;
            if((((i*100) / n) % 10) == 0) worker.ReportProgress((i * 100) / n);
          }
          worker.ReportProgress(100);
        };
        resultsButton.Enabled = false;
        worker.RunWorkerAsync();
        worker.RunWorkerCompleted += delegate(object completedSender, RunWorkerCompletedEventArgs compledArgs) {
          resultsButton.Enabled = true;
          progressBar.Value = 0;
          IControl control = factory.CreateView(results);
          PluginManager.ControlManager.ShowControl(control);
        };
      }
      catch (Exception ex) {
        string text = "Couldn't load selected view: " + viewComboBox.SelectedItem + "\n" + ex.Message;
        MessageBox.Show(text, "Unable to create view", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void ReloadResults() {
      results = dataSet.GetResults();
    }
  }
}

