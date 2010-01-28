using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;
using System.Collections;

namespace HeuristicLab.MainForm.Test {
  [Content(typeof(List<string>), true)]
  public partial class FormView1 : FormView2<MenuItem> {
    public FormView1() {
      InitializeComponent();
    }

    public FormView1(List<string> list)
      : this() {
    }
  }
}
