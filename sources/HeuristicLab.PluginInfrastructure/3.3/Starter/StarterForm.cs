#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Advanced;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Starter {
  /// <summary>
  /// The starter form is responsible for initializing the plugin infrastructure
  /// and shows a list of installed applications.
  /// </summary>
  public partial class StarterForm : Form {

    private ListViewItem pluginManagerListViewItem;
    private bool abortRequested;
    private PluginManager pluginManager;
    private SplashScreen splashScreen;

    /// <summary>
    /// Initializes an instance of the starter form.
    /// The starter form shows a splashscreen and initializes the plugin infrastructure.
    /// </summary>
    public StarterForm()
      : base() {
      InitializeComponent();
      largeImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.HeuristicLab.ToBitmap());
      smallImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.HeuristicLab.ToBitmap());
      FileVersionInfo pluginInfrastructureVersion = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
      Text = "HeuristicLab " + pluginInfrastructureVersion.FileVersion;

      string pluginPath = Path.GetFullPath(Application.StartupPath);
      pluginManager = new PluginManager(pluginPath);
      splashScreen = new SplashScreen(pluginManager, 1000);
      splashScreen.Show(this, "Loading HeuristicLab...");

      pluginManager.DiscoverAndCheckPlugins();
      UpdateApplicationsList();
    }

    /// <summary>
    /// Creates a new StarterForm and tries to start application with <paramref name="appName"/> immediately.
    /// </summary>
    /// <param name="appName">Name of the application</param>
    public StarterForm(string appName)
      : this() {
      var appDesc = (from desc in pluginManager.Applications
                     where desc.Name == appName
                     select desc).SingleOrDefault();
      if (appDesc != null) {
        StartApplication(appDesc);
      } else {
        MessageBox.Show("Cannot start application " + appName + ".",
                        "HeuristicLab",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
      }
    }

    private void applicationsListView_ItemActivate(object sender, EventArgs e) {
      if (applicationsListView.SelectedItems.Count > 0) {
        ListViewItem selected = applicationsListView.SelectedItems[0];
        if (selected == pluginManagerListViewItem) {
          if (pluginManager.Plugins.Any(x => x.PluginState == PluginState.Loaded)) {
            MessageBox.Show("Installation Manager cannot be started while another HeuristicLab application is active." + Environment.NewLine +
              "Please stop all active HeuristicLab applications and try again.", "Plugin Manager",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
          } else {
            try {
              Cursor = Cursors.AppStarting;
              using (InstallationManagerForm form = new InstallationManagerForm(pluginManager)) {
                form.ShowDialog(this);
              }
              UpdateApplicationsList();
            }
            finally {
              Cursor = Cursors.Arrow;
            }
          }
        } else {
          ApplicationDescription app = (ApplicationDescription)applicationsListView.SelectedItems[0].Tag;
          StartApplication(app);
        }
      }
    }

    private void UpdateApplicationsList() {
      applicationsListView.Items.Clear();
      FileVersionInfo pluginInfrastructureVersion = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
      pluginManagerListViewItem = new ListViewItem("Plugin Manager", 0);
      pluginManagerListViewItem.Group = applicationsListView.Groups["Plugin Management"];
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, pluginInfrastructureVersion.FileVersion));
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, "Install, upgrade or delete plugins"));
      pluginManagerListViewItem.ToolTipText = "Install, upgrade or delete plugins";

      applicationsListView.Items.Add(pluginManagerListViewItem);

      foreach (ApplicationDescription info in pluginManager.Applications) {
        ListViewItem item = new ListViewItem(info.Name, 0);
        item.Tag = info;
        item.Group = applicationsListView.Groups["Applications"];
        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.Version.ToString()));
        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.Description));
        item.ToolTipText = info.Description;
        applicationsListView.Items.Add(item);
      }
      foreach (ColumnHeader column in applicationsListView.Columns) {
        if (applicationsListView.Items.Count > 0)
          column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        else column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
      }
    }

    private void StartApplication(ApplicationDescription app) {
      splashScreen.Show("Loading " + app.Name);
      Thread t = new Thread(delegate() {
        bool stopped = false;
        do {
          try {
            if (!abortRequested) {
              pluginManager.Run(app);
            }
            stopped = true;
          }
          catch (Exception ex) {
            stopped = false;
            ThreadPool.QueueUserWorkItem(delegate(object exception) { ErrorHandling.ShowErrorDialog(this, (Exception)exception); }, ex);
            Thread.Sleep(5000); // sleep 5 seconds before autorestart
          }
        } while (!abortRequested && !stopped && app.AutoRestart);
      });
      t.SetApartmentState(ApartmentState.STA); // needed for the AdvancedOptimizationFrontent
      t.Start();
    }

    private void applicationsListView_SelectedIndexChanged(object sender, EventArgs e) {
      startButton.Enabled = applicationsListView.SelectedItems.Count > 0;
    }

    private void largeIconsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.LargeIcon;
    }

    private void detailsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.Details;
      foreach (ColumnHeader column in applicationsListView.Columns) {
        if (applicationsListView.Items.Count > 0)
          column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        else column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      abortRequested = true;
    }

    private void aboutButton_Click(object sender, EventArgs e) {
      List<IPluginDescription> plugins = new List<IPluginDescription>(pluginManager.Plugins.OfType<IPluginDescription>());
      using (var dialog = new AboutDialog(plugins)) {
        dialog.ShowDialog();
      }
    }
  }
}
