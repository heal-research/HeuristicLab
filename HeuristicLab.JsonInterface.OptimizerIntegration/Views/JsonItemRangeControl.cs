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

    public JsonItemRangeControl(DoubleRangeVM vm) : base(vm) {
      InitializeComponent();
      /*
      this.isDouble = isDouble;
      textBoxValueFrom.Text = ((Array)VM.Item.Value).GetValue(0).ToString();
      textBoxValueTo.Text = ((Array)VM.Item.Value).GetValue(1).ToString();
      textBoxValueFrom.Text = VM.Item.Range.First().ToString();
      textBoxValueTo.Text = VM.Item.Range.Last().ToString();
      */
    }
    public JsonItemRangeControl(IntRangeVM vm) : base(vm) {
      InitializeComponent();
      /*
      this.isDouble = isDouble;
      textBoxValueFrom.Text = ((Array)VM.Item.Value).GetValue(0).ToString();
      textBoxValueTo.Text = ((Array)VM.Item.Value).GetValue(1).ToString();
      textBoxValueFrom.Text = VM.Item.Range.First().ToString();
      textBoxValueTo.Text = VM.Item.Range.Last().ToString();
      */
    }
    /*
    protected abstract object Parse(string s);

    private void SetValue() {
      if (!string.IsNullOrWhiteSpace(textBoxValueFrom.Text))
        value[0] = Parse(textBoxValueFrom.Text);
      else
        value[0] = ((Array)VM.Item.Value).GetValue(0);

      if (!string.IsNullOrWhiteSpace(textBoxValueTo.Text))
        value[1] = Parse(textBoxValueTo.Text);
      else
        value[1] = ((Array)VM.Item.Value).GetValue(1);
      VM.Item.Value = value;
    }
    
    private void textBoxValueFrom_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void textBoxValueTo_Leave(object sender, EventArgs e) {
      SetValue();
    }

    private void numericRangeControl1_Load(object sender, EventArgs e) {
      numericRangeControl1.IsDouble = isDouble;
      numericRangeControl1.VM = VM;
    }
    */
  }
}
