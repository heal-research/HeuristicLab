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
      if (VM.Item.Range != null) {
        foreach (var i in VM.Item.Range)
          SetupOption((string)i);
        comboBoxValues.DataBindings.Add("SelectedItem", VM, nameof(StringValueVM.Value));
      } else {
        comboBoxValues.Hide();
        groupBoxRange.Hide();
        TextBox tb = new TextBox();
        this.Controls.Add(tb);
        tb.Location = comboBoxValues.Location;
        tb.Size = comboBoxValues.Size;
        tb.Anchor = comboBoxValues.Anchor;
        tb.Dock = comboBoxValues.Dock;
        tb.DataBindings.Add("Text", VM, nameof(StringValueVM.Value));
        tb.Show();
      }
    }
    
    private void SetupOption(string opt) {
      AddComboOption(opt);
      TextBox tb = new TextBox();
      tb.Text = opt;
      //tb.Size = new Size()
      tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      //tb.Dock = DockStyle.Right | DockStyle.Left;
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
      ((StringValueVM)VM).Range = items;
      comboBoxValues.Enabled = true;
      tableOptions.Refresh();
    }

    private void RemoveComboOption(string opt) {
      comboBoxValues.Items.Remove(opt);
      IList<string> items = new List<string>();
      foreach (var i in comboBoxValues.Items) {
        items.Add((string)i);
      }
      ((StringValueVM)VM).Range = items;
      if (((StringValueVM)VM).Range.Count() <= 0) {
        comboBoxValues.Enabled = false;
        comboBoxValues.SelectedIndex = -1;
      }
      tableOptions.Refresh();
    }
  }
}
