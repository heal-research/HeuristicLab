using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public partial class FormView : ViewBase {
    private int[] array;
    public FormView() {
      InitializeComponent();
    }

    public FormView(IMainForm mainform)
      : base(mainform) {
    }
  }
}
