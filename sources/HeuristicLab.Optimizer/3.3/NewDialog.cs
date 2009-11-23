using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Optimizer {
  public partial class NewDialog : Form {
    private Type newItemType;
    public Type NewItemType {
      get { return newItemType; }
    }

    public NewDialog() {
      InitializeComponent();
      newItemType = null;
    }
  }
}
