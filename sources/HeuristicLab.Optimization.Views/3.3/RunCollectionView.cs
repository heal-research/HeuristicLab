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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection View")]
  [Content(typeof(RunCollection), true)]
  [Content(typeof(IItemCollection<IRun>), false)]
  public partial class RunCollectionView : AsynchronousContentView {
    public new IItemCollection<IRun> Content {
      get { return (IItemCollection<IRun>)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    public RunCollectionView() {
      InitializeComponent();
      Caption = "Run Collection";
    }

    public RunCollectionView(IItemCollection<IRun> content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      DeregisterRunEvents(Content);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      RegisterRunEvents(Content);
    }
    protected virtual void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed += new EventHandler(run_Changed);
    }
    protected virtual void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed -= new EventHandler(run_Changed);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Run Collection";
      while (itemsListView.Items.Count > 0) RemoveListViewItem(itemsListView.Items[0]);
      viewHost.Content = null;

      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        foreach (IRun item in Content)
          AddListViewItem(CreateListViewItem(item));
      }
      SetEnabledStateOfControls();
    }

    private void run_Changed(object sender, EventArgs e) {
      IRun run = (IRun)sender;
      foreach (ListViewItem listViewItem in GetListViewItemsForItem(run)) {
        if (run.Visible) {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
          listViewItem.ForeColor = run.Color;
        } else {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
          listViewItem.ForeColor = Color.LightGray;
        }
      }
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }
    private void SetEnabledStateOfControls() {
      if (Content == null) {
        itemsListView.Enabled = false;
        detailsGroupBox.Enabled = false;
        viewHost.Enabled = false;
        removeButton.Enabled = false;
      } else {
        itemsListView.Enabled = true;
        detailsGroupBox.Enabled = true;
        removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        viewHost.Enabled = true;
      }
    }

    protected virtual ListViewItem CreateListViewItem(IRun item) {
      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
      itemsListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = item;
      return listViewItem;
    }
    protected virtual void AddListViewItem(ListViewItem listViewItem) {
      itemsListView.Items.Add(listViewItem);
      ((IRun)listViewItem.Tag).ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      ((IRun)listViewItem.Tag).ToStringChanged += new EventHandler(Item_ToStringChanged);
    }
    protected virtual void RemoveListViewItem(ListViewItem listViewItem) {
      ((IRun)listViewItem.Tag).ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      ((IRun)listViewItem.Tag).ToStringChanged -= new EventHandler(Item_ToStringChanged);
      listViewItem.Remove();
      foreach (ListViewItem other in itemsListView.Items)
        if (other.ImageIndex > listViewItem.ImageIndex) other.ImageIndex--;
      itemsListView.SmallImageList.Images.RemoveAt(listViewItem.ImageIndex);
    }
    protected virtual void UpdateListViewItemImage(ListViewItem listViewItem) {
      int i = listViewItem.ImageIndex;
      listViewItem.ImageList.Images[i] = ((IRun)listViewItem.Tag).ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    protected virtual void UpdateListViewItemText(ListViewItem listViewItem) {
      if (!listViewItem.Text.Equals(listViewItem.Tag.ToString()))
        listViewItem.Text = listViewItem.Tag.ToString();
    }
    protected virtual IEnumerable<ListViewItem> GetListViewItemsForItem(IRun item) {
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        if (((IRun)listViewItem.Tag) == item)
          yield return listViewItem;
      }
    }

    #region ListView Events
    protected virtual void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && (Content != null) && !Content.IsReadOnly && !ReadOnly;
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
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content.Remove((IRun)item.Tag);
        }
      }
    }
    protected virtual void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IRun item = (IRun)itemsListView.SelectedItems[0].Tag;
        IContentView view = MainFormManager.CreateDefaultView(item);
        if (view != null) {
          view.ReadOnly = ReadOnly;
          view.Locked = Locked;
          view.Show();
        }
      }
    }
    protected virtual void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        ListViewItem listViewItem = (ListViewItem)e.Item;
        IRun item = (IRun)listViewItem.Tag;
        DataObject data = new DataObject();
        data.SetData("Type", item.GetType());
        data.SetData("Value", item);
        if (Content.IsReadOnly || ReadOnly) {
          DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
        } else {
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
          if ((result & DragDropEffects.Move) == DragDropEffects.Move)
            Content.Remove(item);
        }
      }
    }
    protected virtual void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if (!Content.IsReadOnly && !ReadOnly && (type != null) && (typeof(IRun).IsAssignableFrom(type))) {
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
      else {
        RegisterRunEvents(e.Items);
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));
      }
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
      else {
        DeregisterRunEvents(e.Items);
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
        DeregisterRunEvents(e.OldItems);
        foreach (IRun item in e.OldItems) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            RemoveListViewItem(listViewItem);
            break;
          }
        }
        RegisterRunEvents(e.Items);
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));
      }
    }
    #endregion

    #region Item Events
    protected virtual void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemImage(listViewItem);
      }
    }
    protected virtual void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemText(listViewItem);
      }
    }
    #endregion
  }
}

