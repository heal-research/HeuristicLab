using System;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// A tool that uses the ImageExporter to create an image from the
  /// currently selected entities and adds the image to the clipboard.
  /// The ImageExporter is used indirectly by calling 
  /// 'Selection.ToBitmap()'.
  /// </summary>
  // ----------------------------------------------------------------------
  public class ImageExportTool : AbstractTool {
    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ImageExportTool"/> 
    /// class.
    /// </summary>
    /// <param name="name">string: The name of the tool.</param>
    // ------------------------------------------------------------------
    public ImageExportTool(string toolName)
      : base(toolName) {
    }

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      if (this.Controller.Model.Selection.SelectedItems.Count == 0) {
        MessageBox.Show("No shapes are selected.  " +
            "Please make a selection first.",
            "Image Export Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        return;
      }

      try {
        // Create an image using the ImageExporter and copy it to
        // the clipboard.
        Bitmap image = this.Controller.Model.Selection.ToBitmap();
        if (image != null) {
          Clipboard.SetImage(image);
        }
      }
      catch (Exception exc) {
        throw new InconsistencyException(
            "The Copy operation failed.", exc);
      }
      finally {
        DeactivateTool();
      }
    }

    #endregion
  }
}
