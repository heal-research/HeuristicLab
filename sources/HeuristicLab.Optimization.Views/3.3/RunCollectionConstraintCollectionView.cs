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

using System;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  [View("ConstraintCollection View")]
  [Content(typeof(RunCollectionConstraintCollection), true)]
  [Content(typeof(IItemCollection<IRunCollectionConstraint>), false)]
  public partial class RunCollectionConstraintCollectionView : ItemCollectionView<IRunCollectionConstraint> {
    public RunCollectionConstraintCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "RunCollection Constraints";
    }

    protected override IRunCollectionConstraint CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select RunCollection Constraint";
        typeSelectorDialog.TypeSelector.Caption = "Available Constraints";
        typeSelectorDialog.TypeSelector.Configure(typeof(IRunCollectionConstraint), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (IRunCollectionConstraint)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }

    protected override ListViewItem CreateListViewItem(IRunCollectionConstraint item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      if (item.Active)
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Bold);
      else
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
      return listViewItem;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      foreach (IRunCollectionConstraint constraint in Content)
        RegisterConstraintEvents(constraint);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      foreach (IRunCollectionConstraint constraint in Content)
        DeregisterConstraintEvents(constraint);
    }
    protected override void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      base.Content_ItemsAdded(sender, e);
      foreach (IRunCollectionConstraint constraint in e.Items)
        RegisterConstraintEvents(constraint);

    }
    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      base.Content_ItemsRemoved(sender, e);
      foreach (IRunCollectionConstraint constraint in e.Items)
        DeregisterConstraintEvents(constraint);
    }
    protected override void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      base.Content_CollectionReset(sender, e);
      foreach (IRunCollectionConstraint constraint in e.OldItems)
        RegisterConstraintEvents(constraint);
      foreach (IRunCollectionConstraint constraint in e.Items)
        DeregisterConstraintEvents(constraint);
    }

    protected virtual void RegisterConstraintEvents(IRunCollectionConstraint constraint) {
      constraint.ActiveChanged += new EventHandler(constraint_ActiveChanged);
    }
    protected virtual void DeregisterConstraintEvents(IRunCollectionConstraint constraint) {
      constraint.ActiveChanged -= new EventHandler(constraint_ActiveChanged);
    }
    protected virtual void constraint_ActiveChanged(object sender, EventArgs e) {
      IRunCollectionConstraint constraint = (IRunCollectionConstraint)sender;
      foreach (ListViewItem listViewItem in GetListViewItemsForItem(constraint)) {
        if (constraint.Active)
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Bold);
        else
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
      }
      this.AdjustListViewColumnSizes();
    }
  }
}
