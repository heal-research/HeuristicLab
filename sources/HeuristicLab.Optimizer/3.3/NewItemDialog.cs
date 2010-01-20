using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;

namespace HeuristicLab.Optimizer {
  internal partial class NewItemDialog : Form {
    private bool initialized;
    private List<IItem> items;

    private IItem item;
    public IItem Item {
      get { return item; }
    }

    public NewItemDialog() {
      initialized = false;
      items = new List<IItem>();
      item = null;
      InitializeComponent();
    }

    private void NewItemDialog_Load(object sender, EventArgs e) {
      if (!initialized) {
        SetListViewDisplayStyleCheckBoxes();

        var categories = from t in ApplicationManager.Manager.GetTypes(typeof(IItem))
                         where CreatableAttribute.IsCreatable(t)
                         orderby CreatableAttribute.GetCategory(t), ItemAttribute.GetName(t) ascending
                         group t by CreatableAttribute.GetCategory(t) into c
                         select c;

        itemsListView.SmallImageList = new ImageList();
        foreach (var category in categories) {
          ListViewGroup group = new ListViewGroup(category.Key);
          itemsListView.Groups.Add(group);
          foreach (var creatable in category) {
            IItem i = (IItem)Activator.CreateInstance(creatable);
            items.Add(i);
            ListViewItem item = new ListViewItem(new string[] { i.ItemName, i.ItemDescription}, group);
            itemsListView.SmallImageList.Images.Add(i.ItemImage);
            item.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
            item.Tag = i;
            itemsListView.Items.Add(item);
          }
        }
        initialized = true;
      }
    }

    private void NewItemDialog_Shown(object sender, EventArgs e) {
      item = null;
    }

    private void itemTypesListView_SelectedIndexChanged(object sender, EventArgs e) {
      okButton.Enabled = itemsListView.SelectedItems.Count == 1;
    }

    private void okButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        item = (IItem)((IItem)itemsListView.SelectedItems[0].Tag).Clone();
        DialogResult = DialogResult.OK;
        Close();
      }
    }
    private void itemTypesListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        item = (IItem)((IItem)itemsListView.SelectedItems[0].Tag).Clone();
        DialogResult = DialogResult.OK;
        Close();
      }
    }

    private void showIconsCheckBox_Click(object sender, EventArgs e) {
      itemsListView.View = View.SmallIcon;
      SetListViewDisplayStyleCheckBoxes();
    }
    private void showDetailsCheckBox_Click(object sender, EventArgs e) {
      itemsListView.View = View.Details;
      SetListViewDisplayStyleCheckBoxes();
      for (int i = 0; i < itemsListView.Columns.Count; i++)
        itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
    }
    private void SetListViewDisplayStyleCheckBoxes() {
      showIconsCheckBox.Checked = itemsListView.View == View.SmallIcon;
      showDetailsCheckBox.Checked = itemsListView.View == View.Details;
    }
  }
}
