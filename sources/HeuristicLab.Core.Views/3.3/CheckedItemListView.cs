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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a list of checked items.
  /// </summary>
  [View("CheckedItemList View")]
  [Content(typeof(CheckedItemList<>), true)]
  [Content(typeof(ICheckedItemList<>), false)]
  public partial class CheckedItemListView<T> : ItemView where T : class, IItem {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the content to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ItemView.Content"/> of base class <see cref="ItemView"/>
    /// </remarks>
    public new ICheckedItemList<T> Content {
      get { return (ICheckedItemList<T>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckedItemListView"/> with caption "Checked Item List".
    /// </summary>
    public CheckedItemListView() {
      InitializeComponent();
      Caption = "Checked Item List";
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="ICheckedItemList"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ItemView.DeregisterContentEvents"/> of base class <see cref="ItemView"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsRemoved);
      Content.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
      Content.CheckedItemsChanged -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="ICheckedItemList"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ItemView.RegisterContentEvents"/> of base class <see cref="ItemView"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsRemoved);
      Content.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
      Content.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Checked Item List";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      viewHost.Content = null;
      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        foreach (T item in Content)
          AddListViewItem(CreateListViewItem(item));
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      if (Content == null) {
        addButton.Enabled = false;
        moveUpButton.Enabled = false;
        moveDownButton.Enabled = false;
        removeButton.Enabled = false;
        itemsListView.Enabled = false;
        detailsGroupBox.Enabled = false;
      } else {
        addButton.Enabled = !Content.IsReadOnly && !ReadOnly;
        moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != 0 &&
                               !Content.IsReadOnly && !ReadOnly;
        moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                                 itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                                 !Content.IsReadOnly && !ReadOnly;
        removeButton.Enabled = itemsListView.SelectedItems.Count > 0 &&
                               !Content.IsReadOnly && !ReadOnly;
        itemsListView.Enabled = true;
        detailsGroupBox.Enabled = true;
      }
    }

    protected virtual T CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Item";
        typeSelectorDialog.TypeSelector.Caption = "Available Items";
        typeSelectorDialog.TypeSelector.Configure(typeof(T), false, false);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
      return null;
    }
    protected virtual ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
      listViewItem.Tag = item;
      listViewItem.Checked = Content.ItemChecked(item);
      itemsListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      ((T)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      ((T)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
      AdjustListViewColumnSizes();
    }
    protected virtual void InsertListViewItem(int index, ListViewItem listViewItem) {
      itemsListView.Items.Insert(index, listViewItem);
      ((T)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      ((T)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
      AdjustListViewColumnSizes();
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      ((T)listViewItem.Tag).ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      ((T)listViewItem.Tag).ToStringChanged -= new EventHandler(Item_ToStringChanged);
      listViewItem.Remove();
      foreach (ListViewItem other in itemsListView.Items)
        if (other.ImageIndex > listViewItem.ImageIndex) other.ImageIndex--;
      itemsListView.SmallImageList.Images.RemoveAt(listViewItem.ImageIndex);
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

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                             itemsListView.SelectedIndices[0] != 0 &&
                             (Content != null) && !Content.IsReadOnly && !ReadOnly;
      moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                               (Content != null) && !Content.IsReadOnly && !ReadOnly;
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 &&
                             (Content != null) && !Content.IsReadOnly && !ReadOnly;

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

    protected virtual void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content.RemoveAt(item.Index);
        }
      }
    }

    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = (T)itemsListView.SelectedItems[0].Tag;
        IContentView view = MainFormManager.MainForm.ShowContent(item);
        if (view != null) {
          view.ReadOnly = ReadOnly;
          view.Locked = Locked;
        }
      }
    }

    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        ListViewItem listViewItem = (ListViewItem)e.Item;
        T item = (T)listViewItem.Tag;
        DataObject data = new DataObject();
        data.SetData("Type", item.GetType());
        data.SetData("Value", item);
        if (Content.IsReadOnly || ReadOnly) {
          DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
        } else {
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
          if ((result & DragDropEffects.Move) == DragDropEffects.Move)
            Content.RemoveAt(listViewItem.Index);
        }
      }
    }
    protected virtual void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if (!Content.IsReadOnly && !ReadOnly && (type != null) && (typeof(T).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Link;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (T)item.Clone();

        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        if (listViewItem != null) Content.Insert(listViewItem.Index, item);
        else Content.Add(item);
      }
    }

    protected virtual void itemsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var checkedItem = (T)e.Item.Tag;
      if (Content.ItemChecked(checkedItem) != e.Item.Checked)
        Content.SetItemCheckedState(checkedItem, e.Item.Checked);
    }
    #endregion

    #region Button Events
    protected virtual void addButton_Click(object sender, EventArgs e) {
      T item = CreateItem();
      if (item != null)
        Content.Add(item);
    }
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
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          Content.RemoveAt(item.Index);
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsAdded), sender, e);
      else
        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsRemoved), sender, e);
      else {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (IndexedItem<T> item in e.Items)
          listViewItems.Add(itemsListView.Items[item.Index]);
        foreach (ListViewItem listViewItem in listViewItems)
          RemoveListViewItem(listViewItem);
      }
    }
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
          UpdateListViewItemImage(listViewItem);
          UpdateListViewItemText(listViewItem);
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
    protected virtual void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged), sender, e);
      else {
        foreach (var item in e.Items) {
          if (itemsListView.Items[item.Index].Checked != Content.ItemChecked(item.Value))
            itemsListView.Items[item.Index].Checked = Content.ItemChecked(item.Value);
        }
      }
    }

    #endregion

    #region Item Events
    protected virtual void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        T item = (T)sender;
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (((T)listViewItem.Tag) == item)
            UpdateListViewItemImage(listViewItem);
        }
      }
    }
    protected virtual void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        T item = (T)sender;
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (((T)listViewItem.Tag) == item)
            UpdateListViewItemText(listViewItem);
        }
        AdjustListViewColumnSizes();
      }
    }
    #endregion

    #region Helpers
    protected virtual void AdjustListViewColumnSizes() {
      if (itemsListView.Items.Count > 0) {
        for (int i = 0; i < itemsListView.Columns.Count; i++)
          itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }
    #endregion

  }
}
