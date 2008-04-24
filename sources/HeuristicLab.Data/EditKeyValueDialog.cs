using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core; 

namespace HeuristicLab.Data {
  public partial class EditKeyValueDialog : Form {
    private IItem key; 
    public IItem Key {
      get { return key; }
    }

    private IItem value;
    public IItem Value {
      get { return value; }
    }

    public EditKeyValueDialog(Type keyType, Type valueType) {
      InitializeComponent();
      key = (IItem) Activator.CreateInstance(keyType);
      value = (IItem) Activator.CreateInstance(valueType); 
      keyPanel.Controls.Add((Control) key.CreateView());
      valuePanel.Controls.Add((Control) value.CreateView()); 
    }

    private void okButton_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }
}
