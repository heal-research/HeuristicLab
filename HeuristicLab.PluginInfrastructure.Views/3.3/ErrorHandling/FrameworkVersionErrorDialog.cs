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
using System.Reflection;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure {
  public partial class FrameworkVersionErrorDialog : Form {
    private const string NETVersionPath = @"Software\Microsoft\NET Framework Setup\NDP\v4\Full";
    private const string NETVersion = "Version";

    public static bool NET4_5Installed {
      get {
        try {
          var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(NETVersionPath);
          if (registryKey != null) {
            var versionKey = registryKey.GetValue(NETVersion);
            if (versionKey != null) {
              Version version = new Version(versionKey.ToString());
              return version.Major >= 4 && version.Minor >= 5;
            }
          }
        }
        catch (System.Security.SecurityException) {
          return false;
        }
        return false;
      }
    }

    public static bool MonoInstalled {
      get { return Type.GetType("Mono.Runtime") != null; }
    }

    public static bool MonoCorrectVersionInstalled {
      get {
        var monoVersion = MonoVersion;
        var minRequiredVersion = new Version(3, 6, 0);

        //we need at least mono version 3.6.0
        if (monoVersion != null && monoVersion >= minRequiredVersion) {
          return true;
        } else {
          return false;
        }
      }
    }

    public static Version MonoVersion {
      get {
        Type type = Type.GetType("Mono.Runtime");
        if (type != null) {
          MethodInfo dispalayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
          if (dispalayName != null) {
            string versionString = dispalayName.Invoke(null, null) as string;
            if (versionString != null) {
              // the version string looks something like: 2.11.4 (master/99d5e54 Thu Sep  6 15:55:44 CEST 2012)
              var subVerStrings = versionString.Split(' ');
              if (subVerStrings.Length > 0) {
                try {
                  return Version.Parse(subVerStrings[0]);
                }
                catch { }
              }
            }
          }
        }
        return null;
      }
    }

    public FrameworkVersionErrorDialog() {
      InitializeComponent();
    }

    private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=30653");
        linkLabel.LinkVisited = true;
      }
      catch (Exception) { }
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    private void linkLabelMono_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("http://www.mono-project.org");
        linkLabelMono.LinkVisited = true;
      }
      catch (Exception) { }
    }
  }
}