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
  
  public class JsonItemIntArrayValueControl : JsonItemArrayValueControl {
    public JsonItemIntArrayValueControl(IntArrayValueVM vm) : base(vm, vm.Value) { }
  }

  public class JsonItemDoubleArrayValueControl : JsonItemArrayValueControl {
    public JsonItemDoubleArrayValueControl(DoubleArrayValueVM vm) : base(vm, vm.Value) { }
  }
  
  public abstract partial class JsonItemArrayValueControl : JsonItemBaseControl {
    protected NumericRangeControl NumericRangeControl { get; set; }

    public JsonItemArrayValueControl(JsonItemVMBase vm, object dataSource) : base(vm) {
      InitializeComponent();
      dataGridView.DataSource = dataSource;
      NumericRangeControl = numericRangeControl1;

      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }
  }
}
