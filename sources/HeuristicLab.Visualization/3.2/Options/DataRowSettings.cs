using System.Drawing;

namespace HeuristicLab.Visualization.Options {
  public class DataRowSettings {
    public event UpdateDataRowSettingsHandler OnUpdateDataRowSettings;

    private string label;
    private Color color;
    private int thickness;
    private DrawingStyle style;
    private DataRowType lineType;
    private bool showMarkers;

    public DataRowSettings() {
      label = "";
      color = Color.Black;
      thickness = 2;
      style = DrawingStyle.Solid;
      lineType = DataRowType.Normal;
      showMarkers = true;
    }

    public DataRowSettings(DataRowSettings src) {
      label = src.label;
      color = src.color;
      thickness = src.thickness;
      style = src.style;
      lineType = src.lineType;
      showMarkers = src.showMarkers;
    }

    public void UpdateView() {
      if (OnUpdateDataRowSettings != null) {
        OnUpdateDataRowSettings();
      }
    }

//    public bool ShowMarkers {
//      get { return showMarkers; }
//      set {
//        showMarkers = value;
//        //OnDataRowChanged(this);
//      }
//    }

    public string Label {
      get { return label; }
      set {
        label = value;
        //OnDataRowChanged(this);
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        //OnDataRowChanged(this);
      }
    }

    public int Thickness {
      get { return thickness; }
      set {
        thickness = value;
        //OnDataRowChanged(this);
      }
    }

//    public DrawingStyle Style {
//      get { return style; }
//      set {
//        style = value;
//        //OnDataRowChanged(this);
//      }
//    }
//
//    public DataRowType LineType {
//      get { return lineType; }
//      set {
//        lineType = value;
//        //OnDataRowChanged(this);
//      }
//    }
  }
  public delegate void UpdateDataRowSettingsHandler();
}
