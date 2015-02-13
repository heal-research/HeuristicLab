#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  internal partial class NewItemDialog : Form {
    private bool initialized;

    private IItem item;
    public IItem Item {
      get { return item; }
    }

    public NewItemDialog() {
      initialized = false;
      item = null;
      InitializeComponent();
    }

    private void NewItemDialog_Load(object sender, EventArgs e) {
      if (!initialized) {
        var categories = from t in ApplicationManager.Manager.GetTypes(typeof(IItem))
                         where CreatableAttribute.IsCreatable(t)
                         orderby CreatableAttribute.GetCategory(t), ItemAttribute.GetName(t), ItemAttribute.GetVersion(t) ascending
                         group t by CreatableAttribute.GetCategory(t) into c
                         select c;

        itemsListView.SmallImageList = new ImageList();
        itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Class);  // default icon
        foreach (var category in categories) {
          ListViewGroup group = new ListViewGroup(category.Key);
          itemsListView.Groups.Add(group);
          foreach (var creatable in category) {
            string name = ItemAttribute.GetName(creatable);
            string version = ItemAttribute.GetVersion(creatable).ToString();
            string description = ItemAttribute.GetDescription(creatable);
            ListViewItem item = new ListViewItem(new string[] { name, version, description }, group);
            item.ImageIndex = 0;
            Image image = ItemAttribute.GetImage(creatable);
            if (image != null) {
              itemsListView.SmallImageList.Images.Add(image);
              item.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
            }
            item.Tag = creatable;
            itemsListView.Items.Add(item);
          }
        }
        for (int i = 0; i < itemsListView.Columns.Count; i++)
          itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
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
        item = (IItem)Activator.CreateInstance((Type)itemsListView.SelectedItems[0].Tag);
        DialogResult = DialogResult.OK;
        Close();
      }
    }
    private void itemTypesListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        item = (IItem)Activator.CreateInstance((Type)itemsListView.SelectedItems[0].Tag);
        DialogResult = DialogResult.OK;
        Close();
      }
    }
  }
}
