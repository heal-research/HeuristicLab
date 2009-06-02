using System.Drawing;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Options {
  public class ViewSettings {
    public event UpdateViewSettingsHandler OnUpdateSettings;

    private Font titleFont;
    private Color titleColor;
    private Font legendFont;
    private Color legendColor;
    private LegendPosition legendPosition;

    public ViewSettings() {
      titleFont = new Font("Arial", 8);
      titleColor = Color.Blue;

      legendFont = new Font("Arial", 8);
      legendColor = Color.Blue;

      legendPosition = LegendPosition.Right;
    }

    public ViewSettings(ViewSettings src) {
      
      titleFont = (Font)(src.titleFont.Clone());
      titleColor = src.TitleColor;

      legendFont = (Font)(src.legendFont.Clone());
      legendColor = src.LegendColor;

      legendPosition = src.LegendPosition;
    }

    public void UpdateView() {
      if (OnUpdateSettings != null) {
        OnUpdateSettings();
      }
    }

    public Font TitleFont {
      get { return titleFont; }
      set { titleFont = value; }
    }

    public Color TitleColor {
      get { return titleColor; }
      set { titleColor = value; }
    }

    public Font LegendFont {
      get { return legendFont; }
      set { legendFont = value; }
    }

    public Color LegendColor {
      get { return legendColor; }
      set { legendColor = value; }
    }

    public LegendPosition LegendPosition {
      get { return legendPosition; }
      set { legendPosition = value; }
    }
  }

  public delegate void UpdateViewSettingsHandler();
}