using System;
using System.ComponentModel;
using System.Drawing;
using ToolBox.Formatting;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Specifies formatting for text to be drawn (such as color, alignment,
  /// font size, etc.).  The event 'TextStyleChanged' is raised when a
  /// property is changed.
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class TextStyle : ITextStyle {
    // ------------------------------------------------------------------
    /// <summary>
    /// Event raised when this TextStyle is changed.
    /// </summary>
    // ------------------------------------------------------------------
    [field: NonSerialized()]
    public event TextStyleChangedEventHandler TextStyleChanged;

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// TextStyle.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double textStyleVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies the number of decimal places to use when a TextFormat
    /// other than 'String' is specified.
    /// </summary>
    // ------------------------------------------------------------------
    protected int mDecimalPlaces = 2;

    // ------------------------------------------------------------------
    /// <summary>
    /// The formatting to apply to the text.
    /// </summary>
    // ------------------------------------------------------------------
    protected TextFormat mTextFormat = TextFormat.String;

    // ------------------------------------------------------------------
    /// <summary>
    /// The font.
    /// </summary>
    // ------------------------------------------------------------------
    protected Font mFont = new Font("Arial", 10);

    // ------------------------------------------------------------------
    /// <summary>
    /// The fontstyle to use (regular, bold, italics, etc.).
    /// </summary>
    // ------------------------------------------------------------------
    protected FontStyle mFontStyle = FontStyle.Regular;

    // ------------------------------------------------------------------
    /// <summary>
    /// The color for the text;
    /// </summary>
    // ------------------------------------------------------------------
    protected Color mFontColor = Color.Black;

    // ------------------------------------------------------------------
    /// <summary>
    /// The horizontal alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    protected StringAlignment mHorizontalAlignment =
        StringAlignment.Center;

    // ------------------------------------------------------------------
    /// <summary>
    /// The vertical alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    protected StringAlignment mVerticalAlignment =
        StringAlignment.Center;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the font is bold.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mIsBold = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the font is italic.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mIsItalic = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the font is underlined.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mIsUnderline = false;

    #region ITextStyle Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return textStyleVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of decimal places to use when a TextFormat
    /// other than 'String' is specified.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual int DecimalPlaces {
      get {
        return mDecimalPlaces;
      }
      set {
        mDecimalPlaces = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the formatting to apply to the text.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual TextFormat TextFormat {
      get {
        return mTextFormat;
      }
      set {
        mTextFormat = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's font.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Font Font {
      get {
        return this.mFont;
      }
      set {
        this.mFont = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's font size.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual float FontSize {
      get {
        return this.mFont.Size;
      }
      set {
        this.mFont = new Font(
            mFont.FontFamily,
            value,
            mFont.Style);
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's color.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Color FontColor {
      get {
        return mFontColor;
      }
      set {
        mFontColor = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is underlined.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool IsUnderlined {
      get {
        return mFont.Underline;
      }
      set {
        mIsUnderline = value;
        SetFontStyle();
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is bold.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool IsBold {
      get {
        return mFont.Bold;
      }
      set {
        mIsBold = value;
        SetFontStyle();
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is italic.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool IsItalic {
      get {
        return mFont.Italic;
      }
      set {
        mIsItalic = value;
        SetFontStyle();
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the horizontal alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(true),
    Description("The horizontal alignment of the text."),
    Category("Layout")]
    public virtual StringAlignment HorizontalAlignment {
      get {
        return mHorizontalAlignment;
      }
      set {
        mHorizontalAlignment = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the vertical alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(true),
    Description("The vertical alignment of the text."),
    Category("Layout")]
    public virtual StringAlignment VerticalAlignment {
      get {
        return mVerticalAlignment;
      }
      set {
        mVerticalAlignment = value;
        RaiseTextStyleChanged();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the format of the string.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual StringFormat StringFormat {
      get {
        StringFormat format = new StringFormat();
        format.Alignment = this.VerticalAlignment;
        format.LineAlignment = this.HorizontalAlignment;
        return format;
      }
    }

    #endregion

    #region Constructors

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public TextStyle() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor that receives the color of the font.
    /// </summary>
    /// <param name="color">Color</param>
    // ------------------------------------------------------------------
    public TextStyle(Color color) {
      mFontColor = color;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor that receives the font color, font, horizontal
    /// alignment, and vertical alignment.
    /// </summary>
    /// <param name="color">Color</param>
    /// <param name="font">Font</param>
    /// <param name="horizontalAlignment">StringAlignment</param>
    /// <param name="verticalAlignment">StringAlignment</param>
    // ------------------------------------------------------------------
    public TextStyle(
        Color color,
        Font font,
        StringAlignment horizontalAlignment,
        StringAlignment verticalAlignment) {
      mFontColor = color;
      mFont = font;
      mHorizontalAlignment = horizontalAlignment;
      mVerticalAlignment = verticalAlignment;
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates a new font style using the current IsBold, IsItalic,
    /// and IsUnderline properties.
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void SetFontStyle() {
      FontStyle style = FontStyle.Regular;

      if (mIsBold) {
        style = style | FontStyle.Bold;
      }

      if (mIsItalic) {
        style = style | FontStyle.Italic;
      }

      if (mIsUnderline) {
        style = style | FontStyle.Underline;
      }

      this.mFont = new Font(mFont, style);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the brush that's used to draw our text to a GDI+ graphics
    /// surface.
    /// </summary>
    /// <returns>Brush</returns>
    // ------------------------------------------------------------------
    public virtual Brush GetBrush() {
      return new SolidBrush(mFontColor);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Applies the formatting specified by TextFormatting to the text
    /// specified.
    /// </summary>
    /// <param name="text">string: The un-formatted text.</param>
    /// <returns>string: The formatted text.</returns>
    // ------------------------------------------------------------------
    public virtual string GetFormattedText(string text) {
      return TextFormatter.Format(
          mTextFormat,
          text,
          mDecimalPlaces);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the TextStyleChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void RaiseTextStyleChanged() {
      if (this.TextStyleChanged != null) {
        // Raise the event
        this.TextStyleChanged(
            this,
            new TextStyleChangedEventArgs(this));
      }
    }
  }
}
