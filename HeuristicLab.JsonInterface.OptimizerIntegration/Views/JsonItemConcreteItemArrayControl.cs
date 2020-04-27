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
  public partial class JsonItemConcreteItemArrayControl : UserControl {
    private StringArrayVM VM { get; set; }
    private DataGridViewComboBoxColumn ComboBox { get; set; }
    public JsonItemConcreteItemArrayControl(StringArrayVM vm) {
      InitializeComponent();
      dataGridView.AllowUserToDeleteRows = true;
      VM = vm;
      if (VM.Item.ConcreteRestrictedItems != null) {
        ComboBox = new DataGridViewComboBoxColumn();
        dataGridView.Columns.Add(ComboBox);
        concreteItemsRestrictor.OnChecked += AddComboOption;
        concreteItemsRestrictor.OnUnchecked += RemoveComboOption;
        concreteItemsRestrictor.Init(VM.Item.ConcreteRestrictedItems);
      } else {
        dataGridView.AllowUserToAddRows = true;
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        groupBox2.Visible = false;
        tableLayoutPanel1.RowStyles[0].SizeType = SizeType.Percent;
        tableLayoutPanel1.RowStyles[0].Height = 100.0f;
      }
      if(VM.Value != null) {
        foreach (var val in VM.Value) {
          dataGridView.Rows.Add(val);
        }
      }
    }

    private void AddComboOption(object opt) {
      ComboBox.Items.Add(opt);
      IList<string> items = new List<string>();
      foreach (var i in ComboBox.Items) {
        items.Add((string)i);
      }
      VM.Range = items;
      dataGridView.AllowUserToAddRows = true;
    }

    private void RemoveComboOption(object opt) {
      ComboBox.Items.Remove(opt);
      IList<string> items = new List<string>();
      foreach (var i in ComboBox.Items) {
        items.Add((string)i);
      }
      VM.Range = items;
      
      if (VM.Range.Count() <= 0) {
        dataGridView.Rows.Clear();
        dataGridView.AllowUserToAddRows = false;
      }
    }

    private void BuildValue() {
      IList<string> list = new List<string>();
      for (int r = 0; r < dataGridView.Rows.Count; ++r) {
        var value = dataGridView[0, r].Value;
        if(value != null)
          list.Add((string)value);
      }
      VM.Value = list.ToArray();
    }
    
    private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
      BuildValue();
    }

    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      BuildValue();
    }
  }
}