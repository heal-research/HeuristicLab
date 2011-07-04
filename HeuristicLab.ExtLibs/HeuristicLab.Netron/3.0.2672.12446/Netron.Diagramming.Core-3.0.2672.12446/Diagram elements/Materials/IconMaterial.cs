using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Icon shape material without <see cref="IMouseListener"/> service.
  /// <seealso cref="ClickableIconMaterial"/>
  /// </summary>
  public partial class IconMaterial : ShapeMaterialBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// IconMaterial.
    /// </summary>
    // ------------------------------------------------------------------
    protected double iconMaterialVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Text field
    /// </summary>
    // ------------------------------------------------------------------
    private Bitmap mIcon;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return iconMaterialVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Text
    /// </summary>
    // ------------------------------------------------------------------
    public Bitmap Icon {
      get {
        return mIcon;
      }
      set {
        mIcon = value;
      }
    }

    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="IconMaterial"/> class.
    /// </summary>
    /// <param name="resourceLocation">The resource location.</param>
    public IconMaterial(string resourceLocation)
      : base() {

      mIcon = GetBitmap(resourceLocation);
    }

    protected Bitmap GetBitmap(string resourceLocation) {
      if (resourceLocation.Length == 0)
        throw new InconsistencyException("Invalid icon specification.");
      try {
        return new Bitmap(this.GetType(), resourceLocation);
      }
      catch {

        throw;
      }
    }
    public IconMaterial()
      : base() {
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the min size needed to fit this material in.  The
    /// min size is determined by the size of the icon used.  If there
    /// isn't an icon specified, then 'Size.Empty' is returned.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <returns>Size</returns>
    // ------------------------------------------------------------------
    public override Size CalculateMinSize(Graphics g) {
      Size minSizeNeeded = Size.Empty;

      if (mIcon != null) {
        minSizeNeeded = new Size(mIcon.Width, mIcon.Height);
      }
      return minSizeNeeded;
    }

    /// <summary>
    /// Paints the entity using the given graphics object
    /// </summary>
    /// <param name="g"></param>
    public override void Paint(Graphics g) {
      if (!Visible)
        return;

      if (mIcon != null) {
        GraphicsContainer cto = g.BeginContainer();
        g.SetClip(Shape.Rectangle);
        g.DrawImage(mIcon, Rectangle);
        g.EndContainer(cto);
      }
    }
    #endregion

  }
}
