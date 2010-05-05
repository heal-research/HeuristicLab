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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using System.Drawing;

namespace HeuristicLab.Core.Views {
  [View("ConstraintCollection View")]
  [Content(typeof(RunCollectionConstraintCollection), true)]
  [Content(typeof(IItemCollection<IRunCollectionConstraint>), false)]
  public partial class RunCollectionConstraintCollectionView : ItemCollectionView<IRunCollectionConstraint> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public RunCollectionConstraintCollectionView() {
      InitializeComponent();
      Caption = "RunCollectionConstraintCollection";
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
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
      return null;
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
      IRunCollectionConstraint constraint = sender as IRunCollectionConstraint;
      if (constraint != null) {
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(constraint)) {
          if (constraint.Active)
            listViewItem.Font = new Font(listViewItem.Font, FontStyle.Bold);
          else
            listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
        }
      }
      this.AdjustListViewColumnSizes();
    }
  }
}
