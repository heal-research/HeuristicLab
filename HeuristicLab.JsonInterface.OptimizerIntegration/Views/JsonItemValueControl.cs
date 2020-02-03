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

  public class JsonItemIntValueControl : JsonItemValueControl {

    #region Overriden Properties
    protected override string ValuePropertyId => nameof(IntValueVM.Value);
    #endregion

    public JsonItemIntValueControl(IntValueVM vm) : base(vm) {
      Init();
    }

  }

  public class JsonItemDoubleValueControl : JsonItemValueControl {

    #region Overriden Properties
    protected override string ValuePropertyId => nameof(DoubleValueVM.Value);
    #endregion

    public JsonItemDoubleValueControl(DoubleValueVM vm) : base(vm) {
      Init();
    }

  }

  public abstract partial class JsonItemValueControl : JsonItemBaseControl {
    #region Protected Properties
    protected TextBox TBValue { get; set; }
    protected NumericRangeControl NumericRangeControl { get; set; }
    #endregion

    #region Abstract Properties
    protected abstract string ValuePropertyId { get; }
    #endregion

    public JsonItemValueControl(JsonItemVMBase vm) : base(vm) {
      InitializeComponent();
      TBValue = textBoxValue;
      NumericRangeControl = numericRangeControl1;
    }

    protected void Init() {
      TBValue.DataBindings.Add("Text", base.VM, ValuePropertyId);
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }

  }
}
