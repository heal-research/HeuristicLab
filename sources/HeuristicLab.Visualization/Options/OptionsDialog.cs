﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Options {
  public partial class OptionsDialog : Form {
    private readonly IChartDataRowsModel model;
    private readonly ViewSettings viewSettings;
    private readonly ViewSettings oldViewSettings;
    private LineParams[] oldLineParams;
    private Dictionary<CheckBox,bool> ShowYAxisBoxes;

    internal class LineParams {
      string Label { get; set; }
      Color Color { get; set; }
      int Thickness { get; set; }
      DrawingStyle Style { get; set; }
      private readonly IDataRow row;

      public LineParams(IDataRow row) {
        Label = row.Label;
        Color = row.Color;
        Thickness = row.Thickness;
        Style = row.Style;
        this.row = row;
      }

      public void applySettings() {
        row.Label = Label;
        row.Color = Color;
        row.Thickness = Thickness;
        row.Style = Style;
      }
    }

    public OptionsDialog(IChartDataRowsModel model) {
      InitializeComponent();

      this.model = model;
      viewSettings = model.ViewSettings;
      oldViewSettings = new ViewSettings(model.ViewSettings);

      cbLegendPosition.Items.Add(LegendPosition.Top);
      cbLegendPosition.Items.Add(LegendPosition.Bottom);
      cbLegendPosition.Items.Add(LegendPosition.Left);
      cbLegendPosition.Items.Add(LegendPosition.Right);

      cbLegendPosition.SelectedItem = viewSettings.LegendPosition;
    }

    private void OptionsDialogSelectColorBtn_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      dlg.ShowDialog();
      ColorPreviewTB.BackColor = dlg.Color;
      ((IDataRow)LineSelectCB.SelectedValue).Color = dlg.Color;
    }

    public IList<int> GetThicknesses() {
      return new List<int>(new int[] {1, 2, 3, 4, 5, 6, 7, 8});
    }

    public IList<DrawingStyle> GetStyles() {
      return new List<DrawingStyle>(new DrawingStyle[] {DrawingStyle.Solid, DrawingStyle.Dashed});
    }

    private void OptionsDialog_Load(object sender, EventArgs e) {
      InitTabPageLines();
      InitTabPageYAxes();
    }

    private void InitTabPageLines() {
      if (model.Rows.Count != 0) {
        int index = 0;
        oldLineParams = new LineParams[model.Rows.Count];
        foreach (var row in model.Rows) {
          oldLineParams[index++]= new LineParams(row);
        }
        LineThicknessCB.DataSource = GetThicknesses();
        LinestyleCB.DataSource = GetStyles();
        LinestyleCB.SelectedItem = model.Rows[0].Style;
        LineThicknessCB.SelectedItem = model.Rows[0].Thickness;

        LineSelectCB.DataSource = model.Rows;
        LineSelectCB.DisplayMember = "Label";

        
        LineSelectCB.SelectedIndex = 0;
        LineSelectCB_SelectedIndexChanged(this, null);
        
      }
    }

    private void InitTabPageYAxes() {
      ShowYAxisBoxes = new Dictionary<CheckBox, bool>();
      for (int i = 0; i < model.YAxes.Count; i++) {
        YAxisDescriptor yAxisDescriptor = model.YAxes[i];

        CheckBox chkbox = new CheckBox();
        chkbox.Text = yAxisDescriptor.Label;
        chkbox.Checked = yAxisDescriptor.ShowYAxis;
        ShowYAxisBoxes[chkbox] = yAxisDescriptor.ShowYAxis;
        chkbox.CheckedChanged += delegate { yAxisDescriptor.ShowYAxis = chkbox.Checked; };
        
        dataRowsFlowLayout.Controls.Add(chkbox);
      }
    }

    private void LineSelectCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
      /*  int index =
          LineThicknessCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Thickness.ToString());
        LineThicknessCB.SelectedIndex = index;
        index = LinestyleCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Style.ToString());
        LinestyleCB.SelectedIndex = index;  */
        LineThicknessCB.SelectedItem = ((IDataRow) LineSelectCB.SelectedValue).Thickness;
        LinestyleCB.SelectedItem = ((IDataRow)LineSelectCB.SelectedValue).Style;
        ColorPreviewTB.BackColor = ((IDataRow)LineSelectCB.SelectedValue).Color;   
      }
    }

    
    private void OptionsDialogOkButton_Click(object sender, EventArgs e) {
      Close();
    }
    

   

    private void cbLegendPosition_SelectedIndexChanged(object sender, EventArgs e) {
      viewSettings.LegendPosition = (LegendPosition)cbLegendPosition.SelectedItem;
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

    private void OptionsDialogResetButton_Click(object sender, EventArgs e) {
      foreach (var param in oldLineParams) {
        param.applySettings();
      }

      foreach (var box in ShowYAxisBoxes) {
        box.Key.Checked = box.Value;
      }
      viewSettings.LegendColor = oldViewSettings.LegendColor;
      viewSettings.LegendPosition = oldViewSettings.LegendPosition;
      viewSettings.LegendFont = oldViewSettings.LegendFont;
      viewSettings.TitleColor = oldViewSettings.TitleColor;
      viewSettings.TitleFont = oldViewSettings.TitleFont;
      viewSettings.XAxisColor = oldViewSettings.LegendColor;
      viewSettings.XAxisFont = oldViewSettings.XAxisFont;
      viewSettings.UpdateView();
      cbLegendPosition.SelectedItem = viewSettings.LegendPosition;
      this.LineSelectCB_SelectedIndexChanged(this,null);
    }

    private void LinestyleCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null)
        ((IDataRow)LineSelectCB.SelectedValue).Style = (DrawingStyle)LinestyleCB.SelectedItem;
    }

    private void LineThicknessCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null)
        ((IDataRow)LineSelectCB.SelectedValue).Thickness = (int)LineThicknessCB.SelectedItem;
    }




  }
}
