using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    private readonly IChartDataRowsModel model;
    private readonly ViewPropertiesModel propertiesModel;

    public OptionsDialog(IChartDataRowsModel model, ViewPropertiesModel propertiesModel) {
      InitializeComponent();

      this.model = model;
      this.propertiesModel = propertiesModel;
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
    }

    private void btnChangeTitleFont_Click(object sender, EventArgs e) {
      fdFont.Font = propertiesModel.TitleFont;
      fdFont.Color = propertiesModel.TitleColor;

      DialogResult dr = fdFont.ShowDialog();

      if(dr == DialogResult.OK) {
        propertiesModel.TitleFont = fdFont.Font;
        propertiesModel.TitleColor = fdFont.Color;

        propertiesModel.UpdateView();
      }
    }

    private void btnChangeLegendFont_Click(object sender, EventArgs e) {
      fdFont.Font = propertiesModel.LegendFont;
      fdFont.Color = propertiesModel.LegendColor;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
        propertiesModel.LegendFont = fdFont.Font;
        propertiesModel.LegendColor = fdFont.Color;

        propertiesModel.UpdateView();
      }
    }

    private void btnChangeXAxisFont_Click(object sender, EventArgs e) {
      fdFont.Font = propertiesModel.XAxisFont;
      fdFont.Color = propertiesModel.XAxisColor;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
        propertiesModel.XAxisFont = fdFont.Font;
        propertiesModel.XAxisColor = fdFont.Color;

        propertiesModel.UpdateView();
      }
    }
  }
}