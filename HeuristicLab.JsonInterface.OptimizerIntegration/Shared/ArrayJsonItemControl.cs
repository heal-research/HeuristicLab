using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ArrayJsonItemControl : UserControl {

    public ArrayJsonItemControl() {
      InitializeComponent();
    }

    public static ArrayJsonItemControl Create(IJsonItemVM vm) {
      var control = new ArrayJsonItemControl();
      control.checkBoxResizable.DataBindings.Add("Checked", vm, nameof(IArrayJsonItemVM.Resizable));
      return control;
    }
  }
}
