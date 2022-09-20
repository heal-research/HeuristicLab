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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Advanced;
using HeuristicLab.PluginInfrastructure.Manager;
using HeuristicLab.PluginInfrastructure.Properties;

namespace HeuristicLab.PluginInfrastructure.Starter {
  /// <summary>
  /// The starter form is responsible for initializing the plugin infrastructure
  /// and shows a list of installed applications.
  /// </summary>
  public partial class StarterForm : Form {
    private const string pluginManagerItemName = "Plugin Manager";
    private const string updatePluginsItemName = "Updates Available";
    private const string optimizerItemName = "Optimizer";

    private readonly ICommandLineArgument[] arguments;

    private ListViewItem pluginManagerListViewItem;
    private bool abortRequested;
    private PluginManager pluginManager;
    private SplashScreen splashScreen;
    private bool updatesAvailable = false;

    /// <summary>
    /// Initializes an instance of the starter form.
    /// The starter form shows a splashscreen and initializes the plugin infrastructure.
    /// </summary>
    public StarterForm()
      : base() {
      InitializeComponent();
      largeImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.HeuristicLab.ToBitmap());
      largeImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.UpdateAvailable.ToBitmap());
      smallImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.HeuristicLab.ToBitmap());
      smallImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.UpdateAvailable.ToBitmap());
      Text = "HeuristicLab " + AssemblyHelpers.GetFileVersion(GetType().Assembly);

      string pluginPath = Path.GetFullPath(Application.StartupPath);
      pluginManager = new PluginManager(pluginPath);
      splashScreen = new SplashScreen(pluginManager, 1000);
      splashScreen.VisibleChanged += new EventHandler(splashScreen_VisibleChanged);
      splashScreen.Show(this, "Loading HeuristicLab...");

      if (CheckSavedStarterFormSettings()) {
        Location = Settings.Default.StarterFormLocation;
        Size = Settings.Default.StarterFormSize;
        WindowState = Settings.Default.StarterFormWindowState;
      }

      pluginManager.DiscoverAndCheckPlugins();
      UpdateApplicationsList();

      CheckUpdatesAvailableAsync();
    }

    /// <summary>
    /// Creates a new StarterForm and passes the arguments in <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The arguments that should be processed</param>
    public StarterForm(string[] args)
      : this() {
      arguments = CommandLineArgumentHandling.GetArguments(args);
    }

    protected override void SetVisibleCore(bool value) {
      value &= !(arguments.OfType<HideStarterArgument>().Any() || arguments.OfType<OpenArgument>().Any());
      if (!value) HandleArguments();
      base.SetVisibleCore(value);
    }

    #region Events
    private void StarterForm_Load(object sender, EventArgs e) {
      HandleArguments();
    }

    private void StarterForm_FormClosing(object sender, FormClosingEventArgs e) {
      splashScreen.Close();
      abortRequested = true;

      if (WindowState != FormWindowState.Minimized)
        Settings.Default.StarterFormWindowState = WindowState;
      if (WindowState != FormWindowState.Normal) {
        Settings.Default.StarterFormLocation = RestoreBounds.Location;
        Settings.Default.StarterFormSize = RestoreBounds.Size;
      } else if (WindowState == FormWindowState.Normal) {
        Settings.Default.StarterFormLocation = Location;
        Settings.Default.StarterFormSize = Size;
      }

      Settings.Default.Save();
    }

    private void applicationsListView_SelectedIndexChanged(object sender, EventArgs e) {
      startButton.Enabled = applicationsListView.SelectedItems.Count > 0;
    }

    private void applicationsListView_ItemActivate(object sender, EventArgs e) {
      if (applicationsListView.SelectedItems.Count > 0) {
        ListViewItem selected = applicationsListView.SelectedItems[0];
        if (selected.Text == pluginManagerItemName) {
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
        } else if (selected.Text == updatePluginsItemName) {
          if (pluginManager.Plugins.Any(x => x.PluginState == PluginState.Loaded)) {
            MessageBox.Show("Updating is not possible while another HeuristicLab application is active." + Environment.NewLine +
              "Please stop all active HeuristicLab applications and try again.", "Update plugins",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
          } else {
            try {
              Cursor = Cursors.AppStarting;
              using (PluginUpdaterForm form = new PluginUpdaterForm(pluginManager)) {
                form.ShowDialog(this);
              }
              updatesAvailable = false;
              CheckUpdatesAvailableAsync();
              UpdateApplicationsList();
            }
            finally {
              Cursor = Cursors.Arrow;
            }
          }
        } else {
          ApplicationDescription app = (ApplicationDescription)applicationsListView.SelectedItems[0].Tag;
          StartApplication(app, arguments);
        }
      }
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

    private void aboutButton_Click(object sender, EventArgs e) {
      List<IPluginDescription> plugins = new List<IPluginDescription>(pluginManager.Plugins.OfType<IPluginDescription>());
      using (var dialog = new AboutDialog(plugins)) {
        dialog.ShowDialog();
      }
    }

    private void splashScreen_VisibleChanged(object sender, EventArgs e) {
      // close hidden starter form
      if (!splashScreen.Visible && arguments != null &&
           (arguments.OfType<HideStarterArgument>().Any() || arguments.OfType<OpenArgument>().Any()))
        Close();
    }
    #endregion

    #region Helpers
    private bool CheckSavedStarterFormSettings() {
      var formArea = new Rectangle(Settings.Default.StarterFormLocation, Settings.Default.StarterFormSize);
      var screenArea = Screen.FromRectangle(formArea).WorkingArea;
      var overlappingArea = Rectangle.Intersect(formArea, screenArea);
      bool offLimits = overlappingArea.IsEmpty || overlappingArea.Width * overlappingArea.Height < formArea.Width * formArea.Height * 0.25;
      return !formArea.IsEmpty && !offLimits;
    }

    private void UpdateApplicationsList() {
      if (InvokeRequired) Invoke((Action)UpdateApplicationsList);
      else {
        applicationsListView.Items.Clear();
        AddPluginManagerItem();
        AddUpdatePluginsItem();

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
    }

    private void AddPluginManagerItem() {
      pluginManagerListViewItem = new ListViewItem(pluginManagerItemName, 0);
      pluginManagerListViewItem.Group = applicationsListView.Groups["Plugin Management"];
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, AssemblyHelpers.GetFileVersion(GetType().Assembly)));
      pluginManagerListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(pluginManagerListViewItem, "Install, upgrade or delete plugins"));
      pluginManagerListViewItem.ToolTipText = "Install, upgrade or delete plugins";

      applicationsListView.Items.Add(pluginManagerListViewItem);
    }

    private void AddUpdatePluginsItem() {
      if (updatesAvailable) {
        var updateListViewItem = new ListViewItem(updatePluginsItemName, 1);
        updateListViewItem.Group = applicationsListView.Groups["Plugin Management"];
        updateListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(updateListViewItem, ""));
        updateListViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(updateListViewItem, "Download and install updates"));
        updateListViewItem.ToolTipText = "Download and install updates";

        applicationsListView.Items.Add(updateListViewItem);
      }
    }

    private void CheckUpdatesAvailableAsync() {
      string pluginPath = Path.GetFullPath(Application.StartupPath);
      var task = Task.Factory.StartNew<bool>(() => {
        var installationManager = new InstallationManager(pluginPath);
        IEnumerable<IPluginDescription> installedPlugins = pluginManager.Plugins.OfType<IPluginDescription>();
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
        return pluginsToUpdate.Count() > 0;
      });
      task.ContinueWith(t => {
        try {
          t.Wait();
          updatesAvailable = t.Result;
          UpdateApplicationsList();
        }
        catch (AggregateException ae) {
          ae.Handle(ex => {
            if (ex is InstallationManagerException) {
              // this is expected when no internet connection is available => do nothing 
              return true;
            } else {
              return false;
            }
          });
        }
      });
    }

    private void HandleArguments() {
      try {
        if (arguments.OfType<OpenArgument>().Any() && !arguments.OfType<StartArgument>().Any()) {
          InitiateApplicationStart(optimizerItemName);
        }
        foreach (var argument in arguments) {
          if (argument is StartArgument) {
            var arg = (StartArgument)argument;
            InitiateApplicationStart(arg.Value);
          }
        }
      }
      catch (AggregateException ex) {
        ErrorHandling.ShowErrorDialog(this, "One or more errors occurred while initializing the application.", ex);
      }
    }

    private void InitiateApplicationStart(string appName) {
      var appDesc = (from desc in pluginManager.Applications
                     where desc.Name.Equals(appName)
                     select desc).SingleOrDefault();
      if (appDesc != null) {
        StartApplication(appDesc, arguments);
      } else {
        MessageBox.Show("Cannot start application " + appName + ".",
                        "HeuristicLab",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
      }
    }

    private void StartApplication(ApplicationDescription app, ICommandLineArgument[] args) {
      splashScreen.Show("Loading " + app.Name);
      Thread t = new Thread(delegate() {
        bool stopped = false;
        do {
          try {
            if (!abortRequested) {
              pluginManager.Run(app, args);
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
    #endregion
  }
}
