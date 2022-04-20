using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    protected JsonItemVM VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    protected JsonItemBaseControl(JsonItemVM vm) {
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
      propertyList.ItemCheck += PropertyCheckChanged;

      textBoxName.DataBindings.Add("Text", VM, nameof(JsonItemVM.Name));
      textBoxDescription.DataBindings.Add("Text", VM, nameof(JsonItemVM.Description));
    }


    private void PropertyCheckChanged(object sender, ItemCheckEventArgs e) {
      var item = propertyList.Items[e.Index];
      switch (e.NewValue) {
        case CheckState.Unchecked:
          VM.DeactivateProperty(item.Text);
          break;
        case CheckState.Checked:
          VM.ActivateProperty(item.Text);
          break;
        default: break;
      }
    }

    private void textBoxName_Validating(object sender, CancelEventArgs e) {
      if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
        errorProvider.SetError(textBoxName, "Name must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxName, null);
      }
    }

    public static JsonItemBaseControl Create(JsonItemVM vm) => new JsonItemBaseControl(vm);
  }
}
