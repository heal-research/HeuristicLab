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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Starter {
  internal partial class SplashScreen : Form {
    private const int FADE_INTERVAL = 50;
    private Timer fadeTimer;
    private int initialInterval;
    private PluginManager pluginManager;

    internal SplashScreen() {
      InitializeComponent();
    }

    internal SplashScreen(PluginManager manager, int initialInterval)
      : this() {
      this.initialInterval = initialInterval;
      this.pluginManager = manager;

      RegisterPluginManagerEventHandlers();

      versionLabel.Text = "Version " + AssemblyHelpers.GetFileVersion(GetType().Assembly);
      infoLabel.Text = "";

      var attr = (AssemblyCopyrightAttribute)this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).Single();
      copyrightLabel.Text = "Copyright " + attr.Copyright;

      fadeTimer = new Timer();
      fadeTimer.Tick += fadeTimer_Elapsed;
      fadeTimer.Interval = initialInterval;
    }

    #region events
    private void RegisterPluginManagerEventHandlers() {
      pluginManager.ApplicationStarted += new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarted);
      pluginManager.ApplicationStarting += new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarting);
      pluginManager.Initializing += new EventHandler<PluginInfrastructureEventArgs>(manager_Initializing);
      pluginManager.Initialized += new EventHandler<PluginInfrastructureEventArgs>(manager_Initialized);
      pluginManager.PluginLoaded += new EventHandler<PluginInfrastructureEventArgs>(manager_PluginLoaded);
      pluginManager.PluginUnloaded += new EventHandler<PluginInfrastructureEventArgs>(manager_PluginUnloaded);
    }

    private void DeregisterPluginManagerEventHandlers() {
      pluginManager.ApplicationStarted -= new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarted);
      pluginManager.ApplicationStarting -= new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarting);
      pluginManager.Initializing -= new EventHandler<PluginInfrastructureEventArgs>(manager_Initializing);
      pluginManager.Initialized -= new EventHandler<PluginInfrastructureEventArgs>(manager_Initialized);
      pluginManager.PluginLoaded -= new EventHandler<PluginInfrastructureEventArgs>(manager_PluginLoaded);
      pluginManager.PluginUnloaded -= new EventHandler<PluginInfrastructureEventArgs>(manager_PluginUnloaded);
    }

    private void manager_PluginUnloaded(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Unloaded " + e.Entity);
    }

    private void manager_PluginLoaded(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Loaded " + e.Entity);
    }

    private void manager_Initialized(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Initialized");
    }

    private void manager_Initializing(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Initializing");
    }

    private void manager_ApplicationStarting(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Starting " + e.Entity);
    }

    private void manager_ApplicationStarted(object sender, PluginInfrastructureEventArgs e) {
      SafeUpdateMessage("Started " + e.Entity);
    }
    // called from event handlers
    private void SafeUpdateMessage(string msg) {
      try {
        Invoke((Action<string>)UpdateMessage, msg);
      }
      catch (ObjectDisposedException) { }
    }

    // each tick of the timer reduce opacity and restart timer
    private void fadeTimer_Elapsed(object sender, EventArgs e) {
      // only called from local timer: no need to invoke here
      FadeOut();
    }
    #endregion

    public void Show(string initialText) {
      if (InvokeRequired) Invoke((Action<string>)Show, initialText);
      else {
        Opacity = 1;
        infoLabel.Text = initialText;
        ResetFadeTimer();
        Show();
      }
    }

    public void Show(IWin32Window owner, string initialText) {
      if (InvokeRequired) Invoke((Action<IWin32Window, string>)Show, owner, initialText);
      else {
        Opacity = 1;
        infoLabel.Text = initialText;
        ResetFadeTimer();
        Show(owner);
      }
    }

    private void ResetFadeTimer() {
      // wait initialInterval again for the first tick
      fadeTimer.Stop();
      fadeTimer.Interval = initialInterval;
      fadeTimer.Start();
    }

    private void UpdateMessage(string msg) {
      ResetFadeTimer();
      infoLabel.Text = msg;
      Application.DoEvents(); // force immediate update of splash screen control
    }

    // reduces opacity of the splashscreen one step and restarts the fade-timer
    private void FadeOut() {
      fadeTimer.Stop();
      fadeTimer.Interval = FADE_INTERVAL;
      if (this.Opacity > 0) {
        Opacity -= 0.1;
        fadeTimer.Start();
      } else {
        Opacity = 0;
        fadeTimer.Stop();
        Hide();
      }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
      // deregister events when form is closing
      DeregisterPluginManagerEventHandlers();
      base.OnClosing(e);
    }
  }
}
