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
  }
}
