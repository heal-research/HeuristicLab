#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ItemSet View")]
  [Content(typeof(ItemSet<>), true)]
  [Content(typeof(IItemSet<>), false)]
  public partial class ItemSetView<T> : ItemCollectionView<T> where T : class, IItem {
    public new IItemSet<T> Content {
      get { return (IItemSet<T>)base.Content; }
      set { base.Content = value; }
    }

    public ItemSetView() {
      InitializeComponent();
    }

    protected override void itemsListView_DragEnterOver(object sender, DragEventArgs e) {
      base.itemsListView_DragEnterOver(sender, e);
      if (e.Effect == DragDropEffects.Link || e.Effect == DragDropEffects.Move) {
        T item = e.Data.GetData("Value") as T;
        if (Content.Contains(item)) e.Effect = DragDropEffects.None;
      }
    }
  }
}
