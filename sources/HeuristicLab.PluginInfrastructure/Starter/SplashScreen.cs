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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Starter {
  internal partial class SplashScreen : Form {
    private const int FADE_INTERVAL = 50;
    private Timer fadeTimer;
    private int initialInterval;
    private PluginManager manager;
    private bool fadeOutForced;
    internal SplashScreen() {
      InitializeComponent();
    }

    internal SplashScreen(PluginManager manager, int initialInterval)
      : this() {
      this.initialInterval = initialInterval;
      this.manager = manager;

      manager.ApplicationStarted += new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarted);
      manager.ApplicationStarting += new EventHandler<PluginInfrastructureEventArgs>(manager_ApplicationStarting);
      manager.Initializing += new EventHandler<PluginInfrastructureEventArgs>(manager_Initializing);
      manager.Initialized += new EventHandler<PluginInfrastructureEventArgs>(manager_Initialized);
      manager.PluginLoaded += new EventHandler<PluginInfrastructureEventArgs>(manager_PluginLoaded);
      manager.PluginUnloaded += new EventHandler<PluginInfrastructureEventArgs>(manager_PluginUnloaded);

      titleLabel.Text = Application.ProductName;
      versionLabel.Text = "Version " + Application.ProductVersion;
      infoLabel.Text = "";

      var attr = (AssemblyCopyrightAttribute)this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).Single();
      copyrightLabel.Text = "Copyright " + attr.Copyright;

      string user = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.User;
      string company = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.Organization;

      userNameLabel.Text = string.IsNullOrEmpty(user) ? "-" : user;
      companyLabel.Text = string.IsNullOrEmpty(company) ? "-" : company;

      fadeTimer = new Timer();
      fadeTimer.Tick += fadeTimer_Elapsed;
      fadeTimer.Interval = initialInterval;
    }

    void manager_PluginUnloaded(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Unloaded " + e.Entity);
    }

    void manager_PluginLoaded(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Loaded " + e.Entity);
    }

    void manager_Initialized(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Initialized");
    }

    void manager_Initializing(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Initializing");
    }

    void manager_ApplicationStarting(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Starting " + e.Entity);
    }

    void manager_ApplicationStarted(object sender, PluginInfrastructureEventArgs e) {
      UpdateMessage("Started " + e.Entity);
    }

    public void Show(string initialText) {
      if (InvokeRequired) Invoke((Action<string>)Show, initialText);
      else {
        Opacity = 1;
        infoLabel.Text = initialText;
        fadeOutForced = false;
        ResetFadeTimer();
        Show();
      }
    }

    private void ResetFadeTimer() {
      // wait initialInterval again for the first tick
      fadeTimer.Stop();
      fadeTimer.Interval = initialInterval;
      fadeTimer.Start();
    }


    private void SetInfoText(string text) {
      if (InvokeRequired) Invoke((Action<string>)SetInfoText, text);
      else {
        infoLabel.Text = text;
      }
    }

    private void UpdateMessage(string msg) {
      if (InvokeRequired) {
        Invoke((Action<string>)UpdateMessage, msg);
      } else {
        // when the user forced a fade-out (by closing the splashscreen)
        // don't reset the fadeTimer
        if (!fadeOutForced) {
          ResetFadeTimer();
        }
        SetInfoText(msg);
        Application.DoEvents(); // force immediate update of splash screen control
      }
    }

    // each tick of the timer reduce opacity and restart timer
    private void fadeTimer_Elapsed(object sender, EventArgs e) {
      FadeOut();
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

    // force fade out
    private void closeButton_Click(object sender, EventArgs e) {
      fadeOutForced = true;
      FadeOut();
    }
  }
}
