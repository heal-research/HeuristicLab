using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Icon shape material without <see cref="IMouseListener"/> service.
  /// <seealso cref="ClickableIconMaterial"/>
  /// </summary>
  public partial class IconLabelMaterial : ShapeMaterialBase {

    /// <summary>
    /// The distance between the icon and the text.
    /// </summary>
    public const int constTextShift = 2;

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// IconLabelMaterial.
    /// </summary>
    // ------------------------------------------------------------------
    protected double iconLabelMaterialVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Text field
    /// </summary>
    // ------------------------------------------------------------------
    private Bitmap mIcon;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Text field
    /// </summary>
    // ------------------------------------------------------------------
    private string mText = string.Empty;

    private Rectangle textRectangle = Rectangle.Empty;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return iconLabelMaterialVersion;
      }
    }

    /// <summary>
    /// Gets the bounds of the text.
    /// </summary>
    public Rectangle TextArea {
      get {
        return textRectangle;
      }
    }

    /// <summary>
    /// Gets or sets the Text
    /// </summary>
    public string Text {
      get {
        return mText;
      }
      set {
        mText = value;
      }
    }

    /// <summary>
    /// Gets or sets the Text
    /// </summary>
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
    public IconLabelMaterial(string text)
      : base() {
      mText = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IconLabelMaterial"/> class.
    /// </summary>
    /// <param name="resourceLocation">The resource location.</param>
    /// <param name="text">The text.</param>
    public IconLabelMaterial(string text, string resourceLocation)
      : this(text) {
      mIcon = GetBitmap(resourceLocation);
    }

    /// <summary>
    /// Opens the icon (image) from the location specified and sets it
    /// as the image to use.
    /// </summary>
    /// <param name="resourceLocation">string: The resource location.
    /// </param>
    public void SetIcon(string resourceLocation) {
      mIcon = GetBitmap(resourceLocation);
    }

    /// <summary>
    /// Gets the bitmap at the location specified.
    /// </summary>
    /// <param name="resourceLocation">The resource location.</param>
    /// <returns></returns>
    protected Bitmap GetBitmap(string resourceLocation) {
      if (resourceLocation.Length == 0)
        return null;
      try {
        //first try if it's defined in this assembly somewhere                
        return new Bitmap(this.GetType(), resourceLocation);
      }
      catch {

        if (File.Exists(resourceLocation)) {
          return new Bitmap(resourceLocation);
        } else
          return null;
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:IconLabelMaterial"/> class.
    /// </summary>
    public IconLabelMaterial()
      : base() {
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the min size needed to fit this material in.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <returns>Size</returns>
    // ------------------------------------------------------------------
    public override Size CalculateMinSize(Graphics g) {
      Size minSizeNeeded = new Size(0, 0);

      if (mText != String.Empty) {
        minSizeNeeded = Size.Round(g.MeasureString(
        this.myTextStyle.GetFormattedText(this.mText),
        this.myTextStyle.Font));
      }

      minSizeNeeded.Width += constTextShift;

      if (mIcon != null) {
        minSizeNeeded.Width += mIcon.Width;

        if (mIcon.Height > minSizeNeeded.Height) {
          minSizeNeeded.Height = mIcon.Height;
        }
      }
      return minSizeNeeded;
    }

    public override void Transform(Rectangle rectangle) {
      textRectangle = new Rectangle(
          rectangle.X + (mIcon == null ? 0 : mIcon.Width) + constTextShift,
          rectangle.Y,
          rectangle.Width - (mIcon == null ? 0 : mIcon.Width) - constTextShift,
          rectangle.Height);
      base.Transform(rectangle);

    }
    /// <summary>
    /// Paints the entity using the given graphics object
    /// </summary>
    /// <param name="g"></param>
    public override void Paint(Graphics g) {
      if (!Visible)
        return;
      GraphicsContainer cto = g.BeginContainer();
      g.SetClip(Shape.Rectangle);
      if (mIcon != null) {
        g.DrawImage(mIcon, new Rectangle(Rectangle.Location, mIcon.Size));
      }

      StringFormat stringFormat = myTextStyle.StringFormat;
      stringFormat.Trimming = StringTrimming.EllipsisWord;
      stringFormat.FormatFlags = StringFormatFlags.LineLimit;

      g.DrawString(
          myTextStyle.GetFormattedText(mText),
          this.myTextStyle.Font,
          this.myTextStyle.GetBrush(),
          textRectangle,
          stringFormat);
      g.EndContainer(cto);
    }
    #endregion

  }
}
