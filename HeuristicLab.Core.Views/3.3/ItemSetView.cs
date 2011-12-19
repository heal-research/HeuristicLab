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

using System.Collections;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ItemSet View")]
  [Content(typeof(ItemSet<>), true)]
  [Content(typeof(IItemSet<>), false)]
  public partial class ItemSetView<T> : ItemCollectionView<T> where T : class, IItem {
    protected bool draggedItemsAlreadyContained;

    public new IItemSet<T> Content {
      get { return (IItemSet<T>)base.Content; }
      set { base.Content = value; }
    }

    public ItemSetView() {
      InitializeComponent();
    }

    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      base.itemsListView_DragEnter(sender, e);
      draggedItemsAlreadyContained = false;
      if (validDragOperation) {
        if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is T) {
          draggedItemsAlreadyContained = Content.Contains((T)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat));
        } else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
          IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          foreach (object item in items)
            draggedItemsAlreadyContained = draggedItemsAlreadyContained || Content.Contains((T)item);
        }
      }
    }
    protected override void itemsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if (((e.KeyState & 32) == 32) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Link;  // ALT key
        else if (((e.KeyState & 4) == 4) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Link;
      }
    }
  }
}
