using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemBaseControl : UserControl {
    protected IJsonItemVM VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }
    
    public JsonItemBaseControl(IJsonItemVM vm) {
      InitializeComponent();
      VM = vm;

      checkBoxActive.DataBindings.Add("Checked", VM, nameof(IJsonItemVM.Selected),
        false, DataSourceUpdateMode.OnPropertyChanged);
      textBoxName.DataBindings.Add("Text", VM, nameof(IJsonItemVM.Name));
      textBoxDescription.DataBindings.Add("Text", VM, nameof(IJsonItemVM.Description));
      textBoxActualName.DataBindings.Add("Text", VM, nameof(IJsonItemVM.ActualName));

      if (string.IsNullOrWhiteSpace(VM.Item.ActualName))
        textBoxActualName.ReadOnly = true;
      else
        textBoxActualName.Text = VM.Item.ActualName;

    }

    private void textBoxName_Validating(object sender, CancelEventArgs e) {
      if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
        errorProvider.SetError(textBoxName, "Name must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxName, null);
      }
    }
  }
}
