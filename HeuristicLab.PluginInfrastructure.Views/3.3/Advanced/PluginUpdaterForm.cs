#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class PluginUpdaterForm : Form, IStatusView {
    private InstallationManager installationManager;
    private PluginManager pluginManager;
    private string pluginDir;
    private BackgroundWorker updatePluginsBackgroundWorker;

    public PluginUpdaterForm(PluginManager pluginManager)
      : base() {
      InitializeComponent();
      Text = "HeuristicLab Plugin Manager " + AssemblyHelpers.GetFileVersion(GetType().Assembly);
      pluginManager.PluginLoaded += pluginManager_PluginLoaded;
      pluginManager.PluginUnloaded += pluginManager_PluginUnloaded;
      pluginManager.Initializing += pluginManager_Initializing;
      pluginManager.Initialized += pluginManager_Initialized;

      pluginDir = Application.StartupPath;

      installationManager = new InstallationManager(pluginDir);
      installationManager.PluginInstalled += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginInstalled);
      installationManager.PluginRemoved += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginRemoved);
      installationManager.PluginUpdated += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginUpdated);
      installationManager.PreInstallPlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreInstallPlugin);
      installationManager.PreRemovePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreRemovePlugin);
      installationManager.PreUpdatePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreUpdatePlugin);

      this.pluginManager = pluginManager;

      updatePluginsBackgroundWorker = new BackgroundWorker();
      updatePluginsBackgroundWorker.DoWork += new DoWorkEventHandler(updatePluginsBackgroundWorker_DoWork);
      updatePluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updatePluginsBackgroundWorker_RunWorkerCompleted);
    }


    private void PluginUpdaterForm_Load(object sender, EventArgs e) {
      updatePluginsBackgroundWorker.RunWorkerAsync();
      ShowProgressIndicator();
      ShowMessage("Downloading and installing plugins...");
    }

    #region event handlers for update plugins backgroundworker
    void updatePluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        ShowError("Installation Error", "There was a problem while downloading and installing updates." + Environment.NewLine + "Please check your connection settings.");
      } else {
        Close();
      }
    }

    void updatePluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      IEnumerable<IPluginDescription> installedPlugins = pluginManager.Plugins.OfType<IPluginDescription>().ToList();
      var remotePlugins = installationManager.GetRemotePluginList();
      // if there is a local plugin with same name and same major and minor version then it's an update
      var pluginsToUpdate = from remotePlugin in remotePlugins
                            let matchingLocalPlugins = from installedPlugin in installedPlugins
                                                       where installedPlugin.Name == remotePlugin.Name
                                                       where installedPlugin.Version.Major == remotePlugin.Version.Major
                                                       where installedPlugin.Version.Minor == remotePlugin.Version.Minor
                                                       where Util.IsNewerThan(remotePlugin, installedPlugin)
                                                       select installedPlugin
                            where matchingLocalPlugins.Count() > 0
                            select remotePlugin;
      if (pluginsToUpdate.Count() > 0) {
        bool canceled;
        installationManager.Update(pluginsToUpdate, out canceled);
        if (!canceled)
          pluginManager.DiscoverAndCheckPlugins();
        e.Cancel = false;
      }
    }
    #endregion

    #region plugin manager event handlers
    private void pluginManager_Initialized(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Initialized PluginInfrastructure");
    }

    private void pluginManager_Initializing(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Initializing PluginInfrastructure");
    }

    private void pluginManager_PluginUnloaded(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Unloaded " + e.Entity);
    }

    private void pluginManager_PluginLoaded(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Loaded " + e.Entity);
    }
    #endregion

    #region installation manager event handlers
    private void installationManager_PreUpdatePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0) {
        e.Cancel = (bool)Invoke((Func<IEnumerable<IPluginDescription>, bool>)ConfirmUpdateAction, e.Plugins) == false;
      }
    }

    private void installationManager_PreRemovePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0) {
        e.Cancel = (bool)Invoke((Func<IEnumerable<IPluginDescription>, bool>)ConfirmRemoveAction, e.Plugins) == false;
      }
    }

    private void installationManager_PreInstallPlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0)
        if ((bool)Invoke((Func<IEnumerable<IPluginDescription>, bool>)ConfirmInstallAction, e.Plugins) == true) {
          SetStatusStrip("Installing " + e.Plugins.Aggregate("", (a, b) => a.ToString() + "; " + b.ToString()));
          e.Cancel = false;
        } else {
          e.Cancel = true;
          SetStatusStrip("Install canceled");
        }
    }

    private void installationManager_PluginUpdated(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Updated " + e.Entity);
    }

    private void installationManager_PluginRemoved(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Removed " + e.Entity);
    }

    private void installationManager_PluginInstalled(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Installed " + e.Entity);
    }
    #endregion

    #region confirmation dialogs
    private bool ConfirmRemoveAction(IEnumerable<IPluginDescription> plugins) {
      StringBuilder strBuilder = new StringBuilder();
      foreach (var plugin in plugins) {
        foreach (var file in plugin.Files) {
          strBuilder.AppendLine(Path.GetFileName(file.Name));
        }
      }
      using (var confirmationDialog = new ConfirmationDialog("Confirm Delete", "Do you want to delete following files?", strBuilder.ToString())) {
        return (confirmationDialog.ShowDialog(this)) == DialogResult.OK;
      }
    }

    private bool ConfirmUpdateAction(IEnumerable<IPluginDescription> plugins) {
      StringBuilder strBuilder = new StringBuilder();
      foreach (var plugin in plugins) {
        strBuilder.AppendLine(plugin.ToString());
      }
      using (var confirmationDialog = new ConfirmationDialog("Confirm Update", "Do you want to update following plugins?", strBuilder.ToString())) {
        return (confirmationDialog.ShowDialog(this)) == DialogResult.OK;
      }
    }

    private bool ConfirmInstallAction(IEnumerable<IPluginDescription> plugins) {
      foreach (var plugin in plugins) {
        if (!string.IsNullOrEmpty(plugin.LicenseText)) {
          using (var licenseConfirmationBox = new LicenseConfirmationDialog(plugin)) {
            if (licenseConfirmationBox.ShowDialog(this) != DialogResult.OK)
              return false;
          }
        }
      }
      return true;
    }


    #endregion

    #region helper methods
    private void SetStatusStrip(string msg) {
      if (InvokeRequired) Invoke((Action<string>)SetStatusStrip, msg);
      else {
        statusLabel.Text = msg;
      }
    }

    #endregion


    protected override void OnClosing(CancelEventArgs e) {
      installationManager.PluginInstalled -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginInstalled);
      installationManager.PluginRemoved -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginRemoved);
      installationManager.PluginUpdated -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginUpdated);
      installationManager.PreInstallPlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreInstallPlugin);
      installationManager.PreRemovePlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreRemovePlugin);
      installationManager.PreUpdatePlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreUpdatePlugin);
      base.OnClosing(e);
    }

    #region IStatusView Members

    public void ShowProgressIndicator(double percentProgress) {
      if (percentProgress < 0.0 || percentProgress > 1.0) throw new ArgumentException("percentProgress");
      progressBar.Visible = true;
      progressBar.Style = ProgressBarStyle.Continuous;
      int range = progressBar.Maximum - progressBar.Minimum;
      progressBar.Value = (int)(percentProgress * range + progressBar.Minimum);
    }

    public void ShowProgressIndicator() {
      progressBar.Visible = true;
      progressBar.Style = ProgressBarStyle.Marquee;
    }

    public void HideProgressIndicator() {
      progressBar.Visible = false;
    }

    public void ShowMessage(string message) {
      statusLabel.Text = message;
    }

    public void RemoveMessage(string message) {
      statusLabel.Text = string.Empty;
    }

    public void LockUI() {
      Cursor = Cursors.AppStarting;
    }
    public void UnlockUI() {
      Cursor = Cursors.Default;
    }
    public void ShowError(string shortMessage, string description) {
      progressBar.Visible = false;
      okButton.Visible = true;
      statusLabel.Text = shortMessage + Environment.NewLine + description;
      this.Cursor = Cursors.Default;
    }
    #endregion

    private void okButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
