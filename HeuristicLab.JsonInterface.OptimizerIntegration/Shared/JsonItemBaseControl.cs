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
    public JsonItemVM VM { get; set; }

    private JsonItemBaseControl() {
      InitializeComponent();
    }

    public JsonItemBaseControl(JsonItemVM vm) {
      InitializeComponent();
      VM = vm;
      labelPath.Text = VM.Item.Path;
      checkBoxActive.Checked = VM.Selected;
      textBoxName.Text = VM.Item.Name;
      if (string.IsNullOrWhiteSpace(VM.Item.ActualName))
        textBoxActualName.ReadOnly = true;
      else
        textBoxActualName.Text = VM.Item.ActualName;
    }

    private void checkBoxActive_CheckedChanged(object sender, EventArgs e) {
      VM.Selected = checkBoxActive.Checked;
    }

    private void textBoxName_TextChanged(object sender, EventArgs e) {
      VM.Item.Name = textBoxName.Text;
    }

    private void textBoxActualName_TextChanged(object sender, EventArgs e) {
      VM.Item.ActualName = textBoxActualName.Text;
    }
  }
}
