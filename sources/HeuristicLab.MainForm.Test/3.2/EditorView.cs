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
  [DefaultViewAttribute]
  public partial class EditorView : ViewBase<ArrayList> {
    public EditorView() {
      InitializeComponent();
    }

    private void ChangeStateButton_Click(object sender, EventArgs e) {
      IEnumerable<Type> views = MainFormManager.GetViewTypes(typeof(ArrayList));
      views.ToString();
      IEnumerable<Type> views1 = MainFormManager.GetViewTypes(typeof(IList));
      views1.ToString();
      //IEnumerable<Type> views2 = MainFormManager.GetViewTypes(typeof(object));
      //views2.ToString();
      Type def2 = MainFormManager.GetDefaultViewType(typeof(IList));
      def2.ToString();
      Type def1 = MainFormManager.GetDefaultViewType(typeof(ArrayList));
      def1.ToString();
      //Type def3 = MainFormManager.GetDefaultViewType(typeof(object));
      //def3.ToString();

      MainFormManager.MainForm.HideView(this);
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
