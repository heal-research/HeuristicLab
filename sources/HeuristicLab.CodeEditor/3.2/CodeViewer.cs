using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.CodeEditor {
  public partial class CodeViewer : Form {

    public CodeViewer() {
      InitializeComponent();
      textEditorControl1.IsReadOnly = true;
      textEditorControl1.SetHighlighting("C#");
    }

    public CodeViewer(string code) : this() {
      textEditorControl1.Text = code;
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
