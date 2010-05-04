using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionEqualityConstraint),true)]
  public partial class RunCollectionEqualityConstraintView : RunCollectionConstraintView {
    public RunCollectionEqualityConstraintView() {
      InitializeComponent();
    }

    public new RunCollectionEqualityConstraint Content {
      get { return (RunCollectionEqualityConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null || Content.ConstraintData == null)
        this.txtConstraintData.Text = string.Empty;
      else
        this.txtConstraintData.Text = Content.ConstraintData.ToString();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      txtConstraintData.ReadOnly = Content == null || this.ReadOnly ;
    }

    private void txtConstraintData_TextChanged(object sender, EventArgs e) {
      Content.ConstraintData = txtConstraintData.Text;
    }
  }
}
