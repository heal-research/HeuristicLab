using System.ComponentModel;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    protected JsonItemVMBase VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    protected JsonItemBaseControl(JsonItemVMBase vm, UserControl control) {
      InitializeComponent();
      VM = vm;
      if (control != null) {
        control.Margin = new Padding() { All = 0 };
        tableLayoutPanel1.Controls.Add(control, 0, 2);
        control.Dock = DockStyle.Fill;
      }
      textBoxName.DataBindings.Add("Text", VM, nameof(JsonItemVMBase.Name));
      textBoxDescription.DataBindings.Add("Text", VM, nameof(JsonItemVMBase.Description));
    }

    private void textBoxName_Validating(object sender, CancelEventArgs e) {
      if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
        errorProvider.SetError(textBoxName, "Name must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxName, null);
      }
    }

    public static JsonItemBaseControl Create(JsonItemVMBase vm, UserControl control) => new JsonItemBaseControl(vm, control);
  }
}
