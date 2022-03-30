using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class MatrixJsonItemControl : UserControl {
    private MatrixJsonItemControl() {
      InitializeComponent();
    }

    protected MatrixJsonItemControl(IJsonItemVM vm) {
      InitializeComponent();
      checkBoxRowsResizable.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.RowsResizable));
      checkBoxColumnsResizable.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.ColumnsResizable));
    }

    public static MatrixJsonItemControl Create(IJsonItemVM vm) => new MatrixJsonItemControl(vm);
  }
}
