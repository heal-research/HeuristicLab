using System.ComponentModel;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    protected IJsonItemVM VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    protected JsonItemBaseControl(IJsonItemVM vm, UserControl control) {
      InitializeComponent();
      VM = vm;
      if (control != null) {
        control.Margin = new Padding() { All = 0 };
        tableLayoutPanel1.Controls.Add(control, 0, 1);
        control.Dock = DockStyle.Fill;
      }
      textBoxName.DataBindings.Add("Text", VM, nameof(IJsonItemVM.Name));
      textBoxDescription.DataBindings.Add("Text", VM, nameof(IJsonItemVM.Description));
    }

    private void textBoxName_Validating(object sender, CancelEventArgs e) {
      if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
        errorProvider.SetError(textBoxName, "Name must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxName, null);
      }
    }

    public static JsonItemBaseControl Create(IJsonItemVM vm, UserControl control) => new JsonItemBaseControl(vm, control);
  }
}
