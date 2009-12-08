using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimizer {
  internal partial class OptimizerMainForm : DockingMainForm {
    public OptimizerMainForm()
      : base() {
      InitializeComponent();
    }

    public OptimizerMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
    }

    private void OptimizerMainForm_Load(object sender, EventArgs e) {
      Title = "HeuristicLab Optimizer " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
  }
}
