#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  public partial class HiveResourceSelectorDialog : Form {
    public HiveResourceSelector HiveResourceSelector {
      get { return hiveResourceSelector; }
    }

    public HiveResourceSelectorDialog() {
      InitializeComponent();
    }

    public ISet<Resource> GetSelectedResources() { return hiveResourceSelector.SelectedResources; }

    private void HiveResourceSelectorDialog_Load(object sender, System.EventArgs e) {
      HiveAdminClient.Instance.Refreshed += new System.EventHandler(Instance_Refreshed);
      DownloadResources();
    }

    private void HiveResourceSelectorDialog_FormClosing(object sender, FormClosingEventArgs e) {
      HiveAdminClient.Instance.Refreshed -= new System.EventHandler(Instance_Refreshed);
    }

    void Instance_Refreshed(object sender, System.EventArgs e) {
      hiveResourceSelector.Content = HiveAdminClient.Instance.Resources;
    }

    private void refreshButton_Click(object sender, System.EventArgs e) {
      DownloadResources();
    }

    private void DownloadResources() {
      var task = System.Threading.Tasks.Task.Factory.StartNew(DownloadResourcesAsync);
      task.ContinueWith(t => {
        hiveResourceSelector.FinishProgressView();
        ErrorHandling.ShowErrorDialog(this, "An error occurred while downloading the tasks.", t.Exception);
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void DownloadResourcesAsync() {
      hiveResourceSelector.StartProgressView();
      HiveAdminClient.Instance.Refresh();
      hiveResourceSelector.FinishProgressView();
    }
  }
}
