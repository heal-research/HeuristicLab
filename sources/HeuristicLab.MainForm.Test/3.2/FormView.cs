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
  [Content(typeof(IList),true)]
  public partial class FormView1 : FormView2 {
    public FormView1() {
      InitializeComponent();
    }
  }
}
