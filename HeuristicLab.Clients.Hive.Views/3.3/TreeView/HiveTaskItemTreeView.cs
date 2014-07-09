#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Hive.Jobs;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Clients.Hive.Views {
  [View("HiveTask ItemTreeView")]
  [Content(typeof(ItemCollection<HiveTask>), IsDefaultView = false)]
  public partial class HiveTaskItemTreeView : ItemTreeView<HiveTask> {
    public new ItemCollection<HiveTask> Content {
      get { return (ItemCollection<HiveTask>)base.Content; }
      set { base.Content = value; }
    }

    public HiveTaskItemTreeView() {
      InitializeComponent();
    }

    protected override void AddItem() {
      IOptimizer optimizer = CreateItem<IOptimizer>();
      if (optimizer != null) {
        if (treeView.SelectedNode == null) {
          Content.Add(new OptimizerHiveTask(optimizer));
        } else {
          var experiment = ((HiveTask)treeView.SelectedNode.Tag).ItemTask.Item as Experiment;
          if (experiment != null) {
            experiment.Optimizers.Add(optimizer);
          } else {
            Content.Add(new OptimizerHiveTask(optimizer));
          }
        }
      }
    }

    protected override void RemoveItem(HiveTask item) {
      var parentItem = GetParentItem(item);
      if (parentItem == null) {
        Content.Remove(item);
      } else {
        var experiment = parentItem.ItemTask.Item as Experiment;
        if (experiment != null) {
          experiment.Optimizers.Remove(((OptimizerTask)item.ItemTask).Item);
        }
      }
    }

    protected override ICollection<IItemTreeNodeAction<HiveTask>> GetTreeNodeItemActions(HiveTask selectedItem) {
      return base.GetTreeNodeItemActions(selectedItem);
    }
  }
}
