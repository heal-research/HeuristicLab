#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      // TODO: Deregister your event handlers on the Content here
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // TODO: Register your event handlers on the Content here
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // TODO: Put code here when content is null
      } else {
        // TODO: Put code here when content has been changed and is not null
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // TODO: Put code here to enable or disable controls based on whether the Content is/not null or the view is ReadOnly
    }

    #region Event Handlers
    // TODO: Put event handlers here
    #endregion

    #region Child Control Events
    protected override void addButton_Click(object sender, EventArgs e) {
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

    protected override void removeButton_Click(object sender, EventArgs e) {
      base.removeButton_Click(sender, e);

      if (treeView.SelectedNode != null) {
        var selectedItem = (HiveTask)treeView.SelectedNode.Tag;
        var parentItem = GetParentItem(selectedItem);
        if (parentItem == null) {
          Content.Remove((HiveTask)treeView.SelectedNode.Tag);
        } else {
          var experiment = parentItem.ItemTask.Item as Experiment;
          if (experiment != null) {
            experiment.Optimizers.Remove(((OptimizerTask)selectedItem.ItemTask).Item);
          }
        }
      }
    }
    #endregion

    protected override ICollection<IItemTreeNodeAction<HiveTask>> GetTreeNodeItemActions(HiveTask selectedItem) {
      return base.GetTreeNodeItemActions(selectedItem);
    }
  }
}
