using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Common;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionTypeCompatibilityConstraint), true)]
  public partial class RunCollectionTypeCompatibilityConstraintView : RunCollectionConstraintView {
    public RunCollectionTypeCompatibilityConstraintView() {
      InitializeComponent();
      cmbType.DisplayMember = "Name";
    }

    public new RunCollectionTypeCompatibilityConstraint Content {
      get { return (RunCollectionTypeCompatibilityConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      base.Content_ConstraintColumnChanged(sender, e);
      UpdateTypeColumn();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateTypeColumn();
    }

    protected virtual void UpdateTypeColumn() {
      cmbType.Items.Clear();
      if (Content != null && Content.ConstrainedValue != null) {
        foreach (Type t in Content.ConstrainedValue.GetDataType(cmbConstraintColumn.SelectedItem.ToString()))
          cmbType.Items.Add(t);
        if (Content.ConstraintData != null)
          cmbType.SelectedItem = Content.ConstraintData;
        else if (cmbType.Items.Count > 0)
          cmbType.SelectedIndex = 0;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      cmbType.Enabled = Content != null && !this.ReadOnly;
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.ConstraintData = (Type)cmbType.SelectedItem;
    }

    private struct ComboBoxItem {
      public ComboBoxItem(string name, Type type) {
        this.Name = name;
        this.Type = type;
      }
      public string Name;
      public Type Type;
    }
  }
}
