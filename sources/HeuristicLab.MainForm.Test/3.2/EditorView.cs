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
  [Content(typeof(ArrayList),IsDefaultView=true)]
  public partial class EditorView : ViewBase {
    public EditorView()
      : base() {
      InitializeComponent();
    }

    public EditorView(ArrayList list)
      : this() {
    }

    private void ChangeStateButton_Click(object sender, EventArgs e) {
      IEnumerable<Type> views = MainFormManager.GetViewTypes(typeof(ArrayList));
      views.ToString();
      IEnumerable<Type> views1 = MainFormManager.GetViewTypes(typeof(IList));
      views1.ToString();
      IEnumerable<Type> views2 = MainFormManager.GetViewTypes(typeof(IEnumerable));
      views2.ToString();

      ArrayList list = new ArrayList();
      IView defaultView = MainFormManager.CreateDefaultView(list);
      MainFormManager.MainForm.ShowView(defaultView);
      this.OnChanged();
    }

    public override void OnClosing(object sender, CancelEventArgs e) {
      if (DialogResult.Yes != MessageBox.Show(
             "Recent changes have not been saved. Close the editor anyway?", "Close editor?",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button2)) {

        e.Cancel = true;

      }
    }
  }
}
