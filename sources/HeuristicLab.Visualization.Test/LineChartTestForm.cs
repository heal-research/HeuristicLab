using System;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.Test {
  public partial class LineChartTestForm : Form {
    private readonly IView view;
    private ChartDataRowsModel model;

    public LineChartTestForm() {
      InitializeComponent();
    }

    public LineChartTestForm(ChartDataRowsModel model) : this() {
      this.model = model;
      view = model.CreateView();

      Control viewControl = (Control)view;
      viewControl.Dock = DockStyle.Fill;

      lineChartGroupBox.Controls.Add(viewControl);
    }

    private void btnResetView_Click(object sender, EventArgs e) {
      if (view != null) {
        LineChart lineChart = (LineChart)view;
        lineChart.ZoomToFullView();
      }
    }

    private void btnAddRandomValue_Click(object sender, EventArgs e) {
      Random rand = new Random();

      foreach (IDataRow row in model.Rows)
        row.AddValue(rand.NextDouble()*100);
    }
  }
}