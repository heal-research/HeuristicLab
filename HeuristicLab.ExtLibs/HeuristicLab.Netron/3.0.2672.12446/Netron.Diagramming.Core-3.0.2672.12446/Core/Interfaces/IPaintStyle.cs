using System.Drawing;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Defines a custom painting style.
  /// </summary>
  public interface IPaintStyle {
    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when this paint style is changed.
    /// </summary>
    // ------------------------------------------------------------------
    event PaintStyleChangedEventHandler PaintStyleChanged;

    /// <summary>
    /// Gets the brush with which an entity can be painted.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns></returns>
    Brush GetBrush(Rectangle rectangle);

  }


}
