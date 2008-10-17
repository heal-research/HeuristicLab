using System;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class LineChart : UserControl {
    private IChartDataRowsModel model;

    public LineChart() {
      InitializeComponent();
    }

    public IChartDataRowsModel Model {
      get { return model; }
      set {
        if (value == null) {
          throw new NullReferenceException("Model cannot be null.");
        }

        if (model != null) {
          throw new InvalidOperationException("Model has already been set.");
        }

        model = value;

        model.DataChanged += OnDataChanged;
      }
    }

    private void OnDataChanged(ChangeType type, int dataId, double[] values) {
      throw new NotImplementedException();
    }
  }
}