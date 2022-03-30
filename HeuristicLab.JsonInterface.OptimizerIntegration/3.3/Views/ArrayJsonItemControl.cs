using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ArrayJsonItemControl : UserControl {

    private ArrayJsonItemControl() {
      InitializeComponent();
    }

    protected ArrayJsonItemControl(IJsonItemVM vm) {
      InitializeComponent();
      checkBoxResizable.DataBindings.Add("Checked", vm, nameof(IArrayJsonItemVM.Resizable));
    }

    public static ArrayJsonItemControl Create(IJsonItemVM vm) => new ArrayJsonItemControl(vm);
  }
}
