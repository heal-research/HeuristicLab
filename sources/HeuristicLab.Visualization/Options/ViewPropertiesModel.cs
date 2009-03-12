using System.Drawing;

namespace HeuristicLab.Visualization.Options {
  public class ViewPropertiesModel {
    public event UpdatePropertiesHandler OnUpdateProperties;

    private Font titleFont;
    private Color titleColor;
    private Font legendFont;
    private Color legendColor;
    private Font xAxisFont;
    private Color xAxisColor;

    public ViewPropertiesModel(Font titleFont, Color titleColor, Font legendFont, Color legendColor, Font xAxisFont, Color xAxisColor) {
      this.titleFont = titleFont;
      this.titleColor = titleColor;
      this.legendFont = legendFont;
      this.legendColor = legendColor;
      this.xAxisFont = xAxisFont;
      this.xAxisColor = xAxisColor;
    }

    public void UpdateView() {
      if (OnUpdateProperties != null) {
        OnUpdateProperties();
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

    public Font XAxisFont {
      get { return xAxisFont; }
      set { xAxisFont = value; }
    }

    public Color XAxisColor {
      get { return xAxisColor; }
      set { xAxisColor = value; }
    }
  }

  public delegate void UpdatePropertiesHandler();
}