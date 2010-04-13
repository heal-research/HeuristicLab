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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  [View("ItemCollection View")]
  [Content(typeof(ItemCollection<>), true)]
  [Content(typeof(IObservableCollection<>), false)]
  public partial class ItemCollectionView<T> : AsynchronousContentView where T : class, IItem {
    public new IObservableCollection<T> Content {
      get { return (IObservableCollection<T>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    public ItemCollectionView() {
      InitializeComponent();
      Caption = "Item Collection";
    }

    public ItemCollectionView(IObservableCollection<T> content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Item Collection";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      itemsListView.Enabled = false;
      detailsGroupBox.Enabled = false;
      viewHost.Content = null;
      addButton.Enabled = false;
      sortAscendingButton.Enabled = false;
      sortDescendingButton.Enabled = false;
      removeButton.Enabled = false;

      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        itemsListView.Enabled = true;
        addButton.Enabled = !Content.IsReadOnly;
        foreach (T item in Content)
          AddListViewItem(CreateListViewItem(item));
        sortAscendingButton.Enabled = itemsListView.Items.Count > 0;
        sortDescendingButton.Enabled = itemsListView.Items.Count > 0;
        SortItemsListView(SortOrder.Ascending);
      }
    }

    protected virtual T CreateItem() {
      try {
        return (T)Activator.CreateInstance(typeof(T));
      } catch(Exception ex) {
        Auxiliary.ShowErrorMessageBox(ex);
        return null;
      }
    }
    protected virtual ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
      listViewItem.Tag = item;
      itemsListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      ((T)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      ((T)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
      sortAscendingButton.Enabled = itemsListView.Items.Count > 0;
      sortDescendingButton.Enabled = itemsListView.Items.Count > 0;
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      ((T)listViewItem.Tag).ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      ((T)listViewItem.Tag).ToStringChanged -= new EventHandler(Item_ToStringChanged);
      listViewItem.Remove();
      sortAscendingButton.Enabled = itemsListView.Items.Count > 0;
      sortDescendingButton.Enabled = itemsListView.Items.Count > 0;
    }
    protected virtual void UpdateListViewItemImage(ListViewItem listViewItem) {
      int i = listViewItem.ImageIndex;
      listViewItem.ImageList.Images[i] = ((T)listViewItem.Tag).ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    protected virtual void UpdateListViewItemText(ListViewItem listViewItem) {
      if (!listViewItem.Text.Equals(listViewItem.Tag.ToString()))
        listViewItem.Text = listViewItem.Tag.ToString();
    }
    protected virtual IEnumerable<ListViewItem> GetListViewItemsForItem(T item) {
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        if (((T)listViewItem.Tag) == item)
          yield return listViewItem;
      }
    }

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly;
      if (itemsListView.SelectedItems.Count == 1) {
        T item = (T)itemsListView.SelectedItems[0].Tag;
        detailsGroupBox.Enabled = true;
        viewHost.ViewType = null;
        viewHost.Content = item;
      } else {
        viewHost.Content = null;
        detailsGroupBox.Enabled = false;
      }
    }
    protected virtual void itemsListView_SizeChanged(object sender, EventArgs e) {
      if (itemsListView.Columns.Count > 0)
        itemsListView.Columns[0].Width = Math.Max(0, itemsListView.Width - 25);
    }
    protected virtual void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content.Remove((T)item.Tag);
        }
      }
    }
    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = (T)itemsListView.SelectedItems[0].Tag;
        IView view = MainFormManager.CreateDefaultView(item);
        if (view != null) view.Show();
      }
    }
    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      T item = (T)listViewItem.Tag;
      DataObject data = new DataObject();
      data.SetData("Type", item.GetType());
      data.SetData("Value", item);
      if (Content.IsReadOnly) {
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else {
        DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if ((result & DragDropEffects.Move) == DragDropEffects.Move)
          Content.Remove(item);
      }
    }
    protected virtual void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((!Content.IsReadOnly) && (type != null) && (typeof(T).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (T)item.Clone();
        Content.Add(item);
      }
    }
    #endregion

    #region Button Events
    protected virtual void addButton_Click(object sender, EventArgs e) {
      T item = CreateItem();
      if (item != null)
        Content.Add(item);
    }
    protected virtual void sortAscendingButton_Click(object sender, EventArgs e) {
      SortItemsListView(SortOrder.Ascending);
    }
    protected virtual void sortDescendingButton_Click(object sender, EventArgs e) {
      SortItemsListView(SortOrder.Descending);
    }
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          Content.Remove((T)item.Tag);
        itemsListView.SelectedItems.Clear();
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded), sender, e);
      else
        foreach (T item in e.Items)
          AddListViewItem(CreateListViewItem(item));
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved), sender, e);
      else {
        foreach (T item in e.Items) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            RemoveListViewItem(listViewItem);
            break;
          }
        }
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_CollectionReset), sender, e);
      else {
        foreach (T item in e.OldItems) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            RemoveListViewItem(listViewItem);
            break;
          }
        }
        foreach (T item in e.Items)
          AddListViewItem(CreateListViewItem(item));
      }
    }
    #endregion

    #region Item Events
    protected virtual void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        T item = (T)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemImage(listViewItem);
      }
    }
    protected virtual void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        T item = (T)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemText(listViewItem);
      }
    }
    #endregion

    #region Helpers
    protected virtual void SortItemsListView(SortOrder sortOrder) {
      itemsListView.Sorting = SortOrder.None;
      itemsListView.Sorting = sortOrder;
      itemsListView.Sorting = SortOrder.None;
    }
    #endregion
  }
}
