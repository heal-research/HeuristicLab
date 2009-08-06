using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class NewEditorAction : IAction{
    #region IAction Members

    public void Execute(IMainForm mainform) {
      mainform.StatusStripText = "New Editor action called";
    }

    #endregion
  }

}
