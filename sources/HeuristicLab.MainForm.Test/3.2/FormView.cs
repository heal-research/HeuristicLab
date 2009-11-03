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
  public partial class FormView1 : ViewBase,IView<ICollection> {
    private int[] array;
    public FormView1() {
      InitializeComponent();
    }

    #region IView<ICollection> Members

    public void View(ICollection item) {
      throw new NotImplementedException();
    }

    public ICollection Item {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}
