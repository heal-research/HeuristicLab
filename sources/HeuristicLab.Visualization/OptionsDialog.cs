using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class OptionsDialog : Form {
    private LineChart lc;

    public OptionsDialog(LineChart lc) {
      InitializeComponent();
      this.lc = lc;
    }


    private void button1_Click(object sender, EventArgs e) {
      var dlg = new ColorDialog();
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
      int index = this.LineThicknessCB.FindStringExact(((IDataRow) this.LineSelectCB.SelectedValue).Thickness.ToString());
      this.LineThicknessCB.SelectedIndex = index;
      index = this.LinestyleCB.FindStringExact(((IDataRow) this.LineSelectCB.SelectedValue).Style.ToString());
      LinestyleCB.SelectedIndex = index;
      this.ColorPreviewTB.BackColor = ((IDataRow) this.LineSelectCB.SelectedValue).Color;
    }

    private void OptionsDialogCancelButton_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void OptionsDialogOkButton_Click(object sender, EventArgs e) {
      ((IDataRow) this.LineSelectCB.SelectedValue).Thickness = (int) this.LineThicknessCB.SelectedItem;
      ((IDataRow) this.LineSelectCB.SelectedValue).Color = this.ColorPreviewTB.BackColor;
      ((IDataRow) this.LineSelectCB.SelectedValue).Style = (DrawingStyle) this.LineThicknessCB.SelectedItem;
      this.lc.ApplyChangesToRow((IDataRow) this.LineSelectCB.SelectedValue);
      this.Close();
    }
  }
}