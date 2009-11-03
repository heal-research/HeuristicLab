using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public partial class FormView2 : ViewBase, IView<IEnumerable> {
    private int[] array;
    public FormView2() {
      InitializeComponent();
    }

    #region IView<IEnumerable> Members

    public void View(IEnumerable item) {
      throw new NotImplementedException();
    }

    public IEnumerable Item {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}
