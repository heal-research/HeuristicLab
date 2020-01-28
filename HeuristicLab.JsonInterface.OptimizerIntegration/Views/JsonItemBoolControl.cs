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
    private BoolValueVM VM { get; set; }

    public JsonItemBoolControl(BoolValueVM vm) : base(vm) {
      InitializeComponent();
      VM = vm;
      checkBoxValue.DataBindings.Add("Checked", VM, nameof(BoolValueVM.Value));
    }
  }
}
