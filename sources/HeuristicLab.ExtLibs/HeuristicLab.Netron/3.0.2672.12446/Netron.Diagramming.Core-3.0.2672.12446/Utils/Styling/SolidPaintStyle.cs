using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The solid paint style.
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class SolidPaintStyle : IPaintStyle, IVersion {
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
    /// SolidPaintStyle.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double solidPaintStyleVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the SolidColor field
    /// </summary>
    // ------------------------------------------------------------------
    private Color mSolidColor;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Alpha field
    /// </summary>
    // ------------------------------------------------------------------
    private int mAlpha = 255;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return solidPaintStyleVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the SolidColor
    /// </summary>
    // ------------------------------------------------------------------
    public Color SolidColor {
      get {
        return mSolidColor;
      }
      set {
        mSolidColor = value;
        RaisePaintStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Alpha
    /// </summary>
    // ------------------------------------------------------------------
    public int Alpha {
      get {
        return mAlpha;
      }
      set {
        mAlpha = value;
        RaisePaintStyleChanged();
      }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    ///<summary>
    ///Default constructor.
    ///</summary>
    // ------------------------------------------------------------------
    public SolidPaintStyle(Color color) {
      mSolidColor = color;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:SolidPaintStyle"/> class.
    /// </summary>
    // ------------------------------------------------------------------
    public SolidPaintStyle() {
      mSolidColor = ArtPalette.RandomLowSaturationColor;
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the brush with which an entity can fe painted.
    /// </summary>
    /// <param name="rectangle">Rectangle: The rectangle.</param>
    /// <returns>Brush</returns>
    // ------------------------------------------------------------------
    public Brush GetBrush(Rectangle rectangle) {
      return new SolidBrush(Color.FromArgb(mAlpha, mSolidColor));
    }

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
            new PaintStyleChangedEventArgs(this, FillType.Solid));
      }
    }
  }
}
