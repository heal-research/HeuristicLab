#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class ClipboardMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    private ToolStripMenuItem menuItem;

    public override string Name {
      get { return "&Clipboard"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&View" }; }
    }
    public override int Position {
      get { return 3200; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      MainFormManager.MainForm.ViewShown += new EventHandler<ViewShownEventArgs>(MainForm_ViewShown);
      MainFormManager.MainForm.ViewHidden += new EventHandler<ViewEventArgs>(MainForm_ViewHidden);

      menuItem = ToolStripItem as ToolStripMenuItem;
      if (menuItem != null) {
        menuItem.CheckOnClick = true;
        menuItem.Checked = Properties.Settings.Default.ShowClipboard;
      }
    }

    private void MainForm_ViewShown(object sender, ViewShownEventArgs e) {
      if ((e.View is Clipboard<IItem>) && (menuItem != null)) {
        menuItem.Checked = true;
        Properties.Settings.Default.ShowClipboard = true;
        Properties.Settings.Default.Save();
      }
    }
    private void MainForm_ViewHidden(object sender, ViewEventArgs e) {
      if ((e.View is Clipboard<IItem>) && (menuItem != null)) {
        menuItem.Checked = false;
        Properties.Settings.Default.ShowClipboard = false;
        Properties.Settings.Default.Save();
      }
    }

    public override void Execute() {
      IView clipboard = null;
      if (MainFormManager.MainForm is OptimizerDockingMainForm) {
        clipboard = ((OptimizerDockingMainForm)MainFormManager.MainForm).Clipboard;
      } else if (MainFormManager.MainForm is OptimizerMultipleDocumentMainForm) {
        clipboard = ((OptimizerMultipleDocumentMainForm)MainFormManager.MainForm).Clipboard;
      } else if (MainFormManager.MainForm is OptimizerSingleDocumentMainForm) {
        clipboard = ((OptimizerSingleDocumentMainForm)MainFormManager.MainForm).Clipboard;
      }

      if (clipboard != null) {
        if (clipboard.IsShown) {
          clipboard.Hide();
        } else {
          clipboard.Show();
        }
      }
    }
  }
}
