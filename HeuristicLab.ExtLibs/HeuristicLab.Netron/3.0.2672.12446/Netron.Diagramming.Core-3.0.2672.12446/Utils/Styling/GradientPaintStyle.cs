using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The gradient paint style
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class GradientPaintStyle :
      IPaintStyle,
      IVersion {
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
    /// GradientPaintStyle.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double gradientPaintStyleVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Angle field.
    /// </summary>
    // ------------------------------------------------------------------
    private float mAngle;

    // ------------------------------------------------------------------
    /// <summary>
    /// the EndColor field
    /// </summary>
    // ------------------------------------------------------------------
    private Color mEndColor;

    // ------------------------------------------------------------------
    /// <summary>
    /// the StartColor field
    /// </summary>
    // ------------------------------------------------------------------
    private Color mStartColor;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return gradientPaintStyleVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the StartColor
    /// </summary>
    // ------------------------------------------------------------------
    public Color StartColor {
      get {
        return mStartColor;
      }
      set {
        mStartColor = value;
        RaisePaintStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the EndColor
    /// </summary>
    // ------------------------------------------------------------------
    public Color EndColor {
      get {
        return mEndColor;
      }
      set {
        mEndColor = value;
        RaisePaintStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Angle
    /// </summary>
    // ------------------------------------------------------------------
    public float Angle {
      get {
        return mAngle;
      }
      set {
        mAngle = value;
        RaisePaintStyleChanged();
      }
    }
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    ///<summary>
    ///Default constructor
    ///</summary>
    // ------------------------------------------------------------------
    public GradientPaintStyle(
        Color startColor,
        Color endColor,
        float angle) {
      mStartColor = startColor;
      mEndColor = endColor;
      mAngle = angle;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:GradientPaintStyle"/> class.
    /// </summary>
    // ------------------------------------------------------------------
    public GradientPaintStyle() {
      mStartColor = ArtPalette.RandomLowSaturationColor;
      mEndColor = Color.White;
      mAngle = -135;
    }
    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the brush.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>Brush</returns>
    // ------------------------------------------------------------------
    public Brush GetBrush(Rectangle rectangle) {
      if (rectangle.Equals(Rectangle.Empty)) {
        return new SolidBrush(mStartColor);
      } else {
        if (rectangle.Width == 0) {
          rectangle.Width = 1;
        }

        if (rectangle.Height == 0) {
          rectangle.Height = 1;
        }
        return new LinearGradientBrush(
            rectangle,
            mStartColor,
            mEndColor,
            mAngle,
            true);
      }
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
            new PaintStyleChangedEventArgs(
            this,
            FillType.LinearGradient));
      }
    }
  }
}
