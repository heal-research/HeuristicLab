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
  public partial class JsonItemBoolControl : UserControl {
    public JsonItemBoolControl(BoolValueVM vm) {
      InitializeComponent();
      checkBoxValue.DataBindings.Add("Checked", vm, nameof(BoolValueVM.Value));
    }
  }
}
