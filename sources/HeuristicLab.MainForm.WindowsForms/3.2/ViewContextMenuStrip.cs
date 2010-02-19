using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public sealed partial class ViewContextMenuStrip : ContextMenuStrip {
    public ViewContextMenuStrip() {
      InitializeComponent();
      this.menuItems = new Dictionary<Type, ToolStripMenuItem>();
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

    private Dictionary<Type, ToolStripMenuItem> menuItems;
    public IEnumerable<KeyValuePair<Type, ToolStripMenuItem>> MenuItems {
      get { return this.menuItems; }
    }

    private void RefreshMenuItems() {
      this.Items.Clear();
      this.menuItems.Clear();

      if (this.item != null) {
        ToolStripMenuItem menuItem;
        IEnumerable<Type> types = MainFormManager.GetViewTypes(item.GetType());
        foreach (Type t in types) {
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
