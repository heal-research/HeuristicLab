using System.Drawing;
using System.IO;
using System.Reflection;

namespace Netron.Diagramming.Core {
  public class ImagePalette {
    public const string PropertiesFileName =
        "PropertiesHS.png";

    #region Fields

    const string mNameSpace = "Netron.Diagramming.Core";

    static Image mAlignObjectsBottom =
        GetImage("AlignObjectsBottomHS.png");

    static Image mAlignObjectsCenteredHorizontal =
        GetImage("AlignObjectsCenteredHorizontalHS.png");

    static Image mAlignObjectsCenteredVertical =
        GetImage("AlignObjectsCenteredVerticalHS.png");

    static Image mAlignObjectsLeft = GetImage("AlignObjectsLeftHS.png");

    static Image mAlignObjectsRight = GetImage("AlignObjectsRightHS.png");

    static Image mAlignObjectsTop = GetImage("AlignObjectsTopHS.png");

    static Image mArrow = GetImage("StandardArrow.png");

    static Image mBringForward = GetImage("BringForwardHS.png");

    static Image mBringToFront = GetImage("BringToFrontHS.png");

    static Image mBucketFill = GetImage("BucketFill.bmp", Color.Magenta);

    static Image mClassShape = GetImage("ClassShape.png");

    static Image mCenterAlignment = GetImage(
        "CenterAlignment.bmp",
        Color.Silver);

    static Image mConnection = GetImage("Connection.png");

    static Image mDrawEllipse = GetImage("DrawEllipse.png");

    static Image mDrawRectangle = GetImage("DrawRectangle.png");

    static Image mFont = GetImage("FontDialogHS.png");

    static Image mGroup = GetImage("Group.png");

    static Image mLeftAlignment = GetImage(
        "LeftAlignment.bmp",
        Color.Silver);

    static Image mMoveConnector = GetImage("ConnectorMover.png");

    static Image mNewDocumnet = GetImage("NewDocumentHS.png");

    static Image mOutline = GetImage(
        "PenDraw.bmp",
        Color.Magenta);

    static Image mProperties = GetImage("PropertiesHS.png");

    static Image mRedo = GetImage("Edit_RedoHS.png");

    static Image mRightAlignment = GetImage(
        "RightAlignment.bmp",
        Color.Silver);

    static Image mSendBackwards = GetImage("SendBackwardHS.png");

    static Image mSendToBack = GetImage("SendToBackHS.png");

    static Image mShortcut = GetImage("Shortcut.bmp");

    static Image mUndo = GetImage("Edit_UndoHS.png");

    static Image mUngroup = GetImage("Ungroup.png");

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents aligning the bottom edges of
    /// objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsBottom {
      get {
        return mAlignObjectsBottom;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents horizontally aligning the center
    /// of objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsCenteredHorizontal {
      get {
        return mAlignObjectsCenteredHorizontal;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents vertically aligning the center
    /// of objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsCenteredVertical {
      get {
        return mAlignObjectsCenteredVertical;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents aligning the left edge of objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsLeft {
      get {
        return mAlignObjectsLeft;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents aligning the right edge of objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsRight {
      get {
        return mAlignObjectsRight;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents aligning the top edge of objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image AlignObjectsTop {
      get {
        return mAlignObjectsTop;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a arrow (standard cursor arrow).
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Arrow {
      get {
        return mArrow;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents bringing an object all the way to
    /// to the front in the z-order.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image BringToFront {
      get {
        return mBringToFront;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents bring an object forward in the 
    /// z-order.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image BringForward {
      get {
        return mBringForward;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets an image of a bucket of color - used to illustrate fill 
    /// color.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image BucketFill {
      get {
        return mBucketFill;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents text being centered.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image CenterAlignment {
      get {
        return mCenterAlignment;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the class shape image.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image ClassShape {
      get {
        return mClassShape;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a connection line.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Connection {
      get {
        return mConnection;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents an ellipse.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image DrawEllipse {
      get {
        return mDrawEllipse;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a rectangle.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image DrawRectangle {
      get {
        return mDrawRectangle;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents font.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Font {
      get {
        return mFont;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents grouping objects together..
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Group {
      get {
        return mGroup;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents text being aligned to the left.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image LeftAlignment {
      get {
        return mLeftAlignment;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a connection line.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image MoveConnector {
      get {
        return mMoveConnector;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents creating a new diagram.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image NewDocument {
      get {
        return mNewDocumnet;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a pen drawing a line with a
    /// color - used to illustrate line color.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Outline {
      get {
        return mOutline;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents properties.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Properties {
      get {
        return mProperties;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents redoing a command.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Redo {
      get {
        return mRedo;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents text being aligned to the right.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image RightAlignment {
      get {
        return mRightAlignment;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents sending an object back in the 
    /// z-order.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image SendBackwards {
      get {
        return mSendBackwards;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents sending an object all the way back
    /// in the z-order.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image SendToBack {
      get {
        return mSendToBack;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents a shortcut to a file or resource.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Shortcut {
      get {
        return mShortcut;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents undoing a command.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Undo {
      get {
        return mUndo;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image that represents ungrouping objects.
    /// </summary>
    // ------------------------------------------------------------------
    public static Image Ungroup {
      get {
        return mUngroup;
      }
    }

    #endregion

    #region Helpers

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image from the embedded resources for the specified 
    /// filename and sets the image's transparent color to the one
    /// specified..
    /// </summary>
    /// <param name="filename">string: The filename from the embedded
    /// resources.</param>
    /// <param name="transparentColor">Color: The transparent color
    /// for the image.</param>
    /// <returns>Image</returns>
    // ------------------------------------------------------------------
    static Image GetImage(string filename, Color transparentColor) {
      Bitmap bmp = GetImage(filename);
      bmp.MakeTransparent(transparentColor);
      return bmp;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image from the embedded resources for the specified 
    /// filename.
    /// </summary>
    /// <param name="filename">string: The filename from the embedded
    /// resources.</param>
    /// <returns>Image</returns>
    // ------------------------------------------------------------------
    static Bitmap GetImage(string filename) {
      return new Bitmap(GetStream(filename));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns a Stream from the manifest resources for the specified 
    /// filename.
    /// </summary>
    /// <param name="filename">string</param>
    /// <returns>Stream</returns>
    // ------------------------------------------------------------------
    public static Stream GetStream(string filename) {
      return Assembly.GetExecutingAssembly().GetManifestResourceStream(
          mNameSpace +
          ".Resources." +
          filename);
    }

    #endregion
  }
}
