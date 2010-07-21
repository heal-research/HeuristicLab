using System;
using System.Drawing;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// A simple bundle to display textual information
  /// </summary>
  public class TextLabel : SimpleShapeBase {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// TextLabel.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double textLabelVersion = 1.0;

    private Connector connector;
    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return textLabelVersion;
      }
    }

    /// <summary>
    /// Gets the connector.
    /// </summary>
    /// <value>The connector.</value>
    public Connector Connector {
      get { return connector; }
    }

    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI
    /// </summary>
    /// <value></value>
    public override string EntityName {
      get { return "Text Label"; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="s"></param>
    public TextLabel(IModel s)
      : base(s) {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:TextLabel"/> class.
    /// </summary>
    public TextLabel()
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

      PaintStyle = ArtPalette.GetDefaultSolidPaintStyle();
      Resizable = false;
      connector = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
      connector.Name = "Central connector";
      connector.Parent = this;
      Connectors.Add(connector);
    }

    /// <summary>
    /// Paints the bundle on the canvas
    /// </summary>
    /// <param name="g">a Graphics object onto which to paint</param>
    public override void Paint(System.Drawing.Graphics g) {
      if (g == null)
        throw new ArgumentNullException("The Graphics object is 'null'.");

      g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      //the shadow
      if (ArtPalette.EnableShadows)
        g.FillRectangle(ArtPalette.ShadowBrush, Rectangle.X + 5, Rectangle.Y + 5, Rectangle.Width, Rectangle.Height);
      //the container
      g.FillRectangle(Brush, Rectangle);
      //the edge
      if (Hovered)
        g.DrawRectangle(new Pen(Color.Red, 2F), Rectangle);
      //the text		
      if (!string.IsNullOrEmpty(Text))
        g.DrawString(Text, ArtPalette.DefaultFont, Brushes.Black, Rectangle);
    }
    #endregion
  }
}
