#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  [View("ItemArray View")]
  [Content(typeof(ItemArray<>), true)]
  [Content(typeof(IItemArray<>), false)]
  [Content(typeof(ReadOnlyItemArray<>), true)]
  public partial class ItemArrayView<T> : ItemView where T : class, IItem {
    protected Dictionary<T, List<ListViewItem>> itemListViewItemMapping;
    protected TypeSelectorDialog typeSelectorDialog;
    protected bool validDragOperation;

    public new IItemArray<T> Content {
      get { return (IItemArray<T>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    public ItemArrayView() {
      InitializeComponent();
      itemListViewItemMapping = new Dictionary<T, List<ListViewItem>>();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
      foreach (T item in itemListViewItemMapping.Keys) {
        DeregisterItemEvents(item);
      }
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsReplaced);
      Content.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_ItemsMoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CollectionReset);
    }
    protected virtual void DeregisterItemEvents(T item) {
      item.ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
    }
    protected virtual void RegisterItemEvents(T item) {
      item.ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged += new EventHandler(Item_ToStringChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      int selectedIndex = -1;
      if (itemsListView.SelectedItems.Count == 1) selectedIndex = itemsListView.SelectedIndices[0];

      itemsListView.Items.Clear();
      itemListViewItemMapping.Clear();
      RebuildImageList();
      viewHost.Content = null;
      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        foreach (T item in Content)
          AddListViewItem(CreateListViewItem(item));
        AdjustListViewColumnSizes();
        if ((selectedIndex != -1) && (selectedIndex < itemsListView.Items.Count))
          itemsListView.Items[selectedIndex].Selected = true;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
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
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
      }
    }

    protected virtual T CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Item";
        typeSelectorDialog.TypeSelector.Caption = "Available Items";
        typeSelectorDialog.TypeSelector.Configure(typeof(T), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }
    protected virtual ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = new ListViewItem();
      if (item == null) {
        listViewItem.Text = "null";
        itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Nothing);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      } else {
        listViewItem.Text = item.ToString();
        listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
        itemsListView.SmallImageList.Images.Add(item.ItemImage);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
        listViewItem.Tag = item;
      }
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      T item = (listViewItem.Tag as T);
      itemsListView.Items.Add(listViewItem);
      if (item != null) {
        if (!itemListViewItemMapping.ContainsKey(item)) {
          RegisterItemEvents(item);
          itemListViewItemMapping.Add(item, new List<ListViewItem>());
        }
        itemListViewItemMapping[item].Add(listViewItem);
      }
    }
    protected virtual void InsertListViewItem(int index, ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      T item = (listViewItem.Tag as T);
      itemsListView.Items.Insert(index, listViewItem);
      if (item != null) {
        if (!itemListViewItemMapping.ContainsKey(item)) {
          RegisterItemEvents(item);
          itemListViewItemMapping.Add(item, new List<ListViewItem>());
        }
        itemListViewItemMapping[item].Add(listViewItem);
      }
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      T item = (listViewItem.Tag as T);
      if (item != null) {
        itemListViewItemMapping[item].Remove(listViewItem);
        if (itemListViewItemMapping[item].Count == 0) {
          itemListViewItemMapping.Remove(item);
          DeregisterItemEvents(item);
        }
      }
      listViewItem.Remove();
    }
    protected virtual void UpdateListViewItemImage(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      T item = listViewItem.Tag as T;
      int i = listViewItem.ImageIndex;
      itemsListView.SmallImageList.Images[i] = item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    protected virtual void UpdateListViewItemText(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      T item = listViewItem.Tag as T;
      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
    }
    protected virtual IEnumerable<ListViewItem> GetListViewItemsForItem(T item) {
      if (item == null) {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (listViewItem.Tag == null) listViewItems.Add(listViewItem);
        }
        return listViewItems;
      } else {
        List<ListViewItem> listViewItems = null;
        itemListViewItemMapping.TryGetValue(item, out listViewItems);
        return listViewItems == null ? Enumerable.Empty<ListViewItem>() : listViewItems;
      }
    }

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      addButton.Enabled = itemsListView.SelectedItems.Count > 0 && (Content != null) && !Content.IsReadOnly && !ReadOnly;
      moveUpButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                             itemsListView.SelectedIndices[0] != 0 &&
                             (Content != null) && !Content.IsReadOnly && !ReadOnly;
      moveDownButton.Enabled = itemsListView.SelectedItems.Count == 1 &&
                               itemsListView.SelectedIndices[0] != itemsListView.Items.Count - 1 &&
                               (Content != null) && !Content.IsReadOnly && !ReadOnly;
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && (Content != null) && !Content.IsReadOnly && !ReadOnly;

      if (showDetailsCheckBox.Checked) {
        if (itemsListView.SelectedItems.Count == 1) {
          T item = itemsListView.SelectedItems[0].Tag as T;
          detailsGroupBox.Enabled = true;
          viewHost.Content = item;
        } else {
          viewHost.Content = null;
          detailsGroupBox.Enabled = false;
        }
      }
    }

    protected virtual void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content[item.Index] = null;
        }
      } else if (e.KeyData == (Keys.Control | Keys.C)) {
        if (itemsListView.SelectedItems.Count > 0) {
          var builder = new StringBuilder();
          foreach (ListViewItem selected in itemsListView.SelectedItems) {
            builder.AppendLine(selected.Text);
          }
          Clipboard.SetText(builder.ToString());
        }
      }
    }

    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        T item = itemsListView.SelectedItems[0].Tag as T;
        if (item != null) {
          IContentView view = MainFormManager.MainForm.ShowContent(item);
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Locked = Locked;
          }
        }
      }
    }

    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        List<T> items = new List<T>();
        foreach (ListViewItem listViewItem in itemsListView.SelectedItems) {
          T item = listViewItem.Tag as T;
          if (item != null) items.Add(item);
        }

        if (items.Count > 0) {
          DataObject data = new DataObject();
          if (items.Count == 1) data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items[0]);
          else data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items);
          if (Content.IsReadOnly || ReadOnly) {
            DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
          } else {
            DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
            if ((result & DragDropEffects.Move) == DragDropEffects.Move) {
              foreach (ListViewItem listViewItem in itemsListView.SelectedItems.Cast<ListViewItem>().ToArray())
                Content[listViewItem.Index] = null;
            }
          }
        }
      }
    }
    protected virtual void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = !Content.IsReadOnly && !ReadOnly && e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is T;
    }
    protected virtual void itemsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        if (listViewItem != null) {
          if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
          else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
          else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
          else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
          else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
        }
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        Point p = itemsListView.PointToClient(new Point(e.X, e.Y));
        ListViewItem listViewItem = itemsListView.GetItemAt(p.X, p.Y);
        T item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as T;
        Content[listViewItem.Index] = e.Effect.HasFlag(DragDropEffects.Copy) ? (T)item.Clone() : item;
      }
    }
    protected virtual void itemsListView_Layout(object sender, LayoutEventArgs e) {
      if (itemsListView.Columns.Count == 1)
        AdjustListViewColumnSizes();
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
        Content.Reverse(index - 1, 2);
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index - 1].Selected = true;
      }
    }
    protected virtual void moveDownButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        int index = itemsListView.SelectedIndices[0];
        Content.Reverse(index, 2);
        itemsListView.Items[index].Selected = false;
        itemsListView.Items[index + 1].Selected = true;
      }
    }
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          Content[item.Index] = null;
      }
    }
    #endregion

    #region CheckBox Events
    protected virtual void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
        viewHost.Content = itemsListView.SelectedItems.Count == 1 ? (T)itemsListView.SelectedItems[0].Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        viewHost.Content = null;
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
        RebuildImageList();

        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));
        AdjustListViewColumnSizes();

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
          if (listViewItem.Tag != null)
            itemListViewItemMapping[(T)listViewItem.Tag].Remove(listViewItem);
          listViewItem.Tag = item.Value;
          if (listViewItem.Tag != null)
            itemListViewItemMapping[item.Value].Add(listViewItem);
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
        RebuildImageList();

        foreach (IndexedItem<T> item in e.Items)
          InsertListViewItem(item.Index, CreateListViewItem(item.Value));
        AdjustListViewColumnSizes();
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
        if (itemsListView.Columns.Count > 1)
          AdjustListViewColumnSizes();
      }
    }
    #endregion

    #region Helpers
    protected virtual void AdjustListViewColumnSizes() {
      if (itemsListView.Columns.Count == 1) {
        if (itemsListView.Columns[0].Width != itemsListView.ClientSize.Width)
          itemsListView.Columns[0].Width = itemsListView.ClientSize.Width;
      } else {
        if (itemsListView.Items.Count > 0) {
          for (int i = 0; i < itemsListView.Columns.Count; i++)
            itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }
    protected virtual void RebuildImageList() {
      itemsListView.SmallImageList.Images.Clear();
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        T item = listViewItem.Tag as T;
        itemsListView.SmallImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      }
    }
    #endregion
  }
}
