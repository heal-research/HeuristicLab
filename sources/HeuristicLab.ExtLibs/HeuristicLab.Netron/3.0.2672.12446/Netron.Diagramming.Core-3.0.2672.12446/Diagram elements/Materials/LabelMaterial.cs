using System;
using System.Drawing;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Simple label material
  /// </summary>
  public partial class LabelMaterial : ShapeMaterialBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// LabelMaterial.
    /// </summary>
    // ------------------------------------------------------------------
    protected double labelMaterialVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Text field
    /// </summary>
    // ------------------------------------------------------------------
    private string mText;
    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return labelMaterialVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the font for this label.
    /// </summary>
    // ------------------------------------------------------------------
    public Font Font {
      get {
        return this.myTextStyle.Font;
      }
      set {
        this.myTextStyle.Font = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Text
    /// </summary>
    // ------------------------------------------------------------------
    public string Text {
      get {
        return mText;
      }
      set {
        mText = value;
      }
    }

    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor
    ///</summary>
    public LabelMaterial()
      : base() {

    }
    public LabelMaterial(string text)
      : this() {
      this.mText = text;
    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the min size needed to fit the text in.  If there is
    /// no text assigned, then 'Size.Empty' is returned.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <returns>Size</returns>
    // ------------------------------------------------------------------
    public override Size CalculateMinSize(Graphics g) {
      Size minSizeNeeded = Size.Empty;

      if (mText != String.Empty) {
        minSizeNeeded = Size.Round(
            g.MeasureString(mText, this.myTextStyle.Font));
      }
      return minSizeNeeded;
    }

    /// <summary>
    /// Paints the material using the given graphics object.
    /// </summary>
    /// <param name="g"></param>
    public override void Paint(System.Drawing.Graphics g) {
      if (!Visible) return;
      //the text		
      if (!string.IsNullOrEmpty(Text)) {
        //g.DrawRectangle(Pens.Orange, Rectangle);
        StringFormat stringFormat = myTextStyle.StringFormat;
        stringFormat.Trimming = StringTrimming.EllipsisWord;
        stringFormat.FormatFlags = StringFormatFlags.LineLimit;

        g.DrawString(
            this.myTextStyle.GetFormattedText(Text),
            this.myTextStyle.Font,
            myTextStyle.GetBrush(),
            Rectangle,
            stringFormat);
      }
    }
    #endregion

  }
}
