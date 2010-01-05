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

namespace HeuristicLab.PluginInfrastructure.Starter {
  public partial class StarterForm : Form {

    private ListViewItem pluginManagerListViewItem;
    private bool abortRequested;
    private PluginManager pluginManager;

    public StarterForm()
      : base() {
      InitializeComponent();

      string pluginPath = Path.GetFullPath(Application.StartupPath);      
      pluginManager = new PluginManager(pluginPath);
      SplashScreen splashScreen = new SplashScreen(pluginManager, 1000, "Loading HeuristicLab...");
      splashScreen.Show();

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

    public StarterForm(string appName)
      : this() {
      var appDesc = (from desc in pluginManager.Applications
                     where desc.Name == appName
                     select desc).Single();
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
          try {
            //Cursor = Cursors.AppStarting;
            //ManagerForm form = new ManagerForm();
            //this.Visible = false;
            //form.ShowDialog(this);
            //// RefreshApplicationsList();
            //this.Visible = true;
          }
          finally {
            Cursor = Cursors.Arrow;
          }
        } else {
          ApplicationDescription app = (ApplicationDescription)applicationsListView.SelectedItems[0].Tag;
          StartApplication(app);
        }
      }
    }

    private void StartApplication(ApplicationDescription app) {
      SplashScreen splashScreen = new SplashScreen(pluginManager, 2000, "Loading " + app.Name);
      splashScreen.Show();
      Thread t = new Thread(delegate() {
        bool stopped = false;
        do {
          try {
            if (!abortRequested)
              pluginManager.Run(app);
            stopped = true;
          }
          catch (Exception ex) {
            stopped = false;
            ThreadPool.QueueUserWorkItem(delegate(object exception) { ShowErrorMessageBox((Exception)exception); }, ex);
            Thread.Sleep(5000); // sleep 5 seconds before autorestart
          }
        } while (!abortRequested && !stopped && app.AutoRestart);
      });
      t.SetApartmentState(ApartmentState.STA); // needed for the AdvancedOptimizationFrontent
      t.Start();
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
      StringBuilder sb = new StringBuilder();
      sb.Append("Sorry, but something went wrong!\n\n" + ex.Message + "\n\n" + ex.StackTrace);

      while (ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append("\n\n-----\n\n" + ex.Message + "\n\n" + ex.StackTrace);
      }
      return sb.ToString();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      abortRequested = true;
    }
  }
}
