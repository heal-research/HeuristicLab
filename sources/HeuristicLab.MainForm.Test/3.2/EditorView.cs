using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;
using System.Collections;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  [Content(typeof(ArrayList), IsDefaultView = true)]
  public partial class EditorView : HeuristicLab.MainForm.WindowsForms.View {
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
      IEnumerable<Type> views1 = MainFormManager.GetViewTypes(typeof(List<string>));
      views1.ToString();
      IEnumerable<Type> views2 = MainFormManager.GetViewTypes(typeof(List<>));
      views2.ToString();
      IEnumerable<Type> views3 = MainFormManager.GetViewTypes(typeof(ICollection<>));
      views3.ToString();

      List<HeuristicLab.MainForm.WindowsForms.MenuItem> ilist = new List<HeuristicLab.MainForm.WindowsForms.MenuItem>();
      IView defaultView = MainFormManager.CreateDefaultView(ilist);
      defaultView.Show();

      List<object> list = new List<object>();
      IView dView = MainFormManager.CreateDefaultView(list);
      if (dView != null)
        dView.Show();
      this.OnChanged();
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      if (DialogResult.Yes != MessageBox.Show(
             "Recent changes have not been saved. Close the editor anyway?", "Close editor?",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button2)) {

        e.Cancel = true;

      }
    }

    private void EditorView_VisibleChanged(object sender, EventArgs e) {
      MainFormManager.MainForm.Title = "visible: " + this.Visible;
    }
  }
}
