using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public static class NewEditorAction{
    private static IView view;
    public static void Execute(IMainForm mainform) {
      if (view == null)
        view = new EditorView();
      view.Caption = "Editor View";
      mainform.ShowView(view);
    }
  }

}
