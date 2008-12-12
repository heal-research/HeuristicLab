using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.Test {
  public partial class LineChartTestForm : Form {
    private ChartDataRowsModel model;
    private IView view;

    public LineChartTestForm() {
      InitializeComponent();

      model = new ChartDataRowsModel();

      
      view = model.CreateView();

      Control viewControl = (Control)view;
      viewControl.Dock = DockStyle.Fill;

      lineChartGroupBox.Controls.Add(viewControl);
    }

    public ChartDataRowsModel Model {
      get { return model; }
    }

    private void btnResetView_Click(object sender, System.EventArgs e) {
      LineChart lineChart = (LineChart)view;
      lineChart.ResetView();
    }
  }
}