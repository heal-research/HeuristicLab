using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// A shape used for a shortcut to a file.
  /// </summary>
  // ----------------------------------------------------------------------
  [Serializable()]
  public class FileShape : ComplexShapeBase {
    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the FileShape's current version.
    /// </summary>
    // ------------------------------------------------------------------
    protected double fileShapeVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// The complete path to the file we're to open when this shape is
    /// double-clicked.
    /// </summary>
    // ------------------------------------------------------------------
    protected string myFileName = String.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// The IconLabelMaterial used to show the file's icon.
    /// </summary>
    // ------------------------------------------------------------------
    IconLabelMaterial myIcon = null;

    // ------------------------------------------------------------------
    /// <summary>
    /// The LabelMaterial used to show the file's name.
    /// </summary>
    // ------------------------------------------------------------------
    LabelMaterial myLabel = null;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the 'friendly' name of this shape.
    /// </summary>
    // ------------------------------------------------------------------
    public override string EntityName {
      get {
        return "File";
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the version of this file shape.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return fileShapeVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the complete path to the file we're to open when this
    /// shape is double-clicked.
    /// </summary>
    // ------------------------------------------------------------------
    public string FileName {
      get {
        return myFileName;
      }
      set {
        try {
          FileInfo info = new FileInfo(value);
          if (info.Exists) {
            myFileName = value;
            CreateIcon(myFileName);
            CreateLabel(info.Name);
          }
        }
        catch {
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if this shape is resizable - the FileShape is not
    /// resizable.
    /// </summary>
    // ------------------------------------------------------------------
    public override bool Resizable {
      get {
        return false;
      }
      set {
        base.Resizable = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public FileShape()
      : base() {
      myIcon = new IconLabelMaterial();
      myIcon.Icon = (Bitmap)ImagePalette.Shortcut;
      myIcon.Shape = this;
      Children.Add(myIcon);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor that receives the path to the file.
    /// </summary>
    /// <param name="filename">string: The complete path to the file we're 
    /// to open when this shape is double-clicked.</param>
    // ------------------------------------------------------------------
    public FileShape(string filename)
      : base() {
      FileName = filename;
    }

    #region Overrides

    // ------------------------------------------------------------------
    /// <summary>
    /// Opens the file when this shape is double-clicked.
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    /// <returns>bool: True if handled.</returns>
    // ------------------------------------------------------------------
    public override bool MouseDown(MouseEventArgs e) {
      if ((e.Button == MouseButtons.Left) &&
          (e.Clicks == 2) &&
          (this.myFileName != String.Empty)) {
        try {
          Process.Start(this.myFileName);
          return true;
        }
        catch (Exception ex) {
          MessageBox.Show("Unable to open the file.\n" + ex.Message);
          return false;
        }
      }
      return base.MouseDown(e);
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Loads the icon from Windows for the filename specified and
    /// creates our 'icon'.
    /// </summary>
    /// <param name="filename">string</param>
    // ------------------------------------------------------------------
    void CreateIcon(string filename) {
      if (this.myIcon == null) {
        this.myIcon = new IconLabelMaterial();
        Children.Add(myIcon);
        this.myIcon.Shape = this;
      }

      try {
        this.myIcon.Icon = GetIconForFile(filename, false).ToBitmap();
      }
      catch {
        this.myIcon.Icon = (Bitmap)ImagePalette.Shortcut;
      }
      this.myIcon.Transform(new Rectangle(Location, new Size(1, 1)));
      Transform(myIcon.Rectangle);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates the label that shows the file's name.
    /// </summary>
    /// <param name="filename">string: The name to show in the 
    /// label.</param>
    // ------------------------------------------------------------------
    void CreateLabel(string filename) {
      if (this.myLabel == null) {
        this.myLabel = new LabelMaterial();
        Children.Add(this.myLabel);
      }

      this.myLabel.Text = filename;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Draws this shape to the GDI+ graphics surface specified.
    /// </summary>
    /// <param name="g">Graphics</param>
    // ------------------------------------------------------------------
    public override void Paint(Graphics g) {
      this.myIcon.Paint(g);

      int iconWidth = myIcon.Icon.Width;
      int iconHeight = myIcon.Icon.Height;

      // Make the max available label width a little bigger than the
      // icon.
      SizeF labelSize = g.MeasureString(myLabel.Text,
          myLabel.Font,
          myIcon.Icon.Width + 100);

      // Position the label just under the icon and center it.
      int xOffset = ((int)labelSize.Width - iconWidth) / 2;

      Point labelLocation = new Point(
          myIcon.Rectangle.Location.X - xOffset,
          myIcon.Rectangle.Location.Y + iconHeight + 5);

      this.myLabel.Transform(new Rectangle(
          labelLocation,
          Size.Round(labelSize)));

      this.myLabel.Paint(g);

      mRectangle.Width = myIcon.Icon.Width + 20;
      mRectangle.Location = new Point(
          myIcon.Rectangle.X - 10,
          mRectangle.Y);

      if (labelSize.Width > iconWidth) {
        mRectangle.Width = (int)labelSize.Width + 20;
        mRectangle.Location = new Point(
            myLabel.Rectangle.X - 10,
            mRectangle.Y);
      }

      mRectangle.Height =
          myIcon.Icon.Height +
          myLabel.Rectangle.Height + 20;
    }

    #region Get Icon Associated With A File

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO {
      public IntPtr hIcon;
      public IntPtr iIcon;
      public int dwAttributes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szDisplayName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szTypeName;
    };

    internal class Win32 {
      public const uint SHGFI_ICON = 0x100;
      public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
      public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

      [DllImport("shell32.dll")]
      public static extern IntPtr SHGetFileInfo(string pszPath,
          uint dwFileAttributes,
          ref SHFILEINFO psfi,
          uint cbSizeFileInfo,
          uint uFlags);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the Icon associated with the file specified.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="smallImage">bool: Get the small image or the
    /// big one?</param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    public static Icon GetIconForFile(string fileName, bool smallImage) {
      IntPtr hImgSmall;    //the handle to the system small image list
      IntPtr hImgLarge;    //the handle to the system large image list
      SHFILEINFO shinfo = new SHFILEINFO();

      // Get the small image if we're supposed to.
      if (smallImage) {
        hImgSmall = Win32.SHGetFileInfo(
            fileName,
            0,
            ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
      } else {
        // Otherwise get the large image.
        hImgLarge = Win32.SHGetFileInfo(
            fileName,
            0,
            ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
      }

      // Return the image.
      return System.Drawing.Icon.FromHandle(shinfo.hIcon);
    }

    #endregion
  }
}
