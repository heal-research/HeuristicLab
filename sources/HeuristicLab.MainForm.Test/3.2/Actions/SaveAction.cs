using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class SaveAction {
    public void Execute(IMainForm mainform) {
      MessageBox.Show("Save action called");
    }
  }
}
