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
