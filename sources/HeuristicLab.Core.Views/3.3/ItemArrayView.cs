#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of all variables in a specified scope.
  /// </summary>
  [Content(typeof(ItemArray<>), true)]
  [Content(typeof(IObservableArray<>), false)]
  public partial class ItemArrayView<T> : ContentView where T : class, IItem {
    /// <summary>
    /// Gets or sets the scope whose variables to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No won data storage present.</remarks>
    public new IObservableArray<T> Content {
      get { return (IObservableArray<T>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ItemArrayView() {
      InitializeComponent();
      Caption = "Item Array";
    }
    public ItemArrayView(IObservableArray<T> content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IScope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IScope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Item Array";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      itemsListView.Enabled = false;
      detailsGroupBox.Enabled = false;
      viewHost.Content = null;
      moveUpButton.Enabled = false;
      moveDownButton.Enabled = false;

      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        itemsListView.Enabled = true;
        foreach (T item in Content)
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
      if (item == null) {
        return new ListViewItem("null");
      } else {
        if (!itemsListView.SmallImageList.Images.ContainsKey(item.GetType().FullName))
          itemsListView.SmallImageList.Images.Add(item.GetType().FullName, item.ItemImage);

        ListViewItem listViewItem = new ListViewItem();
        listViewItem.Text = item.ToString();
        listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.IndexOfKey(item.GetType().FullName);
        listViewItem.Tag = item;
        return listViewItem;
      }
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      if (listViewItem.Tag != null)
        ((T)listViewItem.Tag).Changed += new ChangedEventHandler(Item_Changed);
    }
    protected virtual void InsertListViewItem(int index, ListViewItem listViewItem) {
      itemsListView.Items.Insert(index, listViewItem);
      if (listViewItem.Tag != null)
        ((T)listViewItem.Tag).Changed += new ChangedEventHandler(Item_Changed);
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      if (listViewItem.Tag != null)
        ((T)listViewItem.Tag).Changed -= new ChangedEventHandler(Item_Changed);
      listViewItem.Remove();
    }
    protected virtual void UpdateListViewItem(ListViewItem listViewItem) {
      T item = listViewItem.Tag as T;
      if ((item != null) && (!itemsListView.SmallImageList.Images.ContainsKey(item.GetType().FullName)))
        itemsListView.SmallImageList.Images.Add(item.GetType().FullName, item.ItemImage);

      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
      listViewItem.ImageIndex = item == null ? -1 : itemsListView.SmallImageList.Images.IndexOfKey(item.GetType().FullName);
    }

    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                             itemsListView.SelectedIndices[0] != 0 &&
                             !Content.IsReadOnly;
      moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                               !Content.IsReadOnly;

      if (itemsListView.SelectedItems.Count == 1) {
        T item = itemsListView.SelectedItems[0].Tag as T;
        viewHost.Content = item;
        detailsGroupBox.Enabled = true;
      } else {
        viewHost.Content = null;
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
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content[item.Index] = null;
        }
      }
    }

    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = itemsListView.SelectedItems[0].Tag as T;
        if (item != null) {
          IView view = MainFormManager.CreateDefaultView(item);
          if (view != null) view.Show();
        }
      }
    }

    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      T item = listViewItem.Tag as T;
      if (item != null) {
        DataObject data = new DataObject();
        data.SetData("Type", item.GetType());
        data.SetData("Value", item);
        if (Content.IsReadOnly) {
          DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
        } else {
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
          if ((result & DragDropEffects.Move) == DragDropEffects.Move)
            Content[listViewItem.Index] = null;
        }
      }
    }
    protected virtual void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((!Content.IsReadOnly) && (type != null) && (typeof(T).IsAssignableFrom(type))) {
        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        if (listViewItem != null) {
          if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
          else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
          else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
          else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
          else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        }
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (T)item.Clone();

        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        Content[listViewItem.Index] = item;
      }
    }
    #endregion

    #region Button Events
    protected virtual void moveUpButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        int index = itemsListView.SelectedIndices[0];
        T item = Content[index - 1];
        Content[index - 1] = Content[index];
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index - 1].Selected = true;
        Content[index] = item;
      }
    }
    protected virtual void moveDownButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        int index = itemsListView.SelectedIndices[0];
        T item = Content[index + 1];
        Content[index + 1] = Content[index];
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index + 1].Selected = true;
        Content[index] = item;
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced), sender, e);
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
    protected virtual void Content_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved), sender, e);
      else {
        foreach (IndexedItem<T> item in e.Items) {
          ListViewItem listViewItem = itemsListView.Items[item.Index];
          listViewItem.Tag = item.Value;
          UpdateListViewItem(listViewItem);
        }
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset), sender, e);
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
