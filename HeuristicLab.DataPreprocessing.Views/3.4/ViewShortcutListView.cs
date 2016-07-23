#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("ViewShortcutCollection View")]
  [Content(typeof(IItemList<IViewShortcut>), true)]
  public partial class ViewShortcutListView : ItemListView<IViewShortcut> {

    public ViewShortcutListView() {
      InitializeComponent();
      itemsGroupBox.Text = "View Shortcuts";
    }

    //Open item in new tab on double click
    //Clone chart items
    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IViewShortcut item = itemsListView.SelectedItems[0].Tag as IViewShortcut;
        if (item != null) {

          if (item is IViewChartShortcut)
            item = (IViewChartShortcut)item.Clone(new Cloner());

          IContentView view = MainFormManager.MainForm.ShowContent(item);
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Locked = Locked;
          }
        }
      }
    }
  }
}
