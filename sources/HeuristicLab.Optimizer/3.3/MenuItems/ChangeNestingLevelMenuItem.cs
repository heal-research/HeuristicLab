#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using Microsoft.Win32;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class ChangeNestingLevelMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {

    #region Creators Update Nesting
    const string VersionSubKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
    private const int VersionWin10CreatorsUpdate = 460798;
    private const int RecommendedMaxNestingLevel = 25;

    static ChangeNestingLevelMenuItem() {
      var settings = HeuristicLab.Core.Views.Properties.Settings.Default;
      try {
        // detect installed .net/OS version https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx#net_d
        using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(VersionSubKey)) {
          if (ndpKey == null || ndpKey.GetValue("Release") == null) return;
          int version = (int)ndpKey.GetValue("Release");
          if (version == VersionWin10CreatorsUpdate && settings.MaximumNestedControls > RecommendedMaxNestingLevel) {
            var message = string.Format("A high nesting level of controls can cause Windows 10 Creators Update to crash with a blue screen. "
                                          + "Do you want to set the maximum nesting level from {0} to {1} to minimize the risk of a crash?",
                settings.MaximumNestedControls, RecommendedMaxNestingLevel);
            if (MessageBox.Show(message, "Reduce Maximum Nesting Level?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
              settings.MaximumNestedControls = 25;
              settings.Save();
            }
          }
        }
      } catch (PlatformNotSupportedException) {
        // thrown on mono
      }
    }
    #endregion

    public override string Name {
      get { return "Change &Nesting Level..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new[] { "&View" }; }
    }
    public override int Position {
      get { return 4000; }
    }
    public override string ToolTipText {
      get { return "Change the maximum nesting level of controls."; }
    }
    public override void Execute() {
      using (var dialog = new ChangeNestingLevelDialog()) {
        var mainForm = MainFormManager.MainForm as IWin32Window;
        if (mainForm != null)
          dialog.ShowDialog(mainForm);
        else
          dialog.ShowDialog();
      }
    }
  }
}
