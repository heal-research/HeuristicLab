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

    public JsonItemValidValuesControl(StringValueVM vm) : base(vm) {
      InitializeComponent();
      /*
      foreach (var i in VM.Item.Range) {
        AddOption((string)i);
        if(i == VM.Item.Value) {
          comboBoxValues.SelectedItem = (string)i;
        }
      }*/
    }
    /*
    private void AddOption(string opt) {
      AddComboOption(opt);
      TextBox tb = new TextBox();
      tb.Text = opt;
      tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tb.Size = new Size(420, 20);
      tb.ReadOnly = true;

      CheckBox checkBox = new CheckBox();
      checkBox.Checked = true;
      
      checkBox.CheckStateChanged += (o, args) => {
        if (checkBox.Checked)
          AddComboOption(opt);
        else
          RemoveComboOption(opt);
      };
      tableOptions.Controls.Add(checkBox);
      tableOptions.Controls.Add(tb);
    }

    private void AddComboOption(string opt) {
      comboBoxValues.Items.Add(opt);
      IList<string> items = new List<string>();
      foreach (var i in comboBoxValues.Items) {
        items.Add((string)i);
      }
      VM.Item.Range = items;
      tableOptions.Refresh();
    }

    private void RemoveComboOption(string opt) {
      comboBoxValues.Items.Remove(opt);
      IList<string> items = new List<string>();
      foreach (var i in comboBoxValues.Items) {
        items.Add((string)i);
      }
      VM.Item.Range = items;
      tableOptions.Refresh();
    }
    
    private void comboBoxValues_SelectedValueChanged(object sender, EventArgs e) {
      VM.Item.Value = (string)comboBoxValues.SelectedItem;
    }

    private void JsonItemValidValuesControl_Load(object sender, EventArgs e) {

    }
    */
  }
}
