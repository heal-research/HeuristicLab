using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Data;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionComparisonConstraint), true)]
  public partial class RunCollectionComparisonConstraintView : RunCollectionConstraintView {
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
        if (Content.ConstraintColumn >= 0) {
          this.cmbConstraintColumn.SelectedItem = (matrix.ColumnNames.ElementAt(Content.ConstraintColumn));
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
      txtConstraintData.ReadOnly = Content == null || this.ReadOnly;
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
