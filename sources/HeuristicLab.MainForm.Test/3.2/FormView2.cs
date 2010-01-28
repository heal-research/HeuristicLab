using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  [Content(typeof(ICollection<>),true)]
  public partial class FormView2<T> : View where T: IMenuItem {
    public FormView2() {
      InitializeComponent();
    }
    public FormView2(ICollection<T> x) {
      InitializeComponent();
    }
  }
}
