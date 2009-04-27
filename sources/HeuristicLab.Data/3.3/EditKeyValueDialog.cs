using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core; 

namespace HeuristicLab.Data {
  /// <summary>
  /// A visual representation to visualize a key-value pair.
  /// </summary>
  public partial class EditKeyValueDialog : Form {
    private IItem key; 
    /// <summary>
    /// Gets the current key.
    /// </summary>
    public IItem Key {
      get { return key; }
    }

    private IItem value;
    /// <summary>
    /// Gets the current value.
    /// </summary>
    public IItem Value {
      get { return value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="EditKeyValueDialog"/> with the given types of 
    /// key and value.
    /// </summary>
    /// <param name="keyType">The type of the key.</param>
    /// <param name="valueType">The type of the value.</param>
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
