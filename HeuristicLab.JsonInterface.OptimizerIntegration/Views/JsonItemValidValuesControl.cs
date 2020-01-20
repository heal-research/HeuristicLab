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
  public partial class JsonItemValidValuesControl : JsonItemBaseControl {
   

    public JsonItemValidValuesControl(JsonItemVM vm) : base(vm) {
      InitializeComponent();
      foreach (var i in VM.Item.Range) {
        AddOption((string)i);
        if(i == VM.Item.Value) {
          comboBoxValues.SelectedItem = (string)i;
        }
      }
    }

    private void AddOption(string opt) {
      comboBoxValues.Items.Add(opt);
      TextBox tb = new TextBox();
      tb.Text = opt;
      tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tb.Size = new Size(420, 20);
      tb.ReadOnly = true;
      Button btn = new Button();
      btn.Text = "-";
      btn.Size = new Size(20, 20);
      btn.Click += (o, args) => {
        tableOptions.Controls.Remove(tb);
        tableOptions.Controls.Remove(btn);
        comboBoxValues.Items.Remove(tb.Text);
        IList<string> items = new List<string>();
        foreach(var i in comboBoxValues.Items) {
          items.Add((string)i);
        }
        VM.Item.Range = items;
        tableOptions.Refresh();
      };
      tableOptions.Controls.Add(tb);
      tableOptions.Controls.Add(btn);
    }

    private void buttonAdd_Click(object sender, EventArgs e) {
      string newOption = textBoxAdd.Text;
      if (string.IsNullOrWhiteSpace(newOption)) return;
      textBoxAdd.Text = "";
      AddOption(newOption);
    }

    private void comboBoxValues_SelectedValueChanged(object sender, EventArgs e) {
      VM.Item.Value = (string)comboBoxValues.SelectedItem;
    }

    private void JsonItemValidValuesControl_Load(object sender, EventArgs e) {

    }
  }
}
