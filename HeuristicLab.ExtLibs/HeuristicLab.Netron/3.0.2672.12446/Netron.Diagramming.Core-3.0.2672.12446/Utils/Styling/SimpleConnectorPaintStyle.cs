using System.Drawing;

namespace Netron.Diagramming.Core {
  public partial class SimpleConnectorPaintStyle : IPaintStyle {
    // ------------------------------------------------------------------
    /// <summary>
    /// Event raised when this paint style is changed.
    /// </summary>
    // ------------------------------------------------------------------
    public event PaintStyleChangedEventHandler PaintStyleChanged;

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// SimpleConnectorPaintStyle.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double simpleConnectorPaintStyleVersion = 1.0;

    Color mSolidColor = Color.Transparent;

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return simpleConnectorPaintStyleVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public SimpleConnectorPaintStyle() {
    }

    #region IPaintStyle Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns a brush that has a transparent color.
    /// </summary>
    /// <param name="rectangle">Rectangle</param>
    /// <returns>Brush</returns>
    // ------------------------------------------------------------------
    public Brush GetBrush(Rectangle rectangle) {
      return new SolidBrush(mSolidColor);
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the PaintStyleChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void RaisePaintStyleChanged() {
      if (this.PaintStyleChanged != null) {
        // Raise the event
        this.PaintStyleChanged(
            this,
            new PaintStyleChangedEventArgs(
            this,
            FillType.LinearGradient));
      }
    }
  }
}
