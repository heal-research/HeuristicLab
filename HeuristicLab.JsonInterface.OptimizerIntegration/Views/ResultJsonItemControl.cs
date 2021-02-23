using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ResultJsonItemControl : UserControl {
    private ResultJsonItemControl() {
      InitializeComponent();
    }

    protected ResultJsonItemControl(ResultItemVM vm) {
      InitializeComponent();
      var formatters = ResultFormatter.ForType(vm.Item.ValueType).ToList();
      comboBoxFormatter.DataSource = formatters;
      comboBoxFormatter.DisplayMember = "Name";

      // set action to override the ResultFormatterType property for changing the selected value
      comboBoxFormatter.SelectedValueChanged += (s, e) => vm.ResultFormatterType = comboBoxFormatter.SelectedValue.GetType().FullName;
      comboBoxFormatter.SelectedItem = formatters.Last();
    }

    public static ResultJsonItemControl Create(ResultItemVM vm) => new ResultJsonItemControl(vm);
  }
}
