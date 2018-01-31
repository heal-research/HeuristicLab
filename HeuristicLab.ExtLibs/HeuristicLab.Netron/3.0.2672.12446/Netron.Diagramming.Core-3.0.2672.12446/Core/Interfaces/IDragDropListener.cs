using System;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  public interface IDragDropListener : IInteraction {

    /// <summary>
    /// On dragdrop.
    /// </summary>
    /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
    bool OnDragDrop(DragEventArgs e);

    /// <summary>
    ///   On drag enter
    /// </summary>
    /// <param name="e"></param>
    bool OnDragEnter(DragEventArgs e);
    /// <summary>
    /// On drag leave.
    /// </summary>
    /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
    bool OnDragLeave(EventArgs e);

    /// <summary>
    /// On drag over.
    /// </summary>
    /// <param name="e">The <see cref="T:KeyPressEventArgs"/> instance containing the event data.</param>
    bool OnDragOver(DragEventArgs e);

    /// <summary>
    /// Gives the feedback on dragging.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Forms.GiveFeedbackEventArgs"/> instance containing the event data.</param>
    void GiveFeedback(GiveFeedbackEventArgs e);

  }
}
