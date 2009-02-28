using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    private LineChart lc;

    public OptionsDialog(LineChart lc) {
      InitializeComponent();
      this.lc = lc;
    }

    private void button1_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      dlg.ShowDialog();
      this.ColorPreviewTB.BackColor = dlg.Color;
    }

    public IList<int> GetThicknesses() {
      return new List<int>() {0, 1, 2, 3, 4, 5, 6, 7, 8};
    }

    public IList<DrawingStyle> GetStyles() {
      return new List<DrawingStyle>() {DrawingStyle.Solid, DrawingStyle.Dashed};
    }

    private void OptionsDialog_Load(object sender, EventArgs e) {
      this.LineSelectCB.DataSource = lc.GetRows();
      this.LineSelectCB.DisplayMember = "Label";

      LineThicknessCB.DataSource = GetThicknesses();
      LinestyleCB.DataSource = GetStyles();
      LineSelectCB.SelectedIndex = 0;
      LineSelectCB_SelectedIndexChanged(this, null);
    }

    private void LineSelectCB_SelectedIndexChanged(object sender, EventArgs e) {
      IDataRow datarow = (IDataRow)this.LineSelectCB.SelectedValue;

      int index = this.LineThicknessCB.FindStringExact(datarow.Thickness.ToString());
      this.LineThicknessCB.SelectedIndex = index;
      index = this.LinestyleCB.FindStringExact(datarow.Style.ToString());
      LinestyleCB.SelectedIndex = index;
      this.ColorPreviewTB.BackColor = datarow.Color;
    }

    private void OptionsDialogCancelButton_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void OptionsDialogOkButton_Click(object sender, EventArgs e) {
      IDataRow datarow = (IDataRow)this.LineSelectCB.SelectedValue;

      datarow.Thickness = (int)this.LineThicknessCB.SelectedItem;
      datarow.Color = this.ColorPreviewTB.BackColor;
      datarow.Style = (DrawingStyle)this.LineThicknessCB.SelectedItem;

      this.lc.ApplyChangesToRow(datarow);
      this.Close();
    }
  }
}