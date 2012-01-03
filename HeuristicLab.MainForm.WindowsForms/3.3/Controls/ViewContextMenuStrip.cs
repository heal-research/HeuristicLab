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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public sealed partial class ViewContextMenuStrip : ContextMenuStrip {
    public ViewContextMenuStrip() {
      InitializeComponent();
      this.menuItems = new Dictionary<Type, ToolStripMenuItem>();
      this.ignoredViewTypes = new List<Type>();
    }

    public ViewContextMenuStrip(object item)
      : this() {
      this.Item = item;
    }

    private object item;
    public object Item {
      get { return this.item; }
      set {
        if (this.item != value) {
          this.item = value;
          this.RefreshMenuItems();
        }
      }
    }

    private List<Type> ignoredViewTypes;
    public IEnumerable<Type> IgnoredViewTypes {
      get { return this.ignoredViewTypes; }
      set { this.ignoredViewTypes = new List<Type>(value); RefreshMenuItems(); }
    }

    private Dictionary<Type, ToolStripMenuItem> menuItems;
    public IEnumerable<KeyValuePair<Type, ToolStripMenuItem>> MenuItems {
      get { return this.menuItems; }
    }

    private void RefreshMenuItems() {
      if (InvokeRequired) Invoke((Action)RefreshMenuItems);
      else {
        this.Items.Clear();
        this.menuItems.Clear();

        if (this.item != null) {
          ToolStripMenuItem menuItem;
          IEnumerable<Type> types = MainFormManager.GetViewTypes(item.GetType(), true);
          foreach (Type t in types.Except(IgnoredViewTypes)) {
            menuItem = new ToolStripMenuItem();
            menuItem.Tag = t;
            menuItem.Text = ViewAttribute.GetViewName(t);

            this.menuItems.Add(t, menuItem);
            this.Items.Add(menuItem);
          }
        }
      }
    }
  }
}
