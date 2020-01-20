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
      textBoxFrom.Text = VM.Item.Range.First().ToString();
      textBoxTo.Text = VM.Item.Range.Last().ToString();
      range[0] = VM.Item.Range.First();
      range[1] = VM.Item.Range.Last();
    }

    private object Parse(string s) {
      if (isDouble) {
        if (s == "-1,79769313486232E+308") return double.MinValue;
        if (s == "1,79769313486232E+308") return double.MaxValue;
        return double.Parse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
      }
      return int.Parse(s);
    }

    private void SetRange() {
      if (!string.IsNullOrWhiteSpace(textBoxFrom.Text))
        range[0] = Parse(textBoxFrom.Text);
      else
        range[0] = VM.Item.Range.First();

      if (!string.IsNullOrWhiteSpace(textBoxTo.Text))
        range[1] = Parse(textBoxTo.Text);
      else
        range[1] = VM.Item.Range.Last();
      VM.Item.Range = range;
    }

    private void textBoxTo_Leave(object sender, EventArgs e) {
      SetRange();
    }

    private void textBoxFrom_Leave(object sender, EventArgs e) {
      SetRange();
    }

    private void textBoxValue_Leave(object sender, EventArgs e) {
      if (!string.IsNullOrWhiteSpace(textBoxValue.Text))
        VM.Item.Value = Parse(textBoxValue.Text);
    }
  }
}
