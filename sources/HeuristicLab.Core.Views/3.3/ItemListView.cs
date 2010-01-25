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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of all variables in a specified scope.
  /// </summary>
  public partial class ItemListView<T> : ObjectView where T : class, IItem {
    /// <summary>
    /// Gets or sets the scope whose variables to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No won data storage present.</remarks>
    public IObservableList<T> ItemList {
      get { return (IObservableList<T>)Object; }
      set { base.Object = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ItemListView() {
      InitializeComponent();
      Caption = "Item List";
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IScope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterObjectEvents() {
      ItemList.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsAdded);
      ItemList.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsRemoved);
      ItemList.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsReplaced);
      ItemList.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsMoved);
      ItemList.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_CollectionReset);
      base.DeregisterObjectEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IScope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      ItemList.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsAdded);
      ItemList.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsRemoved);
      ItemList.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsReplaced);
      ItemList.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsMoved);
      ItemList.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_CollectionReset);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      Caption = "Item List";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      itemsListView.Enabled = false;
      detailsGroupBox.Enabled = false;
      viewHost.Object = null;
      addButton.Enabled = false;
      moveUpButton.Enabled = false;
      moveDownButton.Enabled = false;
      removeButton.Enabled = false;

      if (ItemList != null) {
        Caption += " (" + ItemList.GetType().Name + ")";
        itemsListView.Enabled = true;
        addButton.Enabled = !ItemList.IsReadOnly;
        foreach (T item in ItemList)
          AddListViewItem(CreateListViewItem(item));
      }
    }

    protected virtual T CreateItem() {
      try {
        return (T)Activator.CreateInstance(typeof(T));
      }
      catch (Exception ex) {
        Auxiliary.ShowErrorMessageBox(ex);
        return null;
      }
    }
    protected virtual ListViewItem CreateListViewItem(T item) {
      if (!itemsListView.SmallImageList.Images.ContainsKey(item.GetType().FullName))
        itemsListView.SmallImageList.Images.Add(item.GetType().FullName, item.ItemImage);

      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.IndexOfKey(item.GetType().FullName);
      listViewItem.Tag = item;
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      ((T)listViewItem.Tag).Changed += new ChangedEventHandler(Item_Changed);
    }
    protected virtual void InsertListViewItem(int index, ListViewItem listViewItem) {
      itemsListView.Items.Insert(index, listViewItem);
      ((T)listViewItem.Tag).Changed += new ChangedEventHandler(Item_Changed);
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      ((T)listViewItem.Tag).Changed -= new ChangedEventHandler(Item_Changed);
      listViewItem.Remove();
    }
    protected virtual void UpdateListViewItem(ListViewItem listViewItem) {
      T item = (T)listViewItem.Tag;
      if (!itemsListView.SmallImageList.Images.ContainsKey(item.GetType().FullName))
        itemsListView.SmallImageList.Images.Add(item.GetType().FullName, item.ItemImage);

      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.IndexOfKey(item.GetType().FullName);
    }

    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                             itemsListView.SelectedIndices[0] != 0 &&
                             !ItemList.IsReadOnly;
      moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                               !ItemList.IsReadOnly;
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 &&
                             !ItemList.IsReadOnly;

      if (itemsListView.SelectedItems.Count == 1) {
        T item = (T)itemsListView.SelectedItems[0].Tag;
        viewHost.Object = item;
        detailsGroupBox.Enabled = true;
      } else {
        viewHost.Object = null;
        detailsGroupBox.Enabled = false;
      }
    }

    #region ListView Events
    protected virtual void itemsListView_SizeChanged(object sender, EventArgs e) {
      if (itemsListView.Columns.Count > 0)
        itemsListView.Columns[0].Width = Math.Max(0, itemsListView.Width - 25);
    }

    protected virtual void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !ItemList.IsReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            ItemList.RemoveAt(item.Index);
        }
      }
    }

    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = (T)itemsListView.SelectedItems[0].Tag;
        IView view = MainFormManager.CreateDefaultView(item);
        if (view != null) MainFormManager.MainForm.ShowView(view);
      }
    }

    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      T item = (T)listViewItem.Tag;
      DataObject data = new DataObject();
      data.SetData("Type", item.GetType());
      data.SetData("Value", item);
      if (ItemList.IsReadOnly) {
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else {
        DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if ((result & DragDropEffects.Move) == DragDropEffects.Move)
          ItemList.RemoveAt(listViewItem.Index);
      }
    }
    protected virtual void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((!ItemList.IsReadOnly) && (type != null) && (typeof(T).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Move;  // ALT key
        else e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (T)item.Clone();

        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        if (listViewItem != null) ItemList.Insert(listViewItem.Index, item);
        else ItemList.Add(item);
      }
    }
    #endregion

    #region Button Events
    protected virtual void addButton_Click(object sender, EventArgs e) {
      T item = CreateItem();
      if (item != null)
        ItemList.Add(item);
    }
    protected virtual void moveUpButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        int index = itemsListView.SelectedIndices[0];
        T item = ItemList[index - 1];
        ItemList[index - 1] = ItemList[index];
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index - 1].Selected = true;
        ItemList[index] = item;
      }
    }
    protected virtual void moveDownButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        int index = itemsListView.SelectedIndices[0];
        T item = ItemList[index + 1];
        ItemList[index + 1] = ItemList[index];
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index + 1].Selected = true;
        ItemList[index] = item;
      }
    }
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          ItemList.RemoveAt(item.Index);
      }
    }
    #endregion

    #region NamedItemCollection Events
    protected virtual void ItemList_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsAdded), sender, e);
      else
        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));
    }
    protected virtual void ItemList_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsRemoved), sender, e);
      else {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (IndexedItem<T> item in e.Items)
          listViewItems.Add(itemsListView.Items[item.Index]);
        foreach (ListViewItem listViewItem in listViewItems)
          RemoveListViewItem(listViewItem);
      }
    }
    protected virtual void ItemList_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsReplaced), sender, e);
      else {
        int[] selected = new int[itemsListView.SelectedIndices.Count];
        itemsListView.SelectedIndices.CopyTo(selected, 0);

        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (IndexedItem<T> item in e.OldItems)
          listViewItems.Add(itemsListView.Items[item.Index]);
        foreach (ListViewItem listViewItem in listViewItems)
          RemoveListViewItem(listViewItem);

        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));

        for (int i = 0; i < selected.Length; i++)
          itemsListView.Items[selected[i]].Selected = true;
      }
    }
    protected virtual void ItemList_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_ItemsMoved), sender, e);
      else {
        foreach (IndexedItem<T> item in e.Items) {
          ListViewItem listViewItem = itemsListView.Items[item.Index];
          listViewItem.Tag = item.Value;
          UpdateListViewItem(listViewItem);
        }
      }
    }
    protected virtual void ItemList_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(ItemList_CollectionReset), sender, e);
      else {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (IndexedItem<T> item in e.OldItems)
          listViewItems.Add(itemsListView.Items[item.Index]);
        foreach (ListViewItem listViewItem in listViewItems)
          RemoveListViewItem(listViewItem);

        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));
      }
    }
    #endregion

    #region Item Events
    protected virtual void Item_Changed(object sender, ChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new ChangedEventHandler(Item_Changed), sender, e);
      else {
        T item = (T)sender;
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (((T)listViewItem.Tag) == item)
            UpdateListViewItem(listViewItem);
        }
      }
    }
    #endregion
  }
}
