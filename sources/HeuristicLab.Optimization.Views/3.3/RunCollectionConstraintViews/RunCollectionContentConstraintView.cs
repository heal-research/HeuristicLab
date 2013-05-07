#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionContentConstraint), true)]
  public partial class RunCollectionContentConstraintView : ItemView {
    public RunCollectionContentConstraintView() {
      InitializeComponent();
      runsView.ShowDetails = false;
    }

    public new RunCollectionContentConstraint Content {
      get { return (RunCollectionContentConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        chbActive.Checked = Content.Active;
        ReadOnly = Content.Active;
        runsView.RunCollection = Content.ConstrainedValue;
        runsView.Content = Content.ConstraintData;
      } else {
        chbActive.Checked = false;
        runsView.RunCollection = null;
        runsView.Content = null;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ActiveChanged += new EventHandler(Content_ActiveChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ActiveChanged -= new EventHandler(Content_ActiveChanged);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chbActive.Enabled = !Locked && Content != null;
      runsView.Enabled = !Locked && Content != null;
    }

    protected virtual void Content_ActiveChanged(object sender, EventArgs e) {
      chbActive.Checked = Content.Active;
      ReadOnly = Content.Active;
    }
    protected virtual void chbActive_CheckedChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.Active = chbActive.Checked;
    }

    private class RunSetView : ItemSetView<IRun> {
      public RunCollection RunCollection { get; set; }

      public RunSetView()
        : base() {
        addButton.Enabled = false;
      }

      protected override void SetEnabledStateOfControls() {
        base.SetEnabledStateOfControls();
        addButton.Enabled = false;
      }

      protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
        base.itemsListView_DragEnter(sender, e);
        if (RunCollection != null) {
          var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          foreach (var run in dropData.GetObjectGraphObjects().OfType<IRun>())
            validDragOperation = validDragOperation && RunCollection.Contains(run);
        }
      }

      protected override void itemsListView_DragOver(object sender, DragEventArgs e) {
        e.Effect = DragDropEffects.None;
        if (validDragOperation && !draggedItemsAlreadyContained) {
          e.Effect = DragDropEffects.Link;
        }
      }
    }
  }
}
