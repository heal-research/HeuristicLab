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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab {
  public partial class SplashScreen : Form {
    private const int FADE_INTERVAL = 50;
    private System.Timers.Timer waitTimer;
    private System.Timers.Timer fadeTimer;
    private object bigLock = new object();

    private int displayTime = 1000;
    public int DisplayTime {
      get { return (displayTime); }
      set {
        if(value > 0) {
          displayTime = value;
        }
      }
    }

    public SplashScreen() {
      InitializeComponent();

      Assembly assembly = this.GetType().Assembly;
      object[] attributes = assembly.GetCustomAttributes(false);
      string user, company;

      titleLabel.Text = Application.ProductName;
      versionLabel.Text = "Version " + Application.ProductVersion;
      infoLabel.Text = "";

      foreach(object obj in attributes) {
        if(obj is AssemblyCopyrightAttribute) {
          copyrightLabel.Text = "Copyright " + ((AssemblyCopyrightAttribute)obj).Copyright;
        }
      }

      try {
        user = HeuristicLab.Properties.Settings.Default.User;
        company = HeuristicLab.Properties.Settings.Default.Organization;

        if((user == null) || (user.Equals(""))) {
          userNameLabel.Text = "-";
        } else {
          userNameLabel.Text = user;
        }

        if((company == null) || (company.Equals(""))) {
          companyLabel.Text = "-";
        } else {
          companyLabel.Text = company;
        }
      } catch(Exception) {
        userNameLabel.Text = "-";
        companyLabel.Text = "-";
      }
    }

    public SplashScreen(int displayTime, string initialText)
      : this() {
      DisplayTime = displayTime;
      infoLabel.Text = initialText;
    }

    private void SetInfoText(string text) {
      this.Invoke((MethodInvoker)delegate() { infoLabel.Text = text; });
    }

    public void Manager_Action(object sender, PluginManagerActionEventArgs e) {
      lock(bigLock) {
        if(!this.Disposing && !this.IsDisposed) {
          if(waitTimer == null) {
            waitTimer = new System.Timers.Timer();
            waitTimer.SynchronizingObject = this;
            waitTimer.Elapsed += new System.Timers.ElapsedEventHandler(waitTimer_Elapsed);
          }
          waitTimer.Stop();
          waitTimer.Interval = DisplayTime;
          waitTimer.AutoReset = true;
          string info;
          if(e.Action == PluginManagerAction.Initializing) info = "Initializing ...";
          else if(e.Action == PluginManagerAction.InitializingPlugin) info = "Initializing Plugin " + e.Id + " ...";
          else if(e.Action == PluginManagerAction.InitializedPlugin) info = "Initializing Plugin " + e.Id + " ... Initialized";
          else if(e.Action == PluginManagerAction.Initialized) info = "Initialization Completed";
          else {
            if(e.Id != null) info = e.Action.ToString() + "   (" + e.Id + ")";
            else info = e.Action.ToString();
          }
          SetInfoText(info);
          Application.DoEvents();
          waitTimer.Start();
        }
      }
    }

    private void waitTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      lock(bigLock) {
        if(!this.Disposing && !this.IsDisposed) {
          if(fadeTimer == null) {
            fadeTimer = new System.Timers.Timer();
            fadeTimer.SynchronizingObject = this;
            fadeTimer.Elapsed += new System.Timers.ElapsedEventHandler(fadeTimer_Elapsed);
            fadeTimer.Interval = FADE_INTERVAL;
            fadeTimer.AutoReset = true;
            fadeTimer.Start();
          }
        }
      }
    }

    private void fadeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      lock(bigLock) {
        if(!this.Disposing && !this.IsDisposed) {
          fadeTimer.Stop();
          if(InvokeRequired) {
            Invoke((MethodInvoker)UpdateOpacity);
          } else {
            UpdateOpacity();
          }
        }
      }
    }

    private void UpdateOpacity() {
      if(Opacity > 0.9) {
        Opacity = 0.9;
        fadeTimer.Start();
      } else if(this.Opacity > 0) {
        Opacity -= 0.1;
        fadeTimer.Start();
      } else {
        Opacity = 0;
        Close();
      }
    }

    private void SplashScreen_FormClosing(object sender, FormClosingEventArgs e) {
      PluginManager.Manager.Action -= new PluginManagerActionEventHandler(this.Manager_Action);
    }

    private void closeButton_Click(object sender, EventArgs e) {
      lock(bigLock) {
        if(fadeTimer != null) fadeTimer.Stop();
        if(waitTimer != null) waitTimer.Stop();
        Close();
      }
    }
  }
}
