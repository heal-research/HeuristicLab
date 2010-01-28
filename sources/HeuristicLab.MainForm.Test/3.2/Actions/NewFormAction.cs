using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class NewFormAction {
    public void Execute(IMainForm mainform) {
      FormView1 x = new FormView1();
      x.Caption = "FormView";
      x.Show();
    }
  }

}
