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
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace HeuristicLab.MainForm {
  public abstract class ToolStripItemBase : UserInterfaceItemBase, IToolStripItem {
    #region IToolStripItem Members
    public abstract int Position { get; }
    public virtual ToolStripItemDisplayStyle DisplayStyle {
      get { return ToolStripItemDisplayStyle.ImageAndText; }
    }

    public virtual System.Drawing.Image Image {
      get { return null; }
    }

    private ToolStripItem toolStripItem;
    public ToolStripItem ToolStripItem {
      get { return this.toolStripItem; }
      set { this.toolStripItem = value; }
    }

    public virtual string Structure {
      get { return string.Empty; }
    }

    private static readonly char structureSeparator = '/';
    public virtual char StructureSeparator {
      get { return ToolStripMenuItemBase.structureSeparator; }
    }

    public virtual bool ListenActiveViewChanged {
      get { return false; }
    }
    public virtual bool ListenViewChanged {
      get { return false; }
    }

    public virtual void ActiveViewChanged(object sender, EventArgs e) {
    }

    public virtual void ViewChanged(object sender, EventArgs e) {
    }
    #endregion
  }
}
