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

using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection View")]
  [Content(typeof(RunCollection), true)]
  [Content(typeof(IObservableCollection<IRun>), false)]
  public partial class RunCollectionView : AsynchronousContentView {
    public new IObservableCollection<IRun> Content {
      get { return (IObservableCollection<IRun>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    public RunCollectionView() {
      InitializeComponent();
      Caption = "Run Collection";
    }

    public RunCollectionView(IObservableCollection<IRun> content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Run Collection";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      itemsListView.Enabled = false;
      detailsGroupBox.Enabled = false;
      viewHost.Content = null;
      removeButton.Enabled = false;

      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        itemsListView.Enabled = true;
        foreach (IRun item in Content)
          AddListViewItem(CreateListViewItem(item));
      }
    }

    protected virtual ListViewItem CreateListViewItem(IRun item) {
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
      ((IRun)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      ((IRun)listViewItem.Tag).ToStringChanged -= new EventHandler(Item_ToStringChanged);
      listViewItem.Remove();
    }
    protected virtual void UpdateListViewItem(ListViewItem listViewItem) {
      if (!listViewItem.Text.Equals(listViewItem.Tag.ToString())) {
        listViewItem.Text = listViewItem.Tag.ToString();
      }
    }
    protected virtual IEnumerable<ListViewItem> GetListViewItemsForItem(IRun item) {
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        if (((IRun)listViewItem.Tag) == item)
          yield return listViewItem;
      }
    }

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly;
      if (itemsListView.SelectedItems.Count == 1) {
        IRun item = (IRun)itemsListView.SelectedItems[0].Tag;
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
            Content.Remove((IRun)item.Tag);
        }
      }
    }
    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IRun item = (IRun)itemsListView.SelectedItems[0].Tag;
        IView view = MainFormManager.CreateDefaultView(item);
        if (view != null) view.Show();
      }
    }
    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      IRun item = (IRun)listViewItem.Tag;
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
      if ((!Content.IsReadOnly) && (type != null) && (typeof(IRun).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
      }
    }
    protected virtual void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IRun item = e.Data.GetData("Value") as IRun;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (IRun)item.Clone();
        Content.Add(item);
      }
    }
    #endregion

    #region Button Events
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          Content.Remove((IRun)item.Tag);
        itemsListView.SelectedItems.Clear();
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
      else
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
      else {
        foreach (IRun item in e.Items) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            RemoveListViewItem(listViewItem);
            break;
          }
        }
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
      else {
        foreach (IRun item in e.OldItems) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            RemoveListViewItem(listViewItem);
            break;
          }
        }
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));
      }
    }
    #endregion

    #region Item Events
    protected virtual void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItem(listViewItem);
      }
    }
    #endregion
  }
}

