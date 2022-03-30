using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class NumericRangeControl : UserControl {
    public TextBox TBMinRange { get; set; }
    public TextBox TBMaxRange { get; set; }
    public CheckBox EnableMinRange { get; set; }
    public CheckBox EnableMaxRange { get; set; }

    private NumericRangeControl() {
      InitializeComponent();
      Init();
    }

    protected NumericRangeControl(IJsonItemVM vm) {
      InitializeComponent();
      Init();
      TBMinRange.DataBindings.Add("Text", vm, nameof(RangedValueBaseVM<int, IntJsonItem>.MinRange));
      TBMaxRange.DataBindings.Add("Text", vm, nameof(RangedValueBaseVM<int, IntJsonItem>.MaxRange));
      EnableMinRange.DataBindings.Add("Checked", vm, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      EnableMaxRange.DataBindings.Add("Checked", vm, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }


    private void Init() {
      TBMinRange = textBoxFrom;
      TBMaxRange = textBoxTo;
      EnableMinRange = checkBoxFrom;
      EnableMaxRange = checkBoxTo;
      checkBoxFrom.CheckedChanged += ToggleFromInput;
      checkBoxTo.CheckedChanged += ToggleToInput;
    }

    private void ToggleToInput(object sender, EventArgs e) {
      textBoxTo.ReadOnly = !checkBoxTo.Checked;
    }

    private void ToggleFromInput(object sender, EventArgs e) {
      textBoxFrom.ReadOnly = !checkBoxFrom.Checked;
    }

    private void textBoxFrom_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxFrom.Text)) {
        errorProvider.SetError(textBoxFrom, "'From' must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxFrom, null);
      }
    }

    private void textBoxTo_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxTo.Text)) {
        errorProvider.SetError(textBoxTo, "'To' must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxTo, null);
      }
    }

    public static UserControl Create(IJsonItemVM vm) => new NumericRangeControl(vm);
  }
}
