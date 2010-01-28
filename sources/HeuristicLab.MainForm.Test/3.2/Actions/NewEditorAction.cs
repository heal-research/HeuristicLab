using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public static class NewEditorAction{

    public static void Execute(IMainForm mainform) {
      EditorView view = new EditorView();
      view.Caption = "Editor View " + mainform.Views.Count();
      view.Show();
    }
  }

}
