using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public delegate void XAxisDescriptorChangedHandler(XAxisDescriptor sender);

  public class XAxisDescriptor {
    private string label = "";
    private bool showLabel = true;
    private bool showGrid = true;
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.##");

    public event XAxisDescriptorChangedHandler XAxisDescriptorChanged;

    public bool ShowGrid {
      get { return showGrid; }
      set {
        this.showGrid = value;
        FireXAxisDescriptorChanged();
      }
    }

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set {
        this.labelProvider = value;
        FireXAxisDescriptorChanged();
      }
    }

    public string Label {
      get { return label; }
      set {
        label = value;
        FireXAxisDescriptorChanged();
      }
    }

    public bool ShowLabel {
      get { return showLabel; }
      set {
        showLabel = value;
        FireXAxisDescriptorChanged();
      }
    }

    private void FireXAxisDescriptorChanged() {
      if (XAxisDescriptorChanged != null)
        XAxisDescriptorChanged(this);
    }
  }
}