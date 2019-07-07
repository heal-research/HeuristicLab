#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public override string ToolTipText {
      get { return "Copy the shown content into the HeuristicLab Optimizer clipboard"; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.Content != null) && (activeView.Content is IItem) && !activeView.Locked;
    }

    public override void Execute() {
      Clipboard<IItem> clipboard = null;
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;

      if ((activeView != null) && (activeView.Content != null) && (activeView.Content is IItem) && !activeView.Locked) {
        if (MainFormManager.MainForm is OptimizerDockingMainForm) {
          clipboard = ((OptimizerDockingMainForm)MainFormManager.MainForm).Clipboard;
        } else if (MainFormManager.MainForm is OptimizerMultipleDocumentMainForm) {
          clipboard = ((OptimizerMultipleDocumentMainForm)MainFormManager.MainForm).Clipboard;
        } else if (MainFormManager.MainForm is OptimizerSingleDocumentMainForm) {
          clipboard = ((OptimizerSingleDocumentMainForm)MainFormManager.MainForm).Clipboard;
        }

        if (clipboard != null) {
          IItem content = (IItem)activeView.Content;
          clipboard.AddItem((IItem)content.Clone());
        }
      }
    }
  }
}
