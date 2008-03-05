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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public partial class ItemListView<T> : ViewBase where T : IItem {
    private ChooseItemDialog chooseItemDialog;

    public ItemList<T> ItemList {
      get { return (ItemList<T>)Item; }
      set { base.Item = value; }
    }

    public ItemListView() {
      InitializeComponent();
      itemsListView.Columns[0].Width = Math.Max(0, itemsListView.Width - 25);
    }
    public ItemListView(ItemList<T> itemList)
      : this() {
      ItemList = itemList;
    }

    protected override void RemoveItemEvents() {
      ItemList.ItemAdded -= new EventHandler<ItemIndexEventArgs>(ItemList_ItemInserted);
      ItemList.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(ItemList_ItemRemoved);
      ItemList.Cleared -= new EventHandler(ItemList_Cleared);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ItemList.ItemAdded += new EventHandler<ItemIndexEventArgs>(ItemList_ItemInserted);
      ItemList.ItemRemoved += new EventHandler<ItemIndexEventArgs>(ItemList_ItemRemoved);
      ItemList.Cleared += new EventHandler(ItemList_Cleared);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      removeButton.Enabled = false;
      if (ItemList == null) {
        typeTextBox.Text = "";
        splitContainer.Enabled = false;
      } else {
        typeTextBox.Text = typeof(T).FullName;
        splitContainer.Enabled = true;
        foreach (ListViewItem item in itemsListView.Items) {
          ((IItem)item.Tag).Changed -= new EventHandler(Item_Changed);
        }
        itemsListView.Items.Clear();
        foreach (IItem data in ItemList) {
          ListViewItem item = new ListViewItem();
          item.Text = data.ToString();
          item.Tag = data;
          itemsListView.Items.Add(item);
          data.Changed += new EventHandler(Item_Changed);
        }
      }
    }

    private void elementsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (detailsGroupBox.Controls.Count > 0)
        detailsGroupBox.Controls[0].Dispose();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      removeButton.Enabled = false;
      if (itemsListView.SelectedItems.Count > 0) {
        removeButton.Enabled = true;
      }
      if (itemsListView.SelectedItems.Count == 1) {
        IItem data = (IItem)itemsListView.SelectedItems[0].Tag;
        Control view = (Control)data.CreateView();
        detailsGroupBox.Controls.Add(view);
        view.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
      }
    }

    #region Size Changed Events
    private void elementsListView_SizeChanged(object sender, EventArgs e) {
      if (itemsListView.Columns.Count > 0)
        itemsListView.Columns[0].Width = Math.Max(0, itemsListView.Width - 25);
    }
    #endregion

    #region Key Events
    private void elementsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        while (itemsListView.SelectedIndices.Count > 0)
          ItemList.RemoveAt(itemsListView.SelectedIndices[0]);
      }
    }
    #endregion

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      if (chooseItemDialog == null) {
        chooseItemDialog = new ChooseItemDialog(typeof(T));
        chooseItemDialog.Caption = "Add Item";
      }
      if (chooseItemDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          ItemList.Add((T)chooseItemDialog.Item);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
    private void removeButton_Click(object sender, EventArgs e) {
      while (itemsListView.SelectedIndices.Count > 0)
        ItemList.RemoveAt(itemsListView.SelectedIndices[0]);
    }
    #endregion

    #region Drag and Drop Events
    private void elementsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem item = (ListViewItem)e.Item;
      IItem data = (IItem)item.Tag;
      DataObject dataObject = new DataObject();
      dataObject.SetData("IItem", data);
      dataObject.SetData("DragSource", itemsListView);
      DoDragDrop(dataObject, DragDropEffects.Move);
    }
    private void elementsListView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IItem")) {
        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem item = itemsListView.GetItemAt(p.X, p.Y);
        if (item != null)
          e.Effect = DragDropEffects.Move;
      }
    }
    private void elementsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IItem")) {
        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem item = itemsListView.GetItemAt(p.X, p.Y);
        if (item != null)
          e.Effect = DragDropEffects.Move;
      }
    }
    private void elementsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetDataPresent("IItem")) {
          IItem data = (IItem)e.Data.GetData("IItem");
          Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
          ListViewItem item = itemsListView.GetItemAt(p.X, p.Y);
          if (item != null) {
            int index = item.Index;
            ItemList.Remove((T)data);
            ItemList.Insert(index, (T)data);
            itemsListView.SelectedIndices.Clear();
            itemsListView.SelectedIndices.Add(index);
          }
        }
      }
    }
    #endregion

    #region Item and Item List Events
    private void ItemList_ItemInserted(object sender, ItemIndexEventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler<ItemIndexEventArgs>(ItemList_ItemInserted), sender, e);
      else {
        ListViewItem item = new ListViewItem();
        item.Text = e.Item.ToString();
        item.Tag = e.Item;
        itemsListView.Items.Insert(e.Index, item);
        e.Item.Changed += new EventHandler(Item_Changed);
      }
    }
    private void ItemList_ItemRemoved(object sender, ItemIndexEventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler<ItemIndexEventArgs>(ItemList_ItemRemoved), sender, e);
      else {
        itemsListView.Items.RemoveAt(e.Index);
        e.Item.Changed -= new EventHandler(Item_Changed);
      }
    }
    private void ItemList_Cleared(object sender, EventArgs e) {
      Refresh();
    }
    private void Item_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_Changed), sender, e);
      else {
        IItem data = (IItem)sender;
        foreach (ListViewItem item in itemsListView.Items) {
          if (item.Tag == data)
            item.Text = data.ToString();
        }
      }
    }
    #endregion
  }
}
