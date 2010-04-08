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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using HeuristicLab.PluginInfrastructure;
using System.Threading;
using HeuristicLab.PluginInfrastructure.Manager;
using System.IO;
using HeuristicLab.PluginInfrastructure.Advanced;

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

      string pluginPath = Path.GetFullPath(Application.StartupPath);
      pluginManager = new PluginManager(pluginPath);
      splashScreen = new SplashScreen(pluginManager, 1000);
      splashScreen.Show("Loading HeuristicLab...");

      pluginManager.DiscoverAndCheckPlugins();

      applicationsListView.Items.Clear();

      pluginManagerListViewItem = new ListViewItem("Plugin Manager", 0);
      pluginManagerListViewItem.Group = applicationsListView.Groups["Plugin Management"];
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, "-"));
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
              "Please stop all HeuristicLab applications and try again.");
          } else {
            try {
              Cursor = Cursors.AppStarting;
              InstallationManagerForm form = new InstallationManagerForm();
              this.Visible = false;
              form.ShowDialog(this);
              // RefreshApplicationsList();
              this.Visible = true;
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

    private void StartApplication(ApplicationDescription app) {
      //new SplashScreen(pluginManager, 2000, );
      splashScreen.Show("Loading " + app.Name);
      Thread t = new Thread(delegate() {
        bool stopped = false;
        do {
          try {
            if (!abortRequested) {
              SetCursor(Cursors.AppStarting);
              pluginManager.Run(app);
            }
            stopped = true;
          }
          catch (Exception ex) {
            stopped = false;
            ThreadPool.QueueUserWorkItem(delegate(object exception) { ShowErrorMessageBox((Exception)exception); }, ex);
            Thread.Sleep(5000); // sleep 5 seconds before autorestart
          }
          finally {
            SetCursor(Cursors.Default);
          }
        } while (!abortRequested && !stopped && app.AutoRestart);
      });
      t.SetApartmentState(ApartmentState.STA); // needed for the AdvancedOptimizationFrontent
      t.Start();
    }

    private void SetCursor(Cursor cursor) {
      if (InvokeRequired) Invoke((Action<Cursor>)SetCursor, cursor);
      else {
        Cursor = cursor;
      }
    }

    private void applicationsListBox_SelectedIndexChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
      if (e.IsSelected) {
        startButton.Enabled = true;
      } else {
        startButton.Enabled = false;
      }
    }

    private void largeIconsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.LargeIcon;
    }

    private void listButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.List;
    }

    private void detailsButton_Click(object sender, EventArgs e) {
      applicationsListView.View = View.Details;
    }

    private void ShowErrorMessageBox(Exception ex) {
      MessageBoxOptions options = RightToLeft == RightToLeft.Yes ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : MessageBoxOptions.DefaultDesktopOnly;
      MessageBox.Show(null,
         BuildErrorMessage(ex),
         "Error - " + ex.GetType().Name,
         MessageBoxButtons.OK,
         MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
    }
    private static string BuildErrorMessage(Exception ex) {
      string nl = Environment.NewLine;
      StringBuilder sb = new StringBuilder();
      sb.Append(ex.Message + nl + ex.StackTrace);

      while (ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append(nl + "-----" + nl + ex.Message + nl + ex.StackTrace);
      }
      return sb.ToString();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      abortRequested = true;
    }
  }
}
