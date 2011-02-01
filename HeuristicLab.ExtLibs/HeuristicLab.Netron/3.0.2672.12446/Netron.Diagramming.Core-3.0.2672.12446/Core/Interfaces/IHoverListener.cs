using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Hover service
  /// </summary>
  public interface IHoverListener : IInteraction {
    void MouseHover(MouseEventArgs e);
    void MouseEnter(MouseEventArgs e);
    void MouseLeave(MouseEventArgs e);

  }
}
