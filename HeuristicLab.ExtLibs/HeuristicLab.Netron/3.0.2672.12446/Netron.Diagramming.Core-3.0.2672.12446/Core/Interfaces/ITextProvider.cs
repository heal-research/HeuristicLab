using System.Drawing;

namespace Netron.Diagramming.Core {
  /// <summary>
  /// The signature for any item that wants text editing.
  /// </summary>
  public interface ITextProvider {
    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the bounds of the text.
    /// </summary>
    // ------------------------------------------------------------------
    Rectangle TextArea {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    // ------------------------------------------------------------------
    string Text {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the style of the text to use.
    /// </summary>
    // ------------------------------------------------------------------
    ITextStyle TextStyle {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns if text editing is allowed.
    /// </summary>
    // ------------------------------------------------------------------
    bool AllowTextEditing {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the number of mouse clicks required to launch the in-place
    /// text editor.
    /// </summary>
    // ------------------------------------------------------------------
    int EditTextClicks {
      get;
      set;
    }
  }
}
