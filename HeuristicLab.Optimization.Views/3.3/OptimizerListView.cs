#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  [View("OptimizerList View")]
  [Content(typeof(OptimizerList), true)]
  [Content(typeof(IItemList<IOptimizer>), false)]
  public partial class OptimizerListView : ItemListView<IOptimizer> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OptimizerListView() {
      InitializeComponent();
      itemsGroupBox.Text = "Optimizers";
    }

    protected override IOptimizer CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Optimizer";
        typeSelectorDialog.TypeSelector.Caption = "Available Optimizers";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOptimizer), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (IOptimizer)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }

    protected override void DeregisterItemEvents(IOptimizer item) {
      var batchRun = item as BatchRun;
      if (batchRun != null) {
        batchRun.RepetitionsChanged -= BatchRun_UpdateText;
        batchRun.RepetetionsCounterChanged -= BatchRun_UpdateText;
      }
      base.DeregisterItemEvents(item);
    }

    protected override void RegisterItemEvents(IOptimizer item) {
      base.RegisterItemEvents(item);
      var batchRun = item as BatchRun;
      if (batchRun != null) {
        batchRun.RepetitionsChanged += BatchRun_UpdateText;
        batchRun.RepetetionsCounterChanged += BatchRun_UpdateText;
      }
    }

    protected override ListViewItem CreateListViewItem(IOptimizer item) {
      var listViewItem = base.CreateListViewItem(item);
      var batchRun = item as BatchRun;
      if (batchRun != null) {
        listViewItem.Text += string.Format(" {0}/{1}", batchRun.RepetitionsCounter, batchRun.Repetitions);
      }
      return listViewItem;
    }

    protected override void UpdateListViewItemText(ListViewItem listViewItem) {
      base.UpdateListViewItemText(listViewItem);
      var batchRun = listViewItem.Tag as BatchRun;
      if (batchRun != null) {
        listViewItem.Text = batchRun.ToString() + string.Format(" {0}/{1}", batchRun.RepetitionsCounter, batchRun.Repetitions);
      }
    }

    protected virtual void BatchRun_UpdateText(object sender, EventArgs eventArgs) {
      if (sender is BatchRun) {
        foreach (var item in GetListViewItemsForItem(sender as BatchRun))
          UpdateListViewItemText(item);
      }
    }
  }
}
