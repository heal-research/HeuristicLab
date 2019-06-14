#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("CheckedItemCollection View")]
  [Content(typeof(ICheckedItemCollection<>), true)]
  [Content(typeof(CheckedItemCollection<>), true)]
  [Content(typeof(ReadOnlyCheckedItemCollection<>), true)]
  public partial class CheckedItemCollectionView<T> : ItemCollectionView<T> where T : class, IItem {
    private bool suppressCheckedEvents;

    public new ICheckedItemCollection<T> Content {
      get { return (ICheckedItemCollection<T>)base.Content; }
      set { base.Content = value; }
    }

    public CheckedItemCollectionView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        SetNumberOfCheckItems();
      }
    }

    protected override void RegisterContentEvents() {
      Content.CheckedItemsChanged += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(Content_CheckedItemsChanged);
      base.RegisterContentEvents();
    }

    protected override void DeregisterContentEvents() {
      Content.CheckedItemsChanged -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(Content_CheckedItemsChanged);
      base.DeregisterContentEvents();
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
        bool check = e.NewValue == CheckState.Checked;
        var checkedItem = (T)itemsListView.Items[e.Index].Tag;
        if (Content.ItemChecked(checkedItem) == check) return;

        suppressCheckedEvents = true;
        try {
          if (itemsListView.SelectedIndices.Count > 1
            && itemsListView.SelectedIndices.Contains(e.Index)) {
            if (!ReadOnly && !Locked) Content.SetItemCheckedState(itemsListView.SelectedItems.Cast<ListViewItem>().Select(x => (T)x.Tag), check);
            else e.NewValue = e.CurrentValue;
          } else {
            if (!ReadOnly && !Locked) Content.SetItemCheckedState(checkedItem, check);
            else e.NewValue = e.CurrentValue;
          }
        } finally { suppressCheckedEvents = false; }
      }
    }
    protected void itemsListView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Clicks > 1)
        doubleClick = true;
    }
    #endregion

    #region Content Events
    protected virtual void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_CheckedItemsChanged), sender, e);
      else {
        if (!suppressCheckedEvents) UpdateCheckedItemState(e.Items);
        SetNumberOfCheckItems();
      }
    }
    protected override void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      base.Content_CollectionReset(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      base.Content_ItemsAdded(sender, e);
      SetNumberOfCheckItems();
    }
    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      base.Content_ItemsRemoved(sender, e);
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

    private void UpdateCheckedItemState(IEnumerable<T> items) {
      itemsListView.BeginUpdate();
      try {
        foreach (T item in items) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item)) {
            var isChecked = Content.ItemChecked(item);
            if (listViewItem.Checked != isChecked)
              listViewItem.Checked = isChecked;
          }
        }
      } finally { itemsListView.EndUpdate(); itemsListView.Refresh(); }
    }
  }
}
