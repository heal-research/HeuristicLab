using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.DeploymentService.AdminClient {
  public partial class MultiSelectListView : ListView {
    public MultiSelectListView() {
      InitializeComponent();
    }

    private bool inhibitAutoCheck;
    protected override void OnMouseDown(MouseEventArgs e) {
      inhibitAutoCheck = true;
      base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e) {
      base.OnMouseUp(e);
      inhibitAutoCheck = false;
    }

    // item check is raised for each selected item that was not directly clicked (those should be ignored)
    // and then one the item whose checkbox was clicked
    protected override void OnItemCheck(ItemCheckEventArgs ice) {
      // don't change the checked state of items that were not clicked directly
      if (inhibitAutoCheck) {
        ice.NewValue = ice.CurrentValue;
      }
      // if this item was directly clicked then check if it is selected as well
      var item = Items[ice.Index];
      if (!item.Selected) {
        // when the item was not selected previously 
        // clear any selection 
        List<ListViewItem> selectedItems = new List<ListViewItem>(SelectedItems.OfType<ListViewItem>());
        selectedItems.ForEach(x => x.Selected = false);
        // select only the clicked item
        item.Selected = true;
      }
      base.OnItemCheck(ice);
    }

    private bool suppressItemCheckedEvents;
    public bool SuppressItemCheckedEvents {
      get {
        return suppressItemCheckedEvents;
      }
      set {
        suppressItemCheckedEvents = value;
      }
    }
    protected override void OnItemChecked(ItemCheckedEventArgs e) {
      if (!suppressItemCheckedEvents)
        base.OnItemChecked(e);
    }

    public void CheckItems(IEnumerable<ListViewItem> items) {
      suppressItemCheckedEvents = true;

      foreach (var item in items)
        item.Checked = true;
      suppressItemCheckedEvents = false;
    }

    public void UncheckItems(IEnumerable<ListViewItem> items) {
      suppressItemCheckedEvents = true;

      foreach (var item in items)
        item.Checked = false;
      suppressItemCheckedEvents = false;
    }

  }
}
