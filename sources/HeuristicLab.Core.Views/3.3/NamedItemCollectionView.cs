#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  public partial class NamedItemCollectionView<T> : ItemCollectionView<T> where T : class, INamedItem {
    public IObservableKeyedCollection<string, T> NamedItemCollection {
      get { return (IObservableKeyedCollection<string, T>)Object; }
      set { base.Object = value; }
    }

    private Dictionary<T, ListViewItem> listViewItemDictionary;
    protected Dictionary<T, ListViewItem> ListViewItemDictionary {
      get { return listViewItemDictionary; }
    }

    public NamedItemCollectionView() {
      listViewItemDictionary = new Dictionary<T, ListViewItem>();
      InitializeComponent();
      Caption = "Named Item Collection";
    }

    protected override void DeregisterObjectEvents() {
      NamedItemCollection.ItemsReplaced -= new CollectionItemsChangedEventHandler<T>(NamedItemCollection_ItemsReplaced);
      base.DeregisterObjectEvents();
    }
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      NamedItemCollection.ItemsReplaced += new CollectionItemsChangedEventHandler<T>(NamedItemCollection_ItemsReplaced);
    }

    protected override T CreateItem() {
      T item = base.CreateItem();
      if (item != null) {
        item.Name = GetUniqueName(item.Name);
      }
      return item;
    }
    protected override void AddListViewItem(ListViewItem listViewItem) {
      ListViewItemDictionary.Add((T)listViewItem.Tag, listViewItem);
      base.AddListViewItem(listViewItem);
    }
    protected override void RemoveListViewItem(ListViewItem listViewItem) {
      base.RemoveListViewItem(listViewItem);
      ListViewItemDictionary.Remove((T)listViewItem.Tag);
    }
    protected override IEnumerable<ListViewItem> GetListViewItemsForItem(T item) {
      return new ListViewItem[] { listViewItemDictionary[item] };
    }
 
    #region ListView Events
    protected override void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      base.itemsListView_DragEnterOver(sender, e);
      if (e.Effect != DragDropEffects.None) {
        T item = e.Data.GetData("Value") as T;
        if ((item == null) || (NamedItemCollection.ContainsKey(item.Name)))
          e.Effect = DragDropEffects.None;
      }
    }
    #endregion

    #region NamedItemCollection Events
    protected virtual void NamedItemCollection_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(NamedItemCollection_ItemsReplaced), sender, e);
      else {
        foreach (T item in e.Items) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
            UpdateListViewItem(listViewItem);
        }
      }
    }
    #endregion

    #region Helpers
    protected virtual string GetUniqueName(string originalName) {
      if (!NamedItemCollection.ContainsKey(originalName))
        return originalName;
      else {
        string name = null;
        int index = 0;
        do {
          index++;
          name = originalName + index.ToString();
        } while (NamedItemCollection.ContainsKey(name));
        return name;
      }
    }
    #endregion
  }
}
