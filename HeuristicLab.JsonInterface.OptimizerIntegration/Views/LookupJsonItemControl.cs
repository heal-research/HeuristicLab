using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class LookupJsonItemControl : UserControl {
    public LookupJsonItemControl() {
      InitializeComponent();
    }
    public LookupJsonItemControl(ILookupJsonItemVM vm) {
      InitializeComponent();
      textBoxActualName.DataBindings.Add("Text", vm, nameof(ILookupJsonItemVM.ActualName));
    }
  }
}
