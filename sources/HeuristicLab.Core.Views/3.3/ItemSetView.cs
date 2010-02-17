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

using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [Content(typeof(ItemSet<>), true)]
  [Content(typeof(IObservableSet<>), false)]
  public partial class ItemSetView<T> : ItemCollectionView<T> where T : class, IItem {
    public new IObservableSet<T> Content {
      get { return (IObservableSet<T>)base.Content; }
      set { base.Content = value; }
    }

    private Dictionary<T, ListViewItem> listViewItemDictionary;
    protected Dictionary<T, ListViewItem> ListViewItemDictionary {
      get { return listViewItemDictionary; }
    }

    public ItemSetView() {
      listViewItemDictionary = new Dictionary<T, ListViewItem>();
      InitializeComponent();
      Caption = "Item Set";
    }
    public ItemSetView(IObservableSet<T> content)
      : this() {
      Content = content;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Item Set";
      if (Content != null)
        Caption += " (" + Content.GetType().Name + ")";
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
  }
}
