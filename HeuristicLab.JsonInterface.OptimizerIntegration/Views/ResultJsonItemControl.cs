using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ResultJsonItemControl : UserControl {
    public ResultJsonItemControl() {
      InitializeComponent();
    }

    public static ResultJsonItemControl Create(ResultItemVM vm) {
      var control = new ResultJsonItemControl();
      var formatters = ResultFormatter.ForType(vm.Item.ValueType).ToList();
      control.comboBoxFormatter.DataSource = formatters;
      control.comboBoxFormatter.DisplayMember = "Name";
      
      // set action to override the ResultFormatterType property for changing the selected value
      control.comboBoxFormatter.SelectedValueChanged += (s, e) => vm.ResultFormatterType = control.comboBoxFormatter.SelectedValue.GetType().FullName;
      control.comboBoxFormatter.SelectedItem = formatters.Last();

      return control;
    }
  }
}
