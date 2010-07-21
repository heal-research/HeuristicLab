using System.Drawing;
using System.Runtime.Serialization;

using ToolBox.Formatting;

namespace Netron.Diagramming.Core {
  public interface ITextStyle : ISerializable, IVersion {
    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the text style is changed.
    /// </summary>
    // ------------------------------------------------------------------
    event TextStyleChangedEventHandler TextStyleChanged;

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of decimal places to use when a TextFormat
    /// other than 'String' is specified.
    /// </summary>
    // ------------------------------------------------------------------
    int DecimalPlaces {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the formatting to apply to the text.
    /// </summary>
    // ------------------------------------------------------------------
    TextFormat TextFormat {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the format of the string.
    /// </summary>
    // ------------------------------------------------------------------
    StringFormat StringFormat {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the horizontal alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    StringAlignment HorizontalAlignment {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the vertical alignment of the text.
    /// </summary>
    // ------------------------------------------------------------------
    StringAlignment VerticalAlignment {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's font.
    /// </summary>
    // ------------------------------------------------------------------
    Font Font {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's font size.
    /// </summary>
    // ------------------------------------------------------------------
    float FontSize {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text's color.
    /// </summary>
    // ------------------------------------------------------------------
    Color FontColor {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is underlined.
    /// </summary>
    // ------------------------------------------------------------------
    bool IsUnderlined {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is bold.
    /// </summary>
    // ------------------------------------------------------------------
    bool IsBold {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the text is italic.
    /// </summary>
    // ------------------------------------------------------------------
    bool IsItalic {
      get;
      set;
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the brush that's used to draw our text to a GDI+ graphics
    /// surface.
    /// </summary>
    /// <returns>Brush</returns>
    // ------------------------------------------------------------------
    Brush GetBrush();

    // ------------------------------------------------------------------
    /// <summary>
    /// Applies the formatting specified by TextFormatting to the text
    /// specified.
    /// </summary>
    /// <param name="text">string: The un-formatted text.</param>
    /// <returns>string: The formatted text.</returns>
    // ------------------------------------------------------------------
    string GetFormattedText(string text);

    #endregion
  }
}
