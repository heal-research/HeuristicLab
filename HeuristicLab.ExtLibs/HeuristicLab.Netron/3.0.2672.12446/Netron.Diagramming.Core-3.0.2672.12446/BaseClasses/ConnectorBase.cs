using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract base class for a connector.
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract partial class ConnectorBase :
        DiagramEntityBase,
        IConnector {
    #region Enums
    public enum ConnectorNameLocation {
      Bottom,
      Left,
      Right,
      Top
    }
    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// ConnectorBase.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double connectorBaseVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the location of this connector
    /// </summary>
    // ------------------------------------------------------------------
    protected Point mPoint;

    // ------------------------------------------------------------------
    /// <summary>
    /// the connectors attached to this connector
    /// </summary>
    // ------------------------------------------------------------------
    protected CollectionBase<IConnector> mAttachedConnectors;

    // ------------------------------------------------------------------
    /// <summary>
    /// the connector, if any, to which this connector is attached to
    /// </summary>
    // ------------------------------------------------------------------
    protected IConnector mAttachedTo;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the connectors name is drawn.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mShowName = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies where the name is drawn relative to the connector's
    /// bounds.
    /// </summary>
    // ------------------------------------------------------------------
    protected ConnectorNameLocation mNameLocation =
        ConnectorNameLocation.Top;

    // ------------------------------------------------------------------
    /// <summary>
    /// The font used to draw the name with.
    /// </summary>
    // ------------------------------------------------------------------
    protected Font mFont = new Font("Arial", 12, FontStyle.Bold);

    // ------------------------------------------------------------------
    /// <summary>
    /// The color used to draw the name with.
    /// </summary>
    // ------------------------------------------------------------------
    protected Color mForeColor = Color.Black;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return connectorBaseVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the font used to draw the name with.
    /// </summary>
    // ------------------------------------------------------------------
    public Font Font {
      get {
        return mFont;
      }
      set {
        mFont = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the color used to draw the name with.
    /// </summary>
    // ------------------------------------------------------------------
    public Color ForeColor {
      get {
        return mForeColor;
      }
      set {
        mForeColor = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets where the name is drawn relative to the connector's
    /// bounds.
    /// </summary>
    // ------------------------------------------------------------------
    public ConnectorNameLocation NameLocation {
      get {
        return mNameLocation;
      }
      set {
        mNameLocation = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the connectors name is drawn.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowName {
      get {
        return mShowName;
      }
      set {
        mShowName = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the attached connectors to this connector
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IConnector> AttachedConnectors {
      get {
        return mAttachedConnectors;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// If the connector is attached to another connector
    /// </summary>
    // ------------------------------------------------------------------
    public IConnector AttachedTo {
      get {
        return mAttachedTo;
      }
      set {
        mAttachedTo = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The location of this connector
    /// </summary>
    // ------------------------------------------------------------------
    public Point Point {
      get {
        return mPoint;
      }
      set {
        //throw new NotSupportedException("Use the Move() method instead.");

        mPoint = value;
        //foreach(IConnector con in  attachedConnectors)
        //{
        //    con.Point = value;
        //}
      }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Default connector.
    /// </summary>
    // ------------------------------------------------------------------
    protected ConnectorBase(IModel model)
      : base(model) {
      mAttachedConnectors = new CollectionBase<IConnector>();
      this.InitializeConnector();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructs a connector, passing its location
    /// </summary>
    /// <param name="p">Point: The location of the conector.</param>
    /// <param name="model">IModel: The model.</param>
    // ------------------------------------------------------------------
    protected ConnectorBase(Point p, IModel model)
      : base(model) {
      mAttachedConnectors = new CollectionBase<IConnector>();
      mPoint = p;
      this.InitializeConnector();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ConnectorBase"/> 
    /// class.
    /// </summary>
    /// <param name="p">Point: The location of the conector.</param>
    // ------------------------------------------------------------------
    protected ConnectorBase(Point p)
      : base() {
      mAttachedConnectors = new CollectionBase<IConnector>();
      this.mPoint = p;
      this.InitializeConnector();
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes the default settings of the connector.  Actions
    /// performed in the ConnectorBase are:
    ///     *  Sets the default PaintStyle to be 
    ///        'SolidPaintStyle' with a transparent color.
    ///     *  Sets the default PenStyle to be a LinePenStyle whose color
    ///        is Transparent and DashStyle is 'Solid'.
    /// </summary>
    // ------------------------------------------------------------------
    protected virtual void InitializeConnector() {
      this.mPaintStyle = new SolidPaintStyle(Color.Transparent);
      LinePenStyle lineStyle = new LinePenStyle();
      lineStyle.Color = Color.Transparent;
      lineStyle.DashStyle = DashStyle.Solid;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// The custom menu to be added to the base menu of this connector.
    /// 'null' is returned here.
    /// </summary>
    /// <returns>ToolStripItem[]</returns>
    // ------------------------------------------------------------------
    public override ToolStripItem[] Menu() {
      return null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches the given connector to this connector. 
    /// <remarks>The method will remove a previous binding, if any, 
    /// before creating the attachment. This method prohibits mutliple 
    /// identical connections; you can attach a connector only once.
    /// </remarks>
    /// </summary>
    /// <param name="connector">IConnector</param>
    // ------------------------------------------------------------------
    public virtual void AttachConnector(IConnector connector) {
      if (connector == null || !Enabled)
        return;
      //only attach'm if not already present and not the parent
      if ((!mAttachedConnectors.Contains(connector)) &&
          (connector != mAttachedTo)) {
        connector.DetachFromParent();
        mAttachedConnectors.Add(connector);
        // Make sure the attached connector is centered at this 
        // connector.
        connector.Point = this.mPoint;
        connector.AttachedTo = this;
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Detaches the given connector from this connector. No exception 
    /// will be thrown if the given connector is not a child.
    /// </summary>
    /// <param name="connector">IConnector</param>
    // ------------------------------------------------------------------
    public virtual void DetachConnector(IConnector connector) {
      if (connector == null || !Enabled)
        return;

      if (mAttachedConnectors.Contains(connector)) {
        mAttachedConnectors.Remove(connector);
        connector.AttachedTo = null;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Detaches from its parent (if any). No exception will be thrown 
    /// if this connector is not attached to any parent.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void DetachFromParent() {
      if (this.AttachedTo != null && Enabled) {
        this.AttachedTo.AttachedConnectors.Remove(this);
        this.AttachedTo = null;
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches to another connector. 
    /// <remarks>If this connector is already attached it will be 
    /// detached first and no exception will be thrown.  This method does 
    /// not allow mutliple connections; you cannot attach to a parent if 
    /// it already does
    /// </remarks>
    /// </summary>
    /// <param name="parent">The new parent connector.</param>
    // ------------------------------------------------------------------
    public virtual void AttachTo(IConnector parent) {

      if (parent == null || !Enabled)
        return;
      // Do not re-attach and the parent cannot be an already attached 
      // child.
      if ((this.AttachedTo != parent) &&
          (!AttachedConnectors.Contains(parent))) {
        //remove any other binding
        DetachFromParent();
        //attach to the given parent
        parent.AttachedConnectors.Add(this);
        this.AttachedTo = parent;
      }
    }

    #endregion

  }
}
