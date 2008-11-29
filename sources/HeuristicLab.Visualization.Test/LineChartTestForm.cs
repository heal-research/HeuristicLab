using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization.Test {
  public partial class LineChartTestForm : Form {
    private ChartDataRowsModel model;

    public LineChartTestForm() {
      InitializeComponent();

      model = new ChartDataRowsModel();

      IView view = model.CreateView();

      Control viewControl = (Control)view;
      viewControl.Dock = DockStyle.Fill;

      lineChartGroupBox.Controls.Add(viewControl);
    }

    public ChartDataRowsModel Model {
      get { return model; }
    }
  }
}