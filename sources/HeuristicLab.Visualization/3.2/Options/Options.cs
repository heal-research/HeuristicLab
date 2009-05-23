using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Options {
  public partial class Options : UserControl {
    private IChartDataRowsModel model;
    private ViewSettings viewSettings;
    private ViewSettings oldViewSettings;
    private LineParams[] oldLineParams;
    private string oldTitle;
    private Dictionary<CheckBox, bool> ShowYAxisBoxes;
    private bool oldShowXAxisGrid;
    private Dictionary<CheckBox, bool> yAxisClipChangeableBoxes;

    internal class LineParams {
      private string Label { get; set; }
      private Color Color { get; set; }
      private bool ShowMarkers { get; set; }
      private int Thickness { get; set; }
      private DrawingStyle Style { get; set; }
      private readonly IDataRow row;

      public LineParams(IDataRow row) {
        Label = row.Label;
        Color = row.Color;
        Thickness = row.Thickness;
        Style = row.Style;
        this.row = row;
        this.ShowMarkers = row.ShowMarkers;
      }

      public void applySettings() {
        row.Label = Label;
        row.Color = Color;
        row.Thickness = Thickness;
        row.Style = Style;
        row.ShowMarkers = this.ShowMarkers;
      }
    }

    public Options() {
      InitializeComponent();

      cbLegendPosition.Items.Add(LegendPosition.Top);
      cbLegendPosition.Items.Add(LegendPosition.Bottom);
      cbLegendPosition.Items.Add(LegendPosition.Left);
      cbLegendPosition.Items.Add(LegendPosition.Right);

      Model = new ChartDataRowsModel();
    }

    public IChartDataRowsModel Model {
      get { return model; }
      set {
        model = value;
        oldTitle = model.Title;
        tbxTitle.Text = model.Title;
        viewSettings = model.ViewSettings;
        oldViewSettings = new ViewSettings(model.ViewSettings);
        cbLegendPosition.SelectedItem = viewSettings.LegendPosition;
      }
    }

    public void ResetSettings() {
      model.ShowXAxisGrid = oldShowXAxisGrid;

      foreach (var param in oldLineParams) {
        param.applySettings();
      }

      foreach (var box in ShowYAxisBoxes) {
        box.Key.Checked = box.Value;
      }

      foreach (KeyValuePair<CheckBox, bool> box in yAxisClipChangeableBoxes) {
        box.Key.Checked = box.Value;
      }

      model.Title = oldTitle;
      tbxTitle.Text = oldTitle;

      viewSettings.LegendColor = oldViewSettings.LegendColor;
      viewSettings.LegendPosition = oldViewSettings.LegendPosition;
      viewSettings.LegendFont = oldViewSettings.LegendFont;
      viewSettings.TitleColor = oldViewSettings.TitleColor;
      viewSettings.TitleFont = oldViewSettings.TitleFont;
      viewSettings.XAxisColor = oldViewSettings.LegendColor;
      viewSettings.XAxisFont = oldViewSettings.XAxisFont;
      viewSettings.UpdateView();
      cbLegendPosition.SelectedItem = viewSettings.LegendPosition;
      this.LineSelectCB_SelectedIndexChanged(this, null);
    }

    private void OptionsDialogSelectColorBtn_Click(object sender, EventArgs e) {
      ColorDialog dlg = new ColorDialog();
      dlg.ShowDialog();
      ColorPreviewTB.BackColor = dlg.Color;
      ((IDataRow)LineSelectCB.SelectedValue).Color = dlg.Color;
    }

    private IList<int> GetThicknesses() {
      return new List<int>(new[] {1, 2, 3, 4, 5, 6, 7, 8});
    }

    private IList<DrawingStyle> GetStyles() {
      return new List<DrawingStyle>(new[] {DrawingStyle.Solid, DrawingStyle.Dashed});
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

    private void btnChangeTitleFont_Click(object sender, EventArgs e) {
      fdFont.Font = viewSettings.TitleFont;
      fdFont.Color = viewSettings.TitleColor;

      DialogResult dr = fdFont.ShowDialog();

      if (dr == DialogResult.OK) {
        viewSettings.TitleFont = fdFont.Font;
        viewSettings.TitleColor = fdFont.Color;

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

    private void Options_Load(object sender, EventArgs e) {
      oldShowXAxisGrid = model.ShowXAxisGrid;
      chkShowXAxisGrid.Checked = model.ShowXAxisGrid;

      InitTabPageLines();
      InitTabPageYAxes();
    }

    private void InitTabPageLines() {
      if (model.Rows.Count != 0) {
        int index = 0;
        oldLineParams = new LineParams[model.Rows.Count];
        foreach (var row in model.Rows) {
          oldLineParams[index++] = new LineParams(row);
        }
        LineThicknessCB.DataSource = GetThicknesses();
        LinestyleCB.DataSource = GetStyles();
        LinestyleCB.SelectedItem = model.Rows[0].Style;
        LineThicknessCB.SelectedItem = model.Rows[0].Thickness;
        MarkercheckBox.Checked = model.Rows[0].ShowMarkers;

        LineSelectCB.DataSource = model.Rows;
        LineSelectCB.DisplayMember = "Label";


        LineSelectCB.SelectedIndex = 0;
        LineSelectCB_SelectedIndexChanged(this, null);
      }
    }

    private void InitTabPageYAxes() {
      ShowYAxisBoxes = new Dictionary<CheckBox, bool>();
      yAxisClipChangeableBoxes = new Dictionary<CheckBox, bool>();

      for (int i = 0; i < model.YAxes.Count; i++) {
        YAxisDescriptor yAxisDescriptor = model.YAxes[i];

        CheckBox cbxShowYAxis = new CheckBox();
        cbxShowYAxis.Text = yAxisDescriptor.Label;
        cbxShowYAxis.Checked = yAxisDescriptor.ShowYAxis;
        ShowYAxisBoxes[cbxShowYAxis] = yAxisDescriptor.ShowYAxis;
        cbxShowYAxis.CheckedChanged += delegate { yAxisDescriptor.ShowYAxis = cbxShowYAxis.Checked; };

        flpShowYAxis.Controls.Add(cbxShowYAxis);

        CheckBox cbxClipChangeable = new CheckBox();
        cbxClipChangeable.Text = yAxisDescriptor.Label;
        cbxClipChangeable.Checked = yAxisDescriptor.ClipChangeable;
        yAxisClipChangeableBoxes[cbxClipChangeable] = yAxisDescriptor.ClipChangeable;
        cbxClipChangeable.CheckedChanged += delegate { yAxisDescriptor.ClipChangeable = cbxClipChangeable.Checked; };

        flpYAxisClipChangeable.Controls.Add(cbxClipChangeable);
      }
    }

    private void LineSelectCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
        /*  int index =
            LineThicknessCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Thickness.ToString());
          LineThicknessCB.SelectedIndex = index;
          index = LinestyleCB.FindStringExact(((IDataRow)LineSelectCB.SelectedValue).Style.ToString());
          LinestyleCB.SelectedIndex = index;  */
        LineThicknessCB.SelectedItem = ((IDataRow)LineSelectCB.SelectedValue).Thickness;
        LinestyleCB.SelectedItem = ((IDataRow)LineSelectCB.SelectedValue).Style;
        ColorPreviewTB.BackColor = ((IDataRow)LineSelectCB.SelectedValue).Color;
        MarkercheckBox.Checked = ((IDataRow)LineSelectCB.SelectedValue).ShowMarkers;
      }
    }

    private void cbLegendPosition_SelectedIndexChanged(object sender, EventArgs e) {
      viewSettings.LegendPosition = (LegendPosition)cbLegendPosition.SelectedItem;
      viewSettings.UpdateView();
    }

    private void LinestyleCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
        ((IDataRow)LineSelectCB.SelectedValue).Style = (DrawingStyle)LinestyleCB.SelectedItem;
      }
    }

    private void LineThicknessCB_SelectedIndexChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
        ((IDataRow)LineSelectCB.SelectedValue).Thickness = (int)LineThicknessCB.SelectedItem;
      }
    }

    private void MarkercheckBox_CheckedChanged(object sender, EventArgs e) {
      if (LineSelectCB.SelectedValue != null) {
        ((IDataRow)LineSelectCB.SelectedValue).ShowMarkers = MarkercheckBox.Checked;
      }
    }

    private void chkShowXAxisGrid_CheckedChanged(object sender, EventArgs e) {
      model.ShowXAxisGrid = chkShowXAxisGrid.Checked;
    }

    private void tbxTitle_TextChanged(object sender, EventArgs e) {
      model.Title = tbxTitle.Text;
      model.ViewSettings.UpdateView();
    }
  }
}