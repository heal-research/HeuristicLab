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

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemValueControl : JsonItemBaseControl {
    bool isDouble = false;
    object[] range = new object[2];

    public JsonItemValueControl(JsonItemVM vm, bool isDouble) : base(vm) {
      InitializeComponent();
      this.isDouble = isDouble;
      textBoxValue.Text = VM.Item.Value.ToString();
      range[0] = VM.Item.Range.First();
      range[1] = VM.Item.Range.Last();
    }

    private object Parse(string s) {
      if (isDouble) {
        return double.Parse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
      }
      return int.Parse(s);
    }


    private void textBoxValue_Leave(object sender, EventArgs e) {
      if (!string.IsNullOrWhiteSpace(textBoxValue.Text))
        VM.Item.Value = Parse(textBoxValue.Text);
    }

    private void numericRangeControl1_Load(object sender, EventArgs e) {
      numericRangeControl1.IsDouble = isDouble;
      numericRangeControl1.VM = VM;
    }
  }
}
