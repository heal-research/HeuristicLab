using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    private readonly IChartDataRowsModel model;
    private readonly LineChart lineChart;

    public OptionsDialog(IChartDataRowsModel model, LineChart lineChart) {
      InitializeComponent();
      this.model = model;
      this.lineChart = lineChart;
    }

    private void OptionsDialogSelectColorBtn_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      dlg.ShowDialog();
      ColorPreviewTB.BackColor = dlg.Color;
    }

    public IList<int> GetThicknesses() {
      return new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8};
    }

    public IList<DrawingStyle> GetStyles() {
      return new List<DrawingStyle> {DrawingStyle.Solid, DrawingStyle.Dashed};
    }

    private void OptionsDialog_Load(object sender, EventArgs e) {
      if (model.Rows.Count != 0) {
        LineSelectCB.DataSource = model.Rows;
        LineSelectCB.DisplayMember = "Label";

        LineThicknessCB.DataSource = GetThicknesses();
        LinestyleCB.DataSource = GetStyles();
        LineSelectCB.SelectedIndex = 0;
        LineSelectCB_SelectedIndexChanged(this, null);
      }
    }

    private void LineSelectCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
        int index =
          LineThicknessCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Thickness.ToString());
        LineThicknessCB.SelectedIndex = index;
        index = LinestyleCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Style.ToString());
        LinestyleCB.SelectedIndex = index;
        ColorPreviewTB.BackColor = ((IDataRow)LineSelectCB.SelectedValue).Color;
      }
    }

    private void OptionsDialogCancelButton_Click(object sender, EventArgs e) {
      Close();
    }

    private void OptionsDialogOkButton_Click(object sender, EventArgs e) {
      ApplyChanges();

      Close();
    }

    private void OptionsDialogApplyBtn_Click(object sender, EventArgs e) {
      ApplyChanges();  
    }

    private void ApplyChanges() {
      if (LineSelectCB.SelectedValue != null) {
        ((IDataRow)LineSelectCB.SelectedValue).Thickness = (int)LineThicknessCB.SelectedItem;
        ((IDataRow)LineSelectCB.SelectedValue).Color = ColorPreviewTB.BackColor;
        ((IDataRow)LineSelectCB.SelectedValue).Style = (DrawingStyle)LinestyleCB.SelectedItem;
      }

      lineChart.Invalidate(true);
    }

    private void btnChangeTitleFont_Click(object sender, EventArgs e) {
      fdFont.Font = lineChart.Title.Font;
      fdFont.Color = lineChart.Title.Color;

      DialogResult dr = fdFont.ShowDialog();

      if(dr == DialogResult.OK) {
        lineChart.Title.Font = fdFont.Font;
        lineChart.Title.Color = fdFont.Color;
      }
    }

    private void btnChangeLegendFont_Click(object sender, EventArgs e) {
//      fdFont.Font = lineChart.Legend.Font;
//      fdFont.Color = lineChart.Legend.Color;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
//        lineChart.Legend.Font = fdFont.Font;
//        lineChart.Legend.Color = fdFont.Color;
      }
    }
  }
}