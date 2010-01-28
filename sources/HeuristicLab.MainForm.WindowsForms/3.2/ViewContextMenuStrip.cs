using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class ViewContextMenuStrip : ContextMenuStrip {
    private object item;
    public ViewContextMenuStrip() {
      InitializeComponent();
    }

    public ViewContextMenuStrip(object item) :this() {
      if (item != null) {
        this.item = item;
        IEnumerable<Type> types = MainFormManager.GetViewTypes(item.GetType());
        if (types != null) {
          foreach (Type t in types) {
            ToolStripMenuItem menuItem = new ToolStripMenuItem() {
              Text = t.Name,
              Tag = t
            };
            this.Items.Add(menuItem);
          }
        }
      }
    }

    private void ViewContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      if (item != null) {
        Type viewType = e.ClickedItem.Tag as Type;
        IView view = MainFormManager.CreateView(viewType, this.item);
        view.Show();
      }
    }

  }
}
