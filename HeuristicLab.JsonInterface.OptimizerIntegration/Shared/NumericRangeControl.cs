using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration.Shared {
  public partial class NumericRangeControl : UserControl {

    private JsonItemVM vm;
    public JsonItemVM VM {
      get => vm; 
      set {
        vm = value;
        Init();
      } 
    }
    public bool IsDouble { get; set; }



    public NumericRangeControl() {
      InitializeComponent();
      Init();
    }

    private void Init() {
      textBoxFrom.Text = "";
      textBoxTo.Text = "";
      textBoxFrom.ReadOnly = true;
      textBoxTo.ReadOnly = true;
      checkBoxFrom.Checked = false;
      checkBoxFrom.Checked = false;
    }
    
    private void checkBoxFrom_CheckStateChanged(object sender, EventArgs e) {
      textBoxFrom.ReadOnly = !checkBoxFrom.Checked;
      textBoxFrom.Text = "";
      if(!checkBoxFrom.Checked)
        SetRange();
    }

    private void checkBoxTo_CheckStateChanged(object sender, EventArgs e) {
      textBoxTo.ReadOnly = !checkBoxTo.Checked;
      textBoxTo.Text = "";
      if (!checkBoxTo.Checked)
        SetRange();
    }

    private void textBoxFrom_Leave(object sender, EventArgs e) {
      if (checkBoxFrom.Checked)
        SetRange();
    }

    private void textBoxTo_Leave(object sender, EventArgs e) {
      if (checkBoxTo.Checked)
        SetRange();
    }

    private void SetRange() {
      object[] range = new object[2];
      if (checkBoxFrom.Checked && !string.IsNullOrWhiteSpace(textBoxFrom.Text))
        range[0] = Parse(textBoxFrom.Text);
      else
        range[0] = IsDouble ? double.MinValue : int.MinValue;

      if (checkBoxTo.Checked && !string.IsNullOrWhiteSpace(textBoxTo.Text))
        range[1] = Parse(textBoxTo.Text);
      else
        range[1] = IsDouble ? double.MinValue : int.MinValue;
      VM.Item.Range = range;
    }

    private object Parse(string s) {
      if (IsDouble) {
        return double.Parse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
      }
      return int.Parse(s);
    }
  }
}
