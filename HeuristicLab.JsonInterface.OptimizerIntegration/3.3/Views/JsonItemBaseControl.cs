using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    protected JsonItemVMBase VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    protected JsonItemBaseControl(JsonItemVMBase vm) {
      InitializeComponent();
      VM = vm;
      propertyList.Items.Clear();
      var font = propertyList.Font;
      propertyList.Font = new System.Drawing.Font(font, System.Drawing.FontStyle.Bold);
      foreach (var prop in vm.Properties) {
        var item = new ListViewItem(new [] { prop.Key, prop.Value }) { 
          Checked = true, 
          Font = font 
        };
        propertyList.Items.Add(item);
      }
      propertyList.Columns[0].Width = -2;
      propertyList.Columns[1].Width = 100;

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

    public static JsonItemBaseControl Create(JsonItemVMBase vm) => new JsonItemBaseControl(vm);
  }
}
