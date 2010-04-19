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
  /// The visual representation of all variables in a specified scope.
  /// </summary>
  [View("ItemArray View")]
  [Content(typeof(ItemArray<>), true)]
  [Content(typeof(IItemArray<>), false)]
  public partial class ItemArrayView<T> : AsynchronousContentView where T : class, IItem {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the scope whose variables to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No won data storage present.</remarks>
    public new IItemArray<T> Content {
      get { return (IItemArray<T>)base.Content; }
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
    public ItemArrayView(IItemArray<T> content)
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
        addButton.Enabled = itemsListView.SelectedItems.Count > 0 &&
                            !Content.IsReadOnly && !ReadOnly;
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
        viewHost.ReadOnly = ReadOnly;
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
      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
      itemsListView.SmallImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VS2008ImageLibrary.Nothing : item.ItemImage);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = item;
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      if (listViewItem.Tag != null) {
        ((T)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
        ((T)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
      }
      AdjustListViewColumnSizes();
    }
    protected virtual void InsertListViewItem(int index, ListViewItem listViewItem) {
      itemsListView.Items.Insert(index, listViewItem);
      if (listViewItem.Tag != null) {
        ((T)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
        ((T)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
      }
      AdjustListViewColumnSizes();
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      if (listViewItem.Tag != null) {
        ((T)listViewItem.Tag).ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
        ((T)listViewItem.Tag).ToStringChanged -= new EventHandler(Item_ToStringChanged);
      }
      listViewItem.Remove();
      foreach (ListViewItem other in itemsListView.Items)
        if (other.ImageIndex > listViewItem.ImageIndex) other.ImageIndex--;
      itemsListView.SmallImageList.Images.RemoveAt(listViewItem.ImageIndex);
    }
    protected virtual void UpdateListViewItemImage(ListViewItem listViewItem) {
      T item = listViewItem.Tag as T;
      int i = listViewItem.ImageIndex;
      listViewItem.ImageList.Images[i] = item == null ? HeuristicLab.Common.Resources.VS2008ImageLibrary.Nothing : item.ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    protected virtual void UpdateListViewItemText(ListViewItem listViewItem) {
      T item = listViewItem.Tag as T;
      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
    }

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      addButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly && !ReadOnly;
      moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                             itemsListView.SelectedIndices[0] != 0 &&
                             !Content.IsReadOnly && !ReadOnly;
      moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                               !Content.IsReadOnly && !ReadOnly;
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly && !ReadOnly;

      if (itemsListView.SelectedItems.Count == 1) {
        T item = itemsListView.SelectedItems[0].Tag as T;
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
            Content[item.Index] = null;
        }
      }
    }

    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = itemsListView.SelectedItems[0].Tag as T;
        if (item != null) {
          IView view = MainFormManager.CreateDefaultView(item);
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Show();
          }
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
        if (Content.IsReadOnly || ReadOnly) {
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
      if (!Content.IsReadOnly && !ReadOnly && (type != null) && (typeof(T).IsAssignableFrom(type))) {
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
    protected virtual void addButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        T item = CreateItem();
        if (item != null) {
          foreach (ListViewItem listViewItem in itemsListView.SelectedItems)
            Content[listViewItem.Index] = item;
        }
      }
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
          Content[item.Index] = null;
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
