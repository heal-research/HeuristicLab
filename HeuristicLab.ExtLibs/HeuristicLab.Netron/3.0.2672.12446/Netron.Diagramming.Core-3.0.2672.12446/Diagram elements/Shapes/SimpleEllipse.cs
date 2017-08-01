using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// A simple elliptic shape
  /// </summary>
  [Shape("Ellipse", "SimpleEllipse", "Standard", "A simple elliptic shape.")]
  partial class SimpleEllipse : SimpleShapeBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// SimpleEllipse.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double simpleEllipseVersion = 1.0;

    /// <summary>
    /// the connectors
    /// </summary>
    //Connector cBottom, cLeft, cRight, cTop;
    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return simpleEllipseVersion;
      }
    }

    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI
    /// </summary>
    /// <value></value>
    public override string EntityName {
      get { return "Simple Ellipse"; }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="s"></param>
    public SimpleEllipse(IModel s)
      : base(s) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:SimpleEllipse"/> class.
    /// </summary>
    public SimpleEllipse()
      : base() {
    }

    #endregion

    #region Methods

    // -----------------------------------------------------------------
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    // -----------------------------------------------------------------
    protected override void Initialize() {
      base.Initialize();

      //the initial size
      Transform(0, 0, 200, 50);
      this.mTextArea = Rectangle;

      // I'm changing the "simple" shapes to have no connectors.
      #region Connectors
      //cTop = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Top), Model);
      //cTop.Name = "Top connector";
      //cTop.Parent = this;
      //Connectors.Add(cTop);

      //cRight = new Connector(new Point(Rectangle.Right, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
      //cRight.Name = "Right connector";
      //cRight.Parent = this;
      //Connectors.Add(cRight);

      //cBottom = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
      //cBottom.Name = "Bottom connector";
      //cBottom.Parent = this;
      //Connectors.Add(cBottom);

      //cLeft = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
      //cLeft.Name = "Left connector";
      //cLeft.Parent = this;
      //Connectors.Add(cLeft);
      #endregion
    }

    /// <summary>
    /// Tests whether the mouse hits this shape.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public override bool Hit(Point p) {
      GraphicsPath path = new GraphicsPath();
      path.AddEllipse(Rectangle);
      return path.IsVisible(p);
    }

    /// <summary>
    /// Paints the bundle on the canvas
    /// </summary>
    /// <param name="g"></param>
    public override void Paint(Graphics g) {
      g.SmoothingMode = SmoothingMode.HighQuality;
      //the shadow
      if (ArtPalette.EnableShadows) {
        g.FillEllipse(
            ArtPalette.ShadowBrush,
            Rectangle.X + 5,
            Rectangle.Y + 5,
            Rectangle.Width,
            Rectangle.Height);
      }

      //the actual bundle
      g.FillEllipse(mPaintStyle.GetBrush(Rectangle), Rectangle);

      //the edge of the bundle
      if (Hovered) {
        g.DrawEllipse(ArtPalette.HighlightPen, Rectangle);
      } else {
        g.DrawEllipse(mPenStyle.DrawingPen(), Rectangle);
      }
      //the connectors
      if (this.ShowConnectors) {
        for (int k = 0; k < Connectors.Count; k++) {
          Connectors[k].Paint(g);
        }
      }

      //here we keep it really simple:
      if (!string.IsNullOrEmpty(Text)) {
        //g.DrawString(
        //    Text, 
        //    ArtPalette.DefaultFont, 
        //    Brushes.Black, 
        //    TextArea);
        g.DrawString(
            mTextStyle.GetFormattedText(Text),
            mTextStyle.Font,
            mTextStyle.GetBrush(),
            TextArea,
            mTextStyle.StringFormat);
      }
    }





    #endregion
  }
}
