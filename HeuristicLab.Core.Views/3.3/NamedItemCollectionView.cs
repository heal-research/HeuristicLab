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

using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("NamedItemCollection View")]
  [Content(typeof(NamedItemCollection<>), true)]
  public partial class NamedItemCollectionView<T> : ItemCollectionView<T> where T : class, INamedItem {
    public new IKeyedItemCollection<string, T> Content {
      get { return (IKeyedItemCollection<string, T>)base.Content; }
      set { base.Content = value; }
    }

    public NamedItemCollectionView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsReplaced -= new CollectionItemsChangedEventHandler<T>(Content_ItemsReplaced);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsReplaced += new CollectionItemsChangedEventHandler<T>(Content_ItemsReplaced);
    }

    protected override void OnContentChanged() {
      string selectedName = null;
      if ((itemsListView.SelectedItems.Count == 1) && (itemsListView.SelectedItems[0].Tag != null))
        selectedName = ((T)itemsListView.SelectedItems[0].Tag).Name;
      base.OnContentChanged();
      if (selectedName != null) {
        foreach (ListViewItem item in itemsListView.Items) {
          if ((item.Tag != null) && (((T)item.Tag).Name.Equals(selectedName)))
            item.Selected = true;
        }
      }
    }

    protected override T CreateItem() {
      T item = base.CreateItem();
      if (item != null) {
        item.Name = GetUniqueName(item.Name);
      }
      return item;
    }
    protected override ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      if ((item != null) && !string.IsNullOrEmpty(item.Description)) {
        listViewItem.ToolTipText = item.ItemName + ": " + item.Description;
      }
      return listViewItem;
    }

    #region ListView Events
    protected override void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      base.itemsListView_DragEnterOver(sender, e);
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((item == null) || (Content.ContainsKey(item.Name)))
          e.Effect = DragDropEffects.None;
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsReplaced), sender, e);
      else {
        foreach (T item in e.Items) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
            UpdateListViewItemText(listViewItem);
        }
        AdjustListViewColumnSizes();
      }
    }
    #endregion

    #region Helpers
    protected virtual string GetUniqueName(string originalName) {
      if (!Content.ContainsKey(originalName))
        return originalName;
      else {
        string name = null;
        int index = 0;
        do {
          index++;
          name = originalName + index.ToString();
        } while (Content.ContainsKey(name));
        return name;
      }
    }
    #endregion
  }
}
