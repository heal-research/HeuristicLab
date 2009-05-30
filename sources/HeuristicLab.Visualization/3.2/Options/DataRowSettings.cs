using System.Drawing;

namespace HeuristicLab.Visualization.Options {
  public class DataRowSettings {
    public event UpdateDataRowSettingsHandler OnUpdateDataRowSettings;
    public event DataVisualSettingChangedHandler DataVisualSettingChanged;

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

    public string Label {
      get { return label; }
      set {
        label = value;
        OnDataVisualSettingChanged(this);
      }
    }
    
    public Color Color {
      get { return color; }
      set {
        color = value;
        OnDataVisualSettingChanged(this);
      }
    }

    public int Thickness {
      get { return thickness; }
      set {
        thickness = value;
        OnDataVisualSettingChanged(this);
      }
    }

    public DrawingStyle Style {
      get { return style; }
      set {
        style = value;
        OnDataVisualSettingChanged(this);
      }
    }

    public DataRowType LineType {
      get { return lineType; }
      set {
        lineType = value;
        OnDataVisualSettingChanged(this);
      }
    }

    public bool ShowMarkers {
      get { return showMarkers; }
      set {
        showMarkers = value;
        OnDataVisualSettingChanged(this);
      }
    }
    
    protected void OnDataVisualSettingChanged(DataRowSettings row) {
      if (DataVisualSettingChanged != null) {
        DataVisualSettingChanged(this);
      }
    }

  }
  public delegate void UpdateDataRowSettingsHandler();
  public delegate void DataVisualSettingChangedHandler(DataRowSettings row);
}
