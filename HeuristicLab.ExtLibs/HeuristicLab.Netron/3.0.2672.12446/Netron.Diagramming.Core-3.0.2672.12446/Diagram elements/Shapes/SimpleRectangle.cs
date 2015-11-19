using System.Drawing;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// A simple rectangular shape.
  /// </summary>
  // ----------------------------------------------------------------------
  [Shape(
      "Rectangle",
      "SimpleRectangle",
      "Standard",
      "A simple rectangular shape.")]
  public partial class SimpleRectangle : SimpleShapeBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// SimpleRectangle.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double simpleRectangleVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// The bottom connector.
    /// </summary>
    // ------------------------------------------------------------------
    //Connector cBottom;

    // ------------------------------------------------------------------
    /// <summary>
    /// The left connector.
    /// </summary>
    // ------------------------------------------------------------------
    //Connector cLeft;

    // ------------------------------------------------------------------
    /// <summary>
    /// The right connector.
    /// </summary>
    // ------------------------------------------------------------------
    //Connector cRight;

    // ------------------------------------------------------------------
    /// <summary>
    /// The top connector.
    /// </summary>
    // ------------------------------------------------------------------
    //Connector cTop;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return simpleRectangleVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI
    /// </summary>
    /// <value>string</value>
    // ------------------------------------------------------------------
    public override string EntityName {
      get { return "Simple Rectangle"; }
    }

    public override Rectangle Rectangle {
      get {
        return base.Rectangle;
      }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="s"></param>
    // ------------------------------------------------------------------
    public SimpleRectangle(IModel s)
      : base(s) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:SimpleRectangle"/> class.
    /// </summary>
    // ------------------------------------------------------------------
    public SimpleRectangle()
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

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the bundle on the canvas
    /// </summary>
    /// <param name="g">Graphics: The GDI+ graphics surface.</param>
    // ------------------------------------------------------------------
    public override void Paint(Graphics g) {
      // Draw the shadow.
      if (ArtPalette.EnableShadows) {
        g.FillRectangle(
            ArtPalette.ShadowBrush,
            mRectangle.X + 5,
            mRectangle.Y + 5,
            mRectangle.Width,
            mRectangle.Height);
      }

      // Fill the shape.
      g.FillRectangle(Brush, mRectangle);

      // Draw the edge of the bundle.
      if (Hovered) {
        g.DrawRectangle(ArtPalette.HighlightPen, mRectangle);
      } else {
        g.DrawRectangle(Pen, mRectangle);
      }

      // Finally, the connectors.
      if (this.ShowConnectors) {
        for (int k = 0; k < Connectors.Count; k++) {
          Connectors[k].Paint(g);
        }
      }

      //here we keep it really simple:
      if (!string.IsNullOrEmpty(Text)) {
        //StringFormat format = new StringFormat();
        //format.Alignment = this.VerticalAlignment;
        //format.LineAlignment = this.HorizontalAlignment;
        //SolidBrush textBrush = new SolidBrush(mForeColor);

        //g.DrawString(
        //    Text, 
        //    mFont,
        //    textBrush, 
        //    TextArea,
        //    format);
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
