using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(IRunCollectionConstraint))]
  public partial class RunCollectionConstraintView : ItemView {
    public RunCollectionConstraintView() {
      InitializeComponent();
    }

    public new IRunCollectionConstraint Content {
      get { return (IRunCollectionConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      cmbConstraintColumn.Items.Clear();
      cmbConstraintOperation.Items.Clear();
      chbActive.Checked = false;
      if (Content != null) {
        cmbConstraintOperation.Items.AddRange(Content.AllowedConstraintOperations.ToArray());
        if (Content.ConstraintOperation != null)
          cmbConstraintOperation.SelectedItem = Content.ConstraintOperation;
        else if (cmbConstraintOperation.Items.Count != 0)
          cmbConstraintOperation.SelectedIndex = 0;
        this.UpdateColumnComboBox();
        chbActive.Checked = Content.Active;
        this.ReadOnly = Content.Active;
      }
      SetEnabledStateOfControls();
    }

    protected virtual void UpdateColumnComboBox() {
      this.cmbConstraintColumn.Items.Clear();
      if (Content.ConstrainedValue != null) {
        this.cmbConstraintColumn.Items.AddRange(((IStringConvertibleMatrix)Content.ConstrainedValue).ColumnNames.ToArray());
        if (Content.ConstraintColumn >= 0)
          this.cmbConstraintColumn.SelectedItem = ((IStringConvertibleMatrix)Content.ConstrainedValue).ColumnNames.ElementAt(Content.ConstraintColumn);
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

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }
    protected override void OnLockedChanged() {
      base.OnLockedChanged();
      SetEnabledStateOfControls();
    }
    protected virtual void SetEnabledStateOfControls() {
      cmbConstraintColumn.Enabled = !this.ReadOnly && !this.Locked && Content != null;
      cmbConstraintOperation.Enabled = !this.ReadOnly && !this.Locked && Content != null;
      chbActive.Enabled = !this.Locked && Content != null;
    }

    protected virtual void Content_ConstrainedValueChanged(object sender, EventArgs e) {
      this.UpdateColumnComboBox();
    }

    protected virtual void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        string columnName = ((IStringConvertibleMatrix)Content.ConstrainedValue).ColumnNames.ElementAt(Content.ConstraintColumn);
        if (cmbConstraintColumn.SelectedItem.ToString() != columnName)
          cmbConstraintColumn.SelectedItem = columnName;
      }
    }
    private void cmbConstraintColumn_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content.ConstrainedValue != null) {
        Content.ConstraintColumn = ((IStringConvertibleMatrix)Content.ConstrainedValue).ColumnNames.ToList().IndexOf(cmbConstraintColumn.SelectedItem.ToString());
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
      Content.Active = chbActive.Checked;
    }
  }
}
