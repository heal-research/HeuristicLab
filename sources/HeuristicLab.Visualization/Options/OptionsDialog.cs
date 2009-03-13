using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    private readonly IChartDataRowsModel model;
    private readonly ViewSettings viewSettings;

    public OptionsDialog(IChartDataRowsModel model) {
      InitializeComponent();

      this.model = model;
      viewSettings = model.ViewSettings;
    }

    private void OptionsDialogSelectColorBtn_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      dlg.ShowDialog();
      ColorPreviewTB.BackColor = dlg.Color;
    }

    public IList<int> GetThicknesses() {
      return new List<int>(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8});
    }

    public IList<DrawingStyle> GetStyles() {
      return new List<DrawingStyle>(new DrawingStyle[] {DrawingStyle.Solid, DrawingStyle.Dashed});
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

      InitTabPageYAxes();
    }

    private void InitTabPageYAxes() {
      for (int i = 0; i < model.Rows.Count; i++) {
        IDataRow row = model.Rows[i];

        CheckBox chkbox = new CheckBox();
        chkbox.Text = row.Label;
        chkbox.Checked = row.ShowYAxis;
        chkbox.CheckedChanged += delegate { row.ShowYAxis = chkbox.Checked; };
        
        dataRowsFlowLayout.Controls.Add(chkbox);
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

    private void cbLegendPosition_SelectedIndexChanged(object sender, EventArgs e) {
      string pos = cbLegendPosition.SelectedItem.ToString();
      if (pos.Equals("left")) {
        viewSettings.LegendPosition = LegendPosition.Left;
      } else if (pos.Equals("right")) {
        viewSettings.LegendPosition = LegendPosition.Right;
      } else if (pos.Equals("bottom")) {
        viewSettings.LegendPosition = LegendPosition.Bottom;
      } else {
        viewSettings.LegendPosition = LegendPosition.Top;
      }
      
      viewSettings.UpdateView();
    }

    private void btnChangeTitleFont_Click(object sender, EventArgs e) {
      fdFont.Font = viewSettings.TitleFont;
      fdFont.Color = viewSettings.TitleColor;

      DialogResult dr = fdFont.ShowDialog();

      if(dr == DialogResult.OK) {
        viewSettings.TitleFont = fdFont.Font;
        viewSettings.TitleColor = fdFont.Color;

        viewSettings.UpdateView();
      }
    }

    private void btnChangeLegendFont_Click(object sender, EventArgs e) {
      fdFont.Font = viewSettings.LegendFont;
      fdFont.Color = viewSettings.LegendColor;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
        viewSettings.LegendFont = fdFont.Font;
        viewSettings.LegendColor = fdFont.Color;

        viewSettings.UpdateView();
      }
    }

    private void btnChangeXAxisFont_Click(object sender, EventArgs e) {
      fdFont.Font = viewSettings.XAxisFont;
      fdFont.Color = viewSettings.XAxisColor;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
        viewSettings.XAxisFont = fdFont.Font;
        viewSettings.XAxisColor = fdFont.Color;

        viewSettings.UpdateView();
      }
    }
  }
}
