#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer {
  internal class SaveToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save"; }
    }
    public override string ToolTipText {
      get { return "Save File (Ctrl + S)"; }
    }
    public override int Position {
      get { return 30; }
    }
    public override Image Image {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Save; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      ToolStripItem.Enabled = (activeView != null) && (activeView.Content != null) &&
                              (activeView.Content is IStorableContent) && !activeView.Locked && activeView.Enabled;
    }

    public override void Execute() {
      FileManager.Save();
    }
  }
}
