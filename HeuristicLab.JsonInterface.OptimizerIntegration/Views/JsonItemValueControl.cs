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
  
  public abstract partial class JsonItemValueControl : UserControl {
    #region Protected Properties
    protected IJsonItemVM VM { get; set; }
    protected TextBox TBValue { get; set; }
    protected NumericRangeControl NumericRangeControl { get; set; }
    #endregion

    #region Abstract Properties
    protected abstract string ValuePropertyId { get; }
    #endregion

    public JsonItemValueControl() {
      InitializeComponent();
    }

    public JsonItemValueControl(IJsonItemVM vm) {
      InitializeComponent();
      VM = vm;
      TBValue = textBoxValue;
      NumericRangeControl = numericRangeControl1;
    }
    
    protected void Init() { 
      TBValue.DataBindings.Add("Text", VM, ValuePropertyId);
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void textBoxValue_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxValue.Text)) {
        errorProvider.SetError(textBoxValue, "Value must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxValue, null);
      }
    }
  }
}
