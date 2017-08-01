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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(IRunCollectionColumnConstraint))]
  public partial class RunCollectionColumnConstraintView : ItemView {
    public RunCollectionColumnConstraintView() {
      InitializeComponent();
    }

    public new IRunCollectionColumnConstraint Content {
      get { return (IRunCollectionColumnConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      cmbConstraintColumn.Items.Clear();
      cmbConstraintOperation.Items.Clear();

      if (Content != null) {
        cmbConstraintOperation.Items.AddRange(Content.AllowedConstraintOperations.ToArray());
        if (Content.ConstraintOperation != null)
          cmbConstraintOperation.SelectedItem = Content.ConstraintOperation;
        else if (cmbConstraintOperation.Items.Count != 0)
          cmbConstraintOperation.SelectedIndex = 0;
        UpdateColumnComboBox();
        chbActive.Checked = Content.Active;
        ReadOnly = Content.Active;
      } else
        chbActive.Checked = false;
    }

    protected virtual void UpdateColumnComboBox() {
      this.cmbConstraintColumn.Items.Clear();
      if (Content.ConstrainedValue != null) {
        this.cmbConstraintColumn.Items.AddRange(((IStringConvertibleMatrix)Content.ConstrainedValue).ColumnNames.ToArray());
        if (!string.IsNullOrEmpty(Content.ConstraintColumn))
          this.cmbConstraintColumn.SelectedItem = Content.ConstraintColumn;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      this.Content.ActiveChanged += new EventHandler(Content_ActiveChanged);
      this.Content.ConstraintOperationChanged += new EventHandler(Content_ComparisonOperationChanged);
      this.Content.ConstraintColumnChanged += new EventHandler(Content_ConstraintColumnChanged);
      this.Content.ConstrainedValueChanged += new EventHandler(Content_ConstrainedValueChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      this.Content.ActiveChanged -= new EventHandler(Content_ActiveChanged);
      this.Content.ConstraintOperationChanged -= new EventHandler(Content_ComparisonOperationChanged);
      this.Content.ConstraintColumnChanged -= new EventHandler(Content_ConstraintColumnChanged);
      this.Content.ConstrainedValueChanged += new EventHandler(Content_ConstrainedValueChanged);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      cmbConstraintColumn.Enabled = !this.ReadOnly && Content != null;
      cmbConstraintOperation.Enabled = !this.ReadOnly && Content != null;
      chbActive.Enabled = !this.Locked && Content != null;
    }

    protected virtual void Content_ConstrainedValueChanged(object sender, EventArgs e) {
      this.UpdateColumnComboBox();
    }

    protected virtual void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        if (cmbConstraintColumn.SelectedItem.ToString() != Content.ConstraintColumn)
          cmbConstraintColumn.SelectedItem = Content.ConstraintColumn;
      }
    }
    private void cmbConstraintColumn_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        Content.ConstraintColumn = (string)cmbConstraintColumn.SelectedItem;
      }
    }

    protected virtual void Content_ComparisonOperationChanged(object sender, EventArgs e) {
      if (Content.ConstraintOperation != (ConstraintOperation)this.cmbConstraintOperation.SelectedItem)
        this.cmbConstraintOperation.SelectedItem = Content.ConstraintOperation;
    }
    protected virtual void cmbComparisonOperation_SelectedIndexChanged(object sender, EventArgs e) {
      Content.ConstraintOperation = (ConstraintOperation)this.cmbConstraintOperation.SelectedItem;
    }

    protected virtual void Content_ActiveChanged(object sender, EventArgs e) {
      if (this.chbActive.Checked != Content.Active)
        this.chbActive.Checked = Content.Active;
      this.ReadOnly = Content.Active;
    }
    protected virtual void chbActive_CheckedChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.Active = chbActive.Checked;
    }
  }
}
