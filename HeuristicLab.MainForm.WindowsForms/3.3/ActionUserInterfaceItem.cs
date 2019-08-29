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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public abstract class ActionUserInterfaceItem : HeuristicLab.MainForm.ActionUserInterfaceItem {
    private ToolStripItem toolStripItem;
    public ToolStripItem ToolStripItem {
      get { return this.toolStripItem; }
      internal set {
        if (this.toolStripItem != value) {
          bool firstTimeSet = this.toolStripItem == null;
          this.toolStripItem = value;
          if (firstTimeSet)
            this.OnToolStripItemSet(EventArgs.Empty);
        }
      }
    }

    public virtual ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return ToolStripItemDisplayStyle.ImageAndText; }
    }

    protected virtual void OnToolStripItemSet(EventArgs e) {
    }
  }
}
