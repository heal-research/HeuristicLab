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

    internal SplashScreen() {
      InitializeComponent();
    }

    internal SplashScreen(PluginManager manager, int initialInterval, string initialText)
      : this() {
      this.initialInterval = initialInterval;
      this.manager = manager;

      manager.Action += managerActionEventHandler;

      infoLabel.Text = initialText;
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

    private void SetInfoText(string text) {
      if (InvokeRequired) Invoke((Action<string>)SetInfoText, text);
      else {
        infoLabel.Text = text;
      }
    }

    private void managerActionEventHandler(object sender, PluginInfrastructureEventArgs e) {
      if (InvokeRequired) {
        Invoke((EventHandler<PluginInfrastructureEventArgs>)managerActionEventHandler, sender, e);
      } else {
        ResetFadeTimer();
        string info = e.Action + ": " + e.Entity;
        SetInfoText(info);
        Application.DoEvents(); // force immediate update of splash screen control
      }
    }

    private void ResetFadeTimer() {
      fadeTimer.Stop(); fadeTimer.Start();
    }

    private void fadeTimer_Elapsed(object sender, EventArgs e) {
      FadeOutAndClose();
    }

    private void FadeOutAndClose() {
      fadeTimer.Stop();
      fadeTimer.Interval = FADE_INTERVAL;
      if (this.Opacity > 0) {
        Opacity -= 0.1;
        fadeTimer.Start();
      } else {
        Opacity = 0;
        fadeTimer.Stop();
        fadeTimer.Dispose();
        manager.Action -= managerActionEventHandler; // remove event before calling close
        Close();
      }
    }

    private void closeButton_Click(object sender, EventArgs e) {
      FadeOutAndClose();
    }
  }
}
