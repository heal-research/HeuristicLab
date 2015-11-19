using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// A tool that copies the selected entities to the clipboard.
  /// </summary>
  // ----------------------------------------------------------------------
  public class CopyTool : AbstractTool {
    #region Fields
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:CopyTool"/> class.
    /// </summary>
    /// <param name="name">string: The name of the tool.</param>
    // ------------------------------------------------------------------
    public CopyTool(string name)
      : base(name) {
    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
      if (this.Controller.Model.Selection.SelectedItems.Count == 0)
        return;

      try {
        //Clear the Anchors otherwise subsequent Copy operations will 
        // raise an exception due to the fact that the Anchors class 
        // is a static helper class.
        Anchors.Clear();

        // First add the image to the Windows Clipboard so it can be
        // pasted into other applications, like PowerPoint.
        Bitmap image = this.Controller.Model.Selection.ToBitmap();
        Clipboard.SetDataObject(image, true);

        // This will create a volatile collection of entities, but they 
        // need to be unwrapped!  I never managed to get things 
        // working by putting the serialized collection directly onto 
        // the Clipboad. Thanks to Leppie's suggestion it works by
        // putting the Stream onto the Clipboard, which is weird but 
        // it works.
        //MemoryStream copy = Selection.SelectedItems.ToStream();
        //DataFormats.Format format =
        //    DataFormats.GetFormat(typeof(CopyTool).FullName);

        //IDataObject dataObject = new DataObject();
        //dataObject.SetData(format.Name, false, copy);
        //Clipboard.SetDataObject(dataObject, false);

        // Rather than placing the stream of entities on the Windows
        // Clipboard, we're using our custom clipboard to keep the
        // Windows clipboard for moving data across apps.
        NetronClipboard.Clear();
        NetronClipboard.Add(this.Controller.Model.Selection.SelectedItems.Copy());

      }
      catch (Exception exc) {
        //throw new InconsistencyException(
        //    "The Copy operation failed.", exc);
        MessageBox.Show("Unable to copy the selection.\n\n" +
            "Error Message: " + exc.Message);
      }
      finally {
        DeactivateTool();
      }
    }

    #endregion
  }

}
