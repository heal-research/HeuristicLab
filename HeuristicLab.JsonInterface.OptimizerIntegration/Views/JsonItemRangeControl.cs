using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemRangeControl : UserControl {
    IJsonItemVM VM { get; set; }


    public JsonItemRangeControl(DoubleRangeVM vm) {
      InitializeComponent();
      VM = vm;
      textBoxValueFrom.DataBindings.Add("Text", vm, nameof(vm.MinValue));
      textBoxValueTo.DataBindings.Add("Text", vm, nameof(vm.MaxValue));
      InitNumbericRangeControl();

    }
    public JsonItemRangeControl(IntRangeVM vm) {
      InitializeComponent();
      VM = vm;
      textBoxValueFrom.DataBindings.Add("Text", vm, nameof(vm.MinValue));
      textBoxValueTo.DataBindings.Add("Text", vm, nameof(vm.MaxValue));
      InitNumbericRangeControl();
    }

    private void InitNumbericRangeControl() {
      numericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MinRange));
      numericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MaxRange));
      numericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      numericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }

    /*
    protected abstract object Parse(string s);

    private void SetValue() {
      if (!string.IsNullOrWhiteSpace(textBoxValueFrom.Text))
        value[0] = Parse(textBoxValueFrom.Text);
      else
        value[0] = ((Array)VM.Item.Value).GetValue(0);

      if (!string.IsNullOrWhiteSpace(textBoxValueTo.Text))
        value[1] = Parse(textBoxValueTo.Text);
      else
        value[1] = ((Array)VM.Item.Value).GetValue(1);
      VM.Item.Value = value;
    }
    
    private void textBoxValueFrom_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void textBoxValueTo_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void numericRangeControl1_Load(object sender, EventArgs e) {
      numericRangeControl1.IsDouble = isDouble;
      numericRangeControl1.VM = VM;
    }
    */
  }
}
