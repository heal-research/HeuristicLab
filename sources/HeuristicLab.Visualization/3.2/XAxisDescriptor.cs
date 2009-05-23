using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public delegate void XAxisDescriptorChangedHandler(XAxisDescriptor sender);

  public class XAxisDescriptor {
    private string xAxisLabel = "";
    private bool showXAxisLabel = true;
    private bool showXAxisGrid = true;
    private ILabelProvider xAxisLabelProvider = new ContinuousLabelProvider("0.##");

    public event XAxisDescriptorChangedHandler XAxisDescriptorChanged;

    public bool ShowXAxisGrid {
      get { return showXAxisGrid; }
      set {
        this.showXAxisGrid = value;
        FireXAxisDescriptorChanged();
      }
    }

    public ILabelProvider XAxisLabelProvider {
      get { return xAxisLabelProvider; }
      set {
        this.xAxisLabelProvider = value;
        FireXAxisDescriptorChanged();
      }
    }

    public string XAxisLabel {
      get { return xAxisLabel; }
      set {
        xAxisLabel = value;
        FireXAxisDescriptorChanged();
      }
    }

    public bool ShowXAxisLabel {
      get { return showXAxisLabel; }
      set {
        showXAxisLabel = value;
        FireXAxisDescriptorChanged();
      }
    }

    private void FireXAxisDescriptorChanged() {
      if (XAxisDescriptorChanged != null)
        XAxisDescriptorChanged(this);
    }
  }
}