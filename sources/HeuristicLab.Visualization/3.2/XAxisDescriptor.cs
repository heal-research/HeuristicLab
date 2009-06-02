using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public delegate void XAxisDescriptorChangedHandler(XAxisDescriptor sender);

  public class XAxisDescriptor {
    private string label = "";
    private Font font = new Font("Arial", 8);
    private Color color = Color.Blue;
    private bool showLabel = true;
    private bool showGrid = true;
    private Color gridColor = Color.LightBlue;
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.##");

    public event XAxisDescriptorChangedHandler XAxisDescriptorChanged;

    public string Label {
      get { return label; }
      set {
        label = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Font Font {
      get { return font; }
      set {
        font = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
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

    public bool ShowGrid {
      get { return showGrid; }
      set {
        this.showGrid = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Color GridColor {
      get { return this.gridColor; }
      set {
        this.gridColor = value;
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

    private void FireXAxisDescriptorChanged() {
      if (XAxisDescriptorChanged != null)
        XAxisDescriptorChanged(this);
    }
  }
}