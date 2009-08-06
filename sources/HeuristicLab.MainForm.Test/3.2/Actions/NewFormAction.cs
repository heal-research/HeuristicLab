using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class NewFormAction : IAction{
    #region IAction Members

    public void Execute(IMainForm mainform) {
      mainform.StatusStripText = "New form called";
      FormView x = new FormView();
      x.Caption = "FormView";
      mainform.ShowView(x);
    }

    #endregion
  }

}
