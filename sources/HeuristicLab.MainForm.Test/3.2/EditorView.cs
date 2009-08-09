using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public partial class EditorView : ViewBase {
    public EditorView() {
      InitializeComponent();
    }

    public EditorView(IMainForm mainform)
      : base(mainform) {
    }

    private void ChangeStateButton_Click(object sender, EventArgs e) {
      this.OnStateChanged();
    }

    public override void FormClosing(object sender, FormClosingEventArgs e) {
      if (DialogResult.Yes != MessageBox.Show(
             "Recent changes have not been saved. Close the editor anyway?", "Close editor?",
              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button2)) {

        e.Cancel = true;

      }
    }
  }
}
