using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class MatrixJsonItemControl : UserControl {
    public MatrixJsonItemControl() {
      InitializeComponent();
    }

    public static MatrixJsonItemControl Create(IJsonItemVM vm) {
      var control = new MatrixJsonItemControl();
      control.checkBoxRowsResizable.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.RowsResizable));
      control.checkBoxColumnsResizable .DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.ColumnsResizable));
      return control;
    }
  }
}
