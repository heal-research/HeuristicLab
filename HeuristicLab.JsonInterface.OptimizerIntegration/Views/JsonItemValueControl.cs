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
    protected override string MinRangePropertyId => nameof(IntValueVM.MinRange);
    protected override string MaxRangePropertyId => nameof(IntValueVM.MaxRange);
    protected override string EnableMinRangePropertyId => nameof(IntValueVM.EnableMinRange);
    protected override string EnableMaxRangePropertyId => nameof(IntValueVM.EnableMaxRange);
    #endregion

    public JsonItemIntValueControl(IntValueVM vm) : base(vm) {
      Init();
    }

  }

  public class JsonItemDoubleValueControl : JsonItemValueControl {

    #region Overriden Properties
    protected override string ValuePropertyId => nameof(DoubleValueVM.Value);
    protected override string MinRangePropertyId => nameof(DoubleValueVM.MinRange);
    protected override string MaxRangePropertyId => nameof(DoubleValueVM.MaxRange);
    protected override string EnableMinRangePropertyId => nameof(DoubleValueVM.EnableMinRange);
    protected override string EnableMaxRangePropertyId => nameof(DoubleValueVM.EnableMaxRange);
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
    protected abstract string MinRangePropertyId { get; }
    protected abstract string MaxRangePropertyId { get; }
    protected abstract string EnableMinRangePropertyId { get; }
    protected abstract string EnableMaxRangePropertyId { get; }
    #endregion

    public JsonItemValueControl(JsonItemVMBase vm) : base(vm) {
      InitializeComponent();
      TBValue = textBoxValue;
      NumericRangeControl = numericRangeControl1;
    }

    protected void Init() {
      TBValue.DataBindings.Add("Text", base.VM, ValuePropertyId);
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, MinRangePropertyId);
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, MaxRangePropertyId);
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, EnableMinRangePropertyId,
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, EnableMaxRangePropertyId,
        false, DataSourceUpdateMode.OnPropertyChanged);
    }

  }
}
