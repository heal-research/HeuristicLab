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
  public partial class JsonItemRangeControl : JsonItemBaseControl {
    bool isDouble = false;
    object[] range = new object[2];
    object[] value = new object[2];

    public JsonItemRangeControl(JsonItemVM vm, bool isDouble) : base(vm) {
      InitializeComponent();
      this.isDouble = isDouble;
      textBoxValueFrom.Text = ((Array)VM.Item.Value).GetValue(0).ToString();
      textBoxValueTo.Text = ((Array)VM.Item.Value).GetValue(1).ToString();
      textBoxValueFrom.Text = VM.Item.Range.First().ToString();
      textBoxValueTo.Text = VM.Item.Range.Last().ToString();
    }

    private object Parse(string s) {
      if (isDouble) {
        if (s == "-1,79769313486232E+308") return double.MinValue;
        if (s == "1,79769313486232E+308") return double.MaxValue;
        return double.Parse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
      }
      return int.Parse(s);
    }

    private void SetValue() {
      if (!string.IsNullOrWhiteSpace(textBoxRangeFrom.Text))
        value[0] = Parse(textBoxValueFrom.Text);
      else
        value[0] = ((Array)VM.Item.Value).GetValue(0);

      if (!string.IsNullOrWhiteSpace(textBoxRangeTo.Text))
        value[1] = Parse(textBoxValueTo.Text);
      else
        value[1] = ((Array)VM.Item.Value).GetValue(1);
      VM.Item.Value = value;
    }

    private void SetRange() {
      if (!string.IsNullOrWhiteSpace(textBoxRangeFrom.Text))
        range[0] = Parse(textBoxRangeFrom.Text);
      else
        range[0] = VM.Item.Range.First();

      if (!string.IsNullOrWhiteSpace(textBoxRangeTo.Text))
        range[1] = Parse(textBoxRangeTo.Text);
      else
        range[1] = VM.Item.Range.Last();
      VM.Item.Range = range;
    }

    private void textBoxValueFrom_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void textBoxValueTo_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void textBoxRangeFrom_Leave(object sender, EventArgs e) {
      SetRange();
    }

    private void textBoxRangeTo_Leave(object sender, EventArgs e) {
      SetRange();
    }
  }
}
