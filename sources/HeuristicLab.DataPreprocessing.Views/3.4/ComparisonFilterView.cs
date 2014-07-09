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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.DataPreprocessing.Filter;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Comparison Filter View")]
  [Content(typeof(ComparisonFilter), false)]
  public partial class ComparisonFilterView : ItemView {
    public ComparisonFilterView() {
      InitializeComponent();
    }

    public new ComparisonFilter Content {
      get { return (ComparisonFilter)base.Content; }
      set { base.Content = value; }
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      cbAttr.Items.Clear(); //cmbConstraintColumn.Items.Clear();
      cbFilterOperation.Items.Clear(); //cmbConstraintOperation.Items.Clear();
      tbFilterData.Text = string.Empty;
      if (Content != null) {
        cbFilterOperation.Items.AddRange(Content.AllowedConstraintOperations.ToArray());
        if (Content.ConstraintOperation != null)
          cbFilterOperation.SelectedItem = Content.ConstraintOperation;
        else if (cbFilterOperation.Items.Count != 0)
          cbFilterOperation.SelectedIndex = 0;
        UpdateColumnComboBox();
        ReadOnly = Content.Active;
        if (Content.ConstraintData != null) {
          tbFilterData.Text = Content.ConstraintData.GetValue();
        } else {
          this.Content_ConstraintColumnChanged(cbAttr, EventArgs.Empty); // TODO
        }
      }
      if (Content == null || Content.ConstraintData == null) {
        tbFilterData.Text = string.Empty;
      } else {
        tbFilterData.Text = Content.ConstraintData.GetValue();
      }
    }

    protected virtual void UpdateColumnComboBox() {
      this.cbAttr.Items.Clear();
      if (Content.ConstrainedValue != null) {
        this.cbAttr.Items.AddRange(Content.ConstrainedValue.VariableNames.ToArray<string>());
        this.cbAttr.SelectedItem = this.cbAttr.Items[Content.ConstraintColumn];
        cbAttr.SelectedItem = Content.ConstraintColumn;
        if (Content.ConstraintData != null)
          tbFilterData.Text = Content.ConstraintData.GetValue();
        else
          this.Content_ConstraintColumnChanged(cbAttr, EventArgs.Empty);
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      this.Content.ActiveChanged += new EventHandler(Content_ActiveChanged);
      this.Content.ConstraintOperationChanged += new EventHandler(Content_ComparisonOperationChanged);
      this.Content.ConstraintColumnChanged += new EventHandler(Content_ConstraintColumnChanged);
      this.Content.ConstrainedValueChanged += new EventHandler(Content_ConstrainedValueChanged);
      this.Content.ConstraintDataChanged += new EventHandler(Content_ConstrainedDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      this.Content.ActiveChanged -= new EventHandler(Content_ActiveChanged);
      this.Content.ConstraintOperationChanged -= new EventHandler(Content_ComparisonOperationChanged);
      this.Content.ConstraintColumnChanged -= new EventHandler(Content_ConstraintColumnChanged);
      this.Content.ConstrainedValueChanged -= new EventHandler(Content_ConstrainedValueChanged);
      this.Content.ConstraintDataChanged -= new EventHandler(Content_ConstrainedDataChanged);
    }

    protected virtual void Content_ConstrainedValueChanged(object sender, EventArgs e) {
      this.UpdateColumnComboBox();
    }

    private void Content_ConstrainedDataChanged(object sender, EventArgs e) {
      if (Content.ConstraintData != null)
        tbFilterData.Text = Content.ConstraintData.GetValue();
      else
        tbFilterData.Text = string.Empty;
    }

    protected virtual void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        if (cbAttr.Items.IndexOf(cbAttr.SelectedItem) != Content.ConstraintColumn) {
          cbAttr.SelectedItem = this.cbAttr.Items[Content.ConstraintColumn];
        }
      }
      if (Content.ConstraintData == null) {
        this.Content.ConstraintData = CreateStringConvertibleValue(cbAttr.SelectedIndex);
      }
    }


    protected virtual void Content_ComparisonOperationChanged(object sender, EventArgs e) {
      if (Content.ConstraintOperation != (ConstraintOperation)this.cbFilterOperation.SelectedItem)
        this.cbFilterOperation.SelectedItem = Content.ConstraintOperation;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      cbAttr.Enabled = Content != null && !Content.Active;
      cbFilterOperation.Enabled = Content != null && !Content.Active;
      tbFilterData.Enabled = Content != null && !Content.Active;
    }

    private void cbAttr_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        Content.ConstraintColumn = Content.ConstrainedValue.GetColumnIndex(cbAttr.SelectedItem.ToString());
      }
    }

    private void cbFilterOperation_SelectedIndexChanged(object sender, EventArgs e) {
      Content.ConstraintOperation = (ConstraintOperation)this.cbFilterOperation.SelectedItem;
    }

    protected virtual void Content_ActiveChanged(object sender, EventArgs e) {
      this.ReadOnly = !Content.Active;
      SetEnabledStateOfControls();
      Refresh(); ResumeRepaint(true);
    }

    private void tbFilterData_Validated(object sender, EventArgs e) {
      IStringConvertibleValue value = CreateStringConvertibleValue(cbAttr.SelectedIndex);
      value.SetValue(tbFilterData.Text);
      Content.ConstraintData = value;
    }

    private IStringConvertibleValue CreateStringConvertibleValue(int columnIndex) {
      IStringConvertibleValue value;
      if (Content.ConstrainedValue.VariableHasType<double>(columnIndex)) {
        value = new DoubleValue();
      } else if (Content.ConstrainedValue.VariableHasType<String>(columnIndex)) {
        value = new StringValue();
      } else if (Content.ConstrainedValue.VariableHasType<DateTime>(columnIndex)) {
        value = new DateTimeValue();
      } else {
        throw new ArgumentException("unsupported type");
      }
      return value;
    }

    private void tbFilterData_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      string errorMessage = string.Empty;
      if (!Content.ConstraintData.Validate(tbFilterData.Text, out errorMessage)) {
        errorProvider.SetError(tbFilterData, errorMessage);
        e.Cancel = true;
      } else
        errorProvider.Clear();
    }

  }
}
