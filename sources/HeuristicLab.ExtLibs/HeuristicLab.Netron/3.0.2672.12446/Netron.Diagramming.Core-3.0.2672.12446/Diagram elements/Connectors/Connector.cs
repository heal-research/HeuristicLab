using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The default connector.  Represents an endpoint of a connection or a 
  /// location of a bundle to which a connection can be attached.
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class Connector : ConnectorBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// Connector.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double connectorVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies how the connector is drawn on the canvas when 'IsVisible'
    /// is true.  The default style is 'Simple', which is a transparent
    /// background with a blue 'x' (similar to Visio).
    /// </summary>
    // ------------------------------------------------------------------
    protected ConnectorStyle myStyle = ConnectorStyle.Simple;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return connectorVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets how the connector is drawn on the canvas when 
    /// 'IsVisible' is true.
    /// </summary>
    // ------------------------------------------------------------------
    public ConnectorStyle ConnectorStyle {
      get {
        return this.myStyle;
      }
      set {
        this.myStyle = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public override string EntityName {
      get { return "Connector"; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The bounds of the paintable entity.
    /// </summary>
    /// <value></value>
    // ------------------------------------------------------------------
    public override Rectangle Rectangle {
      get {
        return new Rectangle(Point.X - 2, Point.Y - 2, 4, 4);
      }
      //set {               Point = value.Location;               
      //TODO: think about what to do when setting the size            }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Connector"/> class.
    /// </summary>
    /// <param name="site">The site.</param>
    // ------------------------------------------------------------------
    public Connector(IModel site)
      : base(site) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Connector"/> class.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="site">The site.</param>
    // ------------------------------------------------------------------
    public Connector(Point p, IModel site)
      : base(p, site) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Connector"/> class.
    /// </summary>
    /// <param name="p">The p.</param>
    // ------------------------------------------------------------------
    public Connector(Point p)
      : base(p) {
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the connector on the canvas.
    /// </summary>
    /// <param name="g"></param>
    // ------------------------------------------------------------------
    public override void Paint(Graphics g) {
      if (g == null) {
        throw new ArgumentNullException(
            "The Graphics object is 'null'");
      }

      if (Hovered || IsSelected) {
        Rectangle area = Rectangle;
        area.Inflate(3, 3);
        g.DrawRectangle(
            ArtPalette.ConnectionHighlightPen,
            area);
        //g.FillRectangle(
        //    Brushes.Green,
        //    Point.X - 4,
        //    Point.Y - 4,
        //    8,
        //    8);
      } else {
        if (Visible) {
          switch (this.myStyle) {
            case ConnectorStyle.Simple:
              DrawSimpleConnector(g);
              break;

            case ConnectorStyle.Round:
              break;

            case ConnectorStyle.Square:
              DrawSquareConnector(g);
              break;
          }

          if (this.mShowName) {
            DrawName(g);
          }
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Draws the name of this connector.
    /// </summary>
    /// <param name="g">Graphics</param>
    // ------------------------------------------------------------------
    void DrawName(Graphics g) {
      Size size = Size.Round(
          g.MeasureString(mName, mFont));

      int xOffset = (size.Width - Rectangle.Width) / 2;
      int yOffset = (size.Height - Rectangle.Height) / 2;

      System.Drawing.Point location = Rectangle.Location;

      switch (this.mNameLocation) {
        case ConnectorNameLocation.Top:
          location = new Point(
              Rectangle.X - xOffset,
              Rectangle.Y - size.Height);
          break;

        case ConnectorNameLocation.Bottom:
          location = new Point(
              Rectangle.X - xOffset,
              Rectangle.Bottom + size.Height);
          break;

        case ConnectorNameLocation.Left:
          location = new Point(
              Rectangle.X - size.Width,
              Rectangle.Y - yOffset);
          break;

        case ConnectorNameLocation.Right:
          location = new Point(
              Rectangle.Right,
              Rectangle.Y - yOffset);
          break;
      }

      Rectangle textArea = new Rectangle(location, size);
      StringFormat format = new StringFormat();
      format.FormatFlags = StringFormatFlags.FitBlackBox;
      g.DrawString(
          mName,
          mFont,
          new SolidBrush(mForeColor),
          location);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Draws a blue 'x' using 'Dot' as the line style, with a transparent
    /// color.
    /// </summary>
    /// <param name="g">Graphics</param>
    // ------------------------------------------------------------------
    protected virtual void DrawSimpleConnector(Graphics g) {
      Pen pen = ArtPalette.GetSimpleConnectorPenStyle().DrawingPen();
      Brush brush =
          ArtPalette.GetSimpleConnectorPaintStyle().GetBrush(
          this.Rectangle);

      GraphicsPath path = new GraphicsPath();
      // Diagonal line from top left to bottom right.
      g.DrawLine(pen, this.TopLeftCorner, this.BottomRightCorner);


      // Diagonal line from top right to bottom lrft.
      g.DrawLine(pen, this.TopRightCorner, this.BottomLeftCorner);
    }

    protected virtual void DrawSquareConnector(Graphics g) {
      Pen pen = ArtPalette.GetSimpleConnectorPenStyle().DrawingPen();
      Brush brush = ArtPalette.GetSimpleConnectorPaintStyle().GetBrush(this.Rectangle);
      g.DrawRectangle(pen, this.Rectangle);
    }


    // ------------------------------------------------------------------
    /// <summary>
    /// Tests if the mouse hits this connector.
    /// </summary>
    /// <param name="p">Point</param>
    /// <returns>bool</returns>
    // ------------------------------------------------------------------
    public override bool Hit(Point p) {
      Point a = p;
      Point b = Point;
      b.Offset(-7, -7);
      //a.Offset(-1,-1);
      Rectangle r = new Rectangle(a, new Size(0, 0));
      Rectangle d = new Rectangle(b, new Size(15, 15));
      return d.Contains(r);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the connector
    /// </summary>
    // ------------------------------------------------------------------
    public override void Invalidate() {
      Point p = Point;
      p.Offset(-5, -5);
      if (Model != null)
        Model.RaiseOnInvalidateRectangle(
                  new Rectangle(p, new Size(10, 10)));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the connector with the given shift-vector.
    /// </summary>
    /// <param name="p">Point</param>
    // ------------------------------------------------------------------
    public override void MoveBy(Point p) {
      Point pt = new Point(this.Point.X + p.X, this.Point.Y + p.Y);
      IConnection con = null;
      Point p1 = Point.Empty, p2 = Point.Empty;

      Rectangle rec = new Rectangle(
          Point.X - 10,
          Point.Y - 10,
          20,
          20);

      this.Point = pt;

      #region Case of connection
      if (typeof(IConnection).IsInstanceOfType(this.Parent)) {
        (Parent as IConnection).Invalidate();
      }
      #endregion

      #region Case of attached connectors
      for (int k = 0; k < AttachedConnectors.Count; k++) {
        if (typeof(IConnection).IsInstanceOfType(AttachedConnectors[k].Parent)) {
          //keep a reference to the two points so we can invalidate the region afterwards
          con = AttachedConnectors[k].Parent as IConnection;
          p1 = con.From.Point;
          p2 = con.To.Point;
        }
        AttachedConnectors[k].MoveBy(p);
        if (con != null) {
          //invalidate the 'before the move'-region                     
          Rectangle f = new Rectangle(p1, new Size(10, 10));
          Rectangle t = new Rectangle(p2, new Size(10, 10));
          Model.RaiseOnInvalidateRectangle(Rectangle.Union(f, t));
          //finally, invalidate the region where the connection is now
          (AttachedConnectors[k].Parent as IConnection).Invalidate();
        }
      }
      #endregion
      //invalidate this connector, since it's been moved
      Invalidate(rec);//before the move
      this.Invalidate();//after the move

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the connector with the given shift-vector
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    // ------------------------------------------------------------------
    public void MoveBy(int x, int y) {
      Point pt = new Point(x, y);
      MoveBy(pt);
    }

    #endregion
  }
}
