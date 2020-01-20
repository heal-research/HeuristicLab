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
  public partial class JsonItemBoolControl : JsonItemBaseControl {
    public JsonItemBoolControl(JsonItemVM vm) : base(vm) {
      InitializeComponent();
      checkBoxValue.Checked = (bool)vm.Item.Value;
    }

    private void checkBoxValue_CheckStateChanged(object sender, EventArgs e) {
      VM.Item.Value = checkBoxValue.Checked;
    }
  }
}
