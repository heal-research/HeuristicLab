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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionComparisonConstraint), true)]
  public partial class RunCollectionComparisonConstraintView : RunCollectionColumnConstraintView {
    public RunCollectionComparisonConstraintView() {
      InitializeComponent();
    }

    public new RunCollectionComparisonConstraint Content {
      get { return (RunCollectionComparisonConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null || Content.ConstraintData == null)
        this.txtConstraintData.Text = string.Empty;
      else
        this.txtConstraintData.Text = Content.ConstraintData.GetValue();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ConstraintDataChanged += new EventHandler(Content_ConstraintDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ConstraintDataChanged -= new EventHandler(Content_ConstraintDataChanged);
    }

    protected override void UpdateColumnComboBox() {
      this.cmbConstraintColumn.Items.Clear();
      if (Content.ConstrainedValue != null) {
        IStringConvertibleMatrix matrix = (IStringConvertibleMatrix)Content.ConstrainedValue;
        foreach (string columnName in matrix.ColumnNames) {
          IEnumerable<Type> dataTypes = Content.ConstrainedValue.GetDataType(columnName);
          if (dataTypes.Count() == 1) {
            Type dataType = dataTypes.First();
            if (typeof(IStringConvertibleValue).IsAssignableFrom(dataType) && typeof(IComparable).IsAssignableFrom(dataType))
              this.cmbConstraintColumn.Items.Add(columnName);
          }
        }
        if (!string.IsNullOrEmpty(Content.ConstraintColumn)) {
          this.cmbConstraintColumn.SelectedItem = Content.ConstraintColumn;
          if (Content.ConstraintData != null)
            txtConstraintData.Text = Content.ConstraintData.GetValue();
          else
            this.Content_ConstraintColumnChanged(cmbConstraintColumn, EventArgs.Empty);
        }
      }
    }

    protected override void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      base.Content_ConstraintColumnChanged(sender, e);
      this.Content.ConstraintData = (IStringConvertibleValue)Activator.CreateInstance(Content.ConstrainedValue.GetDataType(cmbConstraintColumn.SelectedItem.ToString()).First());
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      txtConstraintData.Enabled = Content != null;
      txtConstraintData.ReadOnly = ReadOnly;
    }

    protected virtual void Content_ConstraintDataChanged(object sender, EventArgs e) {
      if (Content.ConstraintData != null)
        txtConstraintData.Text = Content.ConstraintData.GetValue();
      else
        txtConstraintData.Text = string.Empty;
    }

    private void txtConstraintData_Validated(object sender, EventArgs e) {
      IStringConvertibleValue value = (IStringConvertibleValue)Activator.CreateInstance(Content.ConstrainedValue.GetDataType(cmbConstraintColumn.SelectedItem.ToString()).First());
      value.SetValue(txtConstraintData.Text);
      Content.ConstraintData = value;
    }

    private void txtConstraintData_Validating(object sender, CancelEventArgs e) {
      string errorMessage = string.Empty;
      if (!Content.ConstraintData.Validate(txtConstraintData.Text, out errorMessage)) {
        errorProvider.SetError(txtConstraintData, errorMessage);
        e.Cancel = true;
      } else
        errorProvider.Clear();
    }
  }
}
