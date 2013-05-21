#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a list of checked items.
  /// </summary>
  [View("CheckedItemList View")]
  [Content(typeof(CheckedItemList<>), true)]
  [Content(typeof(ICheckedItemList<>), true)]
  public partial class CheckedItemListView<T> : ItemListView<T> where T : class, IItem {
    public new ICheckedItemList<T> Content {
      get { return (ICheckedItemList<T>)base.Content; }
      set { base.Content = value; }
    }

    public CheckedItemListView()
      : base() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      Content.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
      base.RegisterContentEvents();
    }
    protected override void DeregisterContentEvents() {
      Content.CheckedItemsChanged -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        SetNumberOfCheckItems();
      }
    }

    private Color backupColor = Color.Empty;
    protected override void SetEnabledStateOfControls() {
      if (backupColor == Color.Empty) backupColor = base.itemsListView.BackColor;
      base.SetEnabledStateOfControls();
      if (ReadOnly || Locked)
        base.itemsListView.BackColor = ListView.DefaultBackColor;
      else
        base.itemsListView.BackColor = backupColor;
    }

    protected override ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      listViewItem.Checked = Content.ItemChecked(item);
      return listViewItem;
    }

    #region ListView Events
    private bool doubleClick;
    protected virtual void itemsListView_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (doubleClick) {
        e.NewValue = e.CurrentValue;
        doubleClick = false;
      } else {
        var checkedItem = (T)itemsListView.Items[e.Index].Tag;
        bool check = e.NewValue == CheckState.Checked;
        if (Content.ItemChecked(checkedItem) != check) {
          if (!ReadOnly && !Locked) Content.SetItemCheckedState(checkedItem, check);
          else e.NewValue = e.CurrentValue;
        }
      }
    }

    protected void itemsListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      if (e.Clicks > 1)
        doubleClick = true;
    }

    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (Locked || ReadOnly) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as ICheckedItemList<T>;
      if (data != null)
        validDragOperation = Content.Select(x => x.ToString()).SequenceEqual(data.Select(x => x.ToString()));
      else
        base.itemsListView_DragEnter(sender, e);
    }

    protected override void itemsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!validDragOperation) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as ICheckedItemList<T>;
      if (data != null)
        e.Effect = DragDropEffects.Copy;
      else
        base.itemsListView_DragOver(sender, e);
    }

    protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect == DragDropEffects.None) return;
      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as ICheckedItemList<T>;
      if (data != null) {
        for (int i = 0; i < Content.Count; i++) {
          Content.SetItemCheckedState(Content[i], data.ItemChecked(data[i]));
        }
      } else
        base.itemsListView_DragDrop(sender, e);
    }
    #endregion

    #region Content Events
    protected virtual void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged), sender, e);
      else {
        foreach (var item in e.Items) {
          if (itemsListView.Items[item.Index].Checked != Content.ItemChecked(item.Value))
            itemsListView.Items[item.Index].Checked = Content.ItemChecked(item.Value);
        }
        SetNumberOfCheckItems();
      }
    }
    protected override void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Content_CollectionReset(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Content_ItemsAdded(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Content_ItemsMoved(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Content_ItemsRemoved(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      base.Content_ItemsReplaced(sender, e);
      SetNumberOfCheckItems();
    }
    #endregion

    private void SetNumberOfCheckItems() {
      if (InvokeRequired) {
        Invoke((Action)SetNumberOfCheckItems);
      } else {
        this.itemsGroupBox.Text = String.Format("Items (Checked: {0}/{1})", Content.CheckedItems.Count(), Content.Count);
      }
    }
  }
}
