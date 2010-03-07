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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer {
  internal class SaveAllToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save All"; }
    }
    public override string ToolTipText {
      get { return "Save All Files"; }
    }
    public override int Position {
      get { return 40; }
    }
    public override Image Image {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.SaveAll; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      var views = from v in MainFormManager.MainForm.Views
                  where v is IContentView
                  where ((v is ItemView) && ((ItemView)v).EnableFileOperations) || (!(v is ItemView))
                  select v;
      ToolStripItem.Enabled = views.FirstOrDefault() != null;
    }

    public override void Execute() {
      FileManager.SaveAll();
    }
  }
}
