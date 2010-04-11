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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class CopyToClipboardMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Copy To Clipboard"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit" }; }
    }
    public override int Position {
      get { return 2100; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.C; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      ItemView activeView = MainFormManager.MainForm.ActiveView as ItemView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.SaveEnabled);
    }

    public override void Execute() {
      ItemView activeView = MainFormManager.MainForm.ActiveView as ItemView;
      if ((activeView != null) && (activeView.SaveEnabled)) {
        Clipboard<IItem> clipboard = ((OptimizerMainForm)MainFormManager.MainForm).Clipboard;
        clipboard.AddItem((IItem)activeView.Content.Clone());
      }
    }
  }
}
