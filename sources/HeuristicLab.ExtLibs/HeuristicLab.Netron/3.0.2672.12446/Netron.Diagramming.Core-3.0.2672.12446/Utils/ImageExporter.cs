using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Netron.Diagramming.Core {
  public class ImageExporter {
    public static Bitmap FromBundle(IBundle bundle, Graphics g) {
      // The entities will be drawn to our graphics surface using
      // their locations on the canvas.  We want them to be drawn
      // relative to the 0,0 (upper, left corner) of our image.
      // Therefore, calculate the offset required to position them
      // (normalize) in our image and apply that translation to
      // g2, the GDI+ drawing surface used to paint the entities to
      // the image.  Notice we're actually calculating the offset
      // relative to location 5,5.  We adjusting the image height
      // and widht by 10 to ensure we catch everything, so by using
      // location 5,5 here we kept things centered.
      Point offset = new Point(
          5 - bundle.Rectangle.X,
          5 - bundle.Rectangle.Y);
      Matrix matrix = new Matrix();
      matrix.Translate(offset.X, offset.Y);

      Rectangle bundleArea = bundle.Rectangle;
      // bundleArea.Inflate(5, 5);

      Bitmap image = new Bitmap(
          bundleArea.Width + 10,
          bundleArea.Height + 10,
          g);
      Graphics g2 = Graphics.FromImage(image);
      g2.Transform = matrix;


      // The background color is a weird blue, so I'm filling the
      // entire area first with a white color to get around this.
      g2.Clear(Color.White);
      g2.SmoothingMode = SmoothingMode.HighQuality;
      g2.CompositingQuality = CompositingQuality.HighQuality;
      g2.InterpolationMode = InterpolationMode.HighQualityBicubic;

      // Deselect and set Hovered to 'false' for all entities so 
      // they're drawn to the image in their *normal* state.
      bundle.DeSelectAll();
      bundle.SetHovered(false);
      bundle.Paint(g2);

      g.Flush();
      g2.Flush();
      g.Dispose();
      g2.Dispose();

      //System.IntPtr dc1 = g.GetHdc();

      //System.IntPtr dc2 = g2.GetHdc();

      //BitBlt(dc2, 0, 0, width, height, dc1, 0, 0, 0x00CC0020);

      //g.ReleaseHdc(dc1);

      //g2.ReleaseHdc(dc2);

      //g.Dispose();

      //g2.Dispose();
      return image;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// GDI32 imported function not available in the framework,
    /// used here to create an image.
    /// </summary>
    /// <param name="hdcDest"></param>
    /// <param name="nXDest"></param>
    /// <param name="nYDest"></param>
    /// <param name="nWidth"></param>
    /// <param name="nHeight"></param>
    /// <param name="hdcSrc"></param>
    /// <param name="nXSrc"></param>
    /// <param name="nYSrc"></param>
    /// <param name="dwRop"></param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(

        IntPtr hdcDest, // handle to destination DC

        int nXDest, // x-coord of destination upper-left corner

        int nYDest, // y-coord of destination upper-left corner

        int nWidth, // width of destination rectangle

        int nHeight, // height of destination rectangle

        IntPtr hdcSrc, // handle to source DC

        int nXSrc, // x-coordinate of source upper-left corner

        int nYSrc, // y-coordinate of source upper-left corner

        System.Int32 dwRop // raster operation code

        );
  }
}
