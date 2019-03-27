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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("ResultCollection View")]
  [Content(typeof(ResultCollection), true)]
  [Content(typeof(IKeyedItemCollection<string, IResult>), false)]
  public partial class ResultCollectionView : NamedItemCollectionView<IResult> {

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public ResultCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "Results";
      viewHost.ViewsLabelVisible = false;
    }

    protected override IResult CreateItem() {
      return null;
    }

    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IResult result = itemsListView.SelectedItems[0].Tag as IResult;
        if (result != null) {
          IContentView view = MainFormManager.MainForm.ShowContent(result, typeof(ResultView));
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Locked = Locked;
          }
        }
      }
    }

    protected override void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
        return;
      }

      base.Item_ToStringChanged(sender, e);
      Application.DoEvents();
    }
  }
}