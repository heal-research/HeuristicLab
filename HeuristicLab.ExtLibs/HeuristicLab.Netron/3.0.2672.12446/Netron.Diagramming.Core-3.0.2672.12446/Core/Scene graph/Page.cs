using System;
using System.Drawing;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Implementation of the <see cref="IPage"/> interface. The page represents one page the diagram control, pages being usually visualized as tabs in the control.
  /// 
  /// </summary>
  public partial class Page : IPage, IVersion {
    #region Events
    /// <summary>
    /// Occurs when the Ambience has changed
    /// </summary>
    public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
    /// <summary>
    /// Occurs when an entity is added.
    /// <remarks>This event usually is bubbled from one of the layers</remarks>
    /// </summary>
    public event EventHandler<EntityEventArgs> OnEntityAdded;
    /// <summary>
    /// Occurs when an entity is removed.
    /// <remarks>This event usually is bubbled from one of the layers</remarks>
    /// </summary>
    public event EventHandler<EntityEventArgs> OnEntityRemoved;
    /// <summary>
    /// Occurs when the page is cleared
    /// </summary>
    public event EventHandler OnClear;
    /// <summary>
    /// Occurs before an entity is transformed
    /// </summary>
    //public event EventHandler<CancelableEntityEventArgs> OnBeforeResize;
    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// Page.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double pageVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// The scaling factor of this page.
    /// </summary>
    // ------------------------------------------------------------------
    protected SizeF mMagnification = new SizeF(71F, 71F);

    // ------------------------------------------------------------------
    /// <summary>
    /// Pans the page with the given shift.
    /// </summary>
    // ------------------------------------------------------------------
    protected Point mOrigin = new Point(0, 0);

    /// <summary>
    /// the Name field
    /// </summary>
    protected string mName;

    /// <summary>
    /// the default layer
    /// </summary>
    [NonSerialized]
    protected ILayer mDefaultLayer;

    /// <summary>
    /// the Layers field
    /// </summary>
    protected CollectionBase<ILayer> mLayers;

    /// <summary>
    /// the Ambience field
    /// </summary>
    protected Ambience mAmbience;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return pageVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the scaling factor of this page.
    /// </summary>
    // ------------------------------------------------------------------
    public SizeF Magnification {
      get {
        return mMagnification;
      }
      set {
        if (value == SizeF.Empty) {
          throw new InconsistencyException(
              "The magnification has to be a positive value " +
              "greater than zero.");
        }

        mMagnification = value;

        // Update the magnification for each entity.
        foreach (IDiagramEntity entity in Entities) {
          entity.Magnification = mMagnification;
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the origin of this page.
    /// </summary>
    // ------------------------------------------------------------------
    public Point Origin {
      get {
        return mOrigin;
      }
      set {
        mOrigin = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the page bounds, taking into account landscape and page
    /// settings (for printing).
    /// </summary>
    // ------------------------------------------------------------------
    public RectangleF Bounds {
      get {
        float width = (float)Ambience.PageSize.Width / 1000;
        float height = (float)Ambience.PageSize.Height / 1000;
        width = width * Display.DpiX;
        height = height * Display.DpiY;

        if (Ambience.Landscape) {
          float w = width;
          width = height;
          height = w;
        }

        // Offset the upper-left corner by 1 inch.
        //float x = 1F * Display.DpiX;
        //float y = 1F * Display.DpiY;

        //inserted to remove margin around the drawi-ng aera
        float x = 0;
        float y = 0;

        RectangleF bounds = new RectangleF(x, y, width, height);
        return bounds;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all shapes in this page.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IShape> Shapes {
      get {
        CollectionBase<IShape> shapes = new CollectionBase<IShape>();
        foreach (ILayer layer in mLayers) {
          shapes.AddRange(layer.Shapes);
        }
        return shapes;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all entities in this page.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> Entities {
      get {
        //return DefaultLayer.Entities;
        CollectionBase<IDiagramEntity> entities =
            new CollectionBase<IDiagramEntity>();
        foreach (ILayer layer in mLayers) {
          entities.AddRange(layer.Entities);
        }
        return entities;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all connections in this page.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IConnection> Connections {
      get {
        CollectionBase<IConnection> cons = new CollectionBase<IConnection>();
        foreach (ILayer layer in mLayers) {
          cons.AddRange(layer.Connections);
        }
        return cons;
      }
    }

    /// <summary>
    /// Gets or sets the Ambience
    /// </summary>
    public Ambience Ambience {
      get {
        return mAmbience;
      }
      set {
        mAmbience = value;
        RaiseOnAmbienceChanged(new AmbienceEventArgs(mAmbience));
      }
    }

    /// <summary>
    /// Gets the default layer.
    /// </summary>
    /// <value>The default layer.</value>
    public ILayer DefaultLayer {
      get {
        return mDefaultLayer;
      }
    }
    /// <summary>
    /// Gets or sets the Layers
    /// </summary>
    public CollectionBase<ILayer> Layers {
      get {
        return mLayers;
      }
      set {
        mLayers = value;
      }
    }
    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    public string Name {
      get { return mName; }
      set { mName = value; }
    }
    /// <summary>
    /// the Model field
    /// </summary>
    private IModel mModel;
    /// <summary>
    /// Gets or sets the Model
    /// </summary>
    public IModel Model {
      get {
        return mModel;
      }
      set {
        if (value == null)
          throw new InconsistencyException("The model you want to set has value 'null'.");
        mModel = value;
        //this will cascade the setting down the hierarchy
        foreach (ILayer layer in mLayers)
          layer.Model = value;
      }
    }
    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor
    ///</summary>
    public Page(string name, IModel model) {
      this.mModel = model;

      mName = name;
      mAmbience = new Ambience(this);

      //the one and only and indestructible layer            
      mLayers = new CollectionBase<ILayer>();
      mLayers.Add(new Layer("Default Layer"));

      Init();


    }

    /// <summary>
    /// Initializes this object
    /// <remarks>See also the <see cref="OnDeserialized"/> event for post-deserialization actions to which this method is related.
    /// </remarks>
    /// </summary>
    private void Init() {
      if (mLayers == null || mLayers.Count == 0)
        throw new InconsistencyException("The page object does not contain the expected default layer.");
      mDefaultLayer = mLayers[0];

      //listen to events
      AttachToAmbience(mAmbience);

      foreach (ILayer layer in mLayers)
        AttachToLayer(layer);
    }

    private void AttachToLayer(ILayer layer) {
      layer.OnEntityAdded += new EventHandler<EntityEventArgs>(defaultLayer_OnEntityAdded);
      layer.OnEntityRemoved += new EventHandler<EntityEventArgs>(mDefaultLayer_OnEntityRemoved);
      layer.OnClear += new EventHandler(mDefaultLayer_OnClear);
    }

    /// <summary>
    /// Attaches the model to the ambience class.
    /// </summary>
    /// <param name="ambience">The ambience.</param>
    private void AttachToAmbience(Ambience ambience) {
      if (ambience == null)
        throw new ArgumentNullException("The ambience object assigned to the model cannot be 'null'");

      mAmbience.OnAmbienceChanged += new EventHandler<AmbienceEventArgs>(mAmbience_OnAmbienceChanged);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the layer that has the entity specified.  If the entity
    /// specified could not be found, 'null' is returned.
    /// </summary>
    /// <param name="entity">IDiagramEntity</param>
    /// <returns>ILayer</returns>
    // ------------------------------------------------------------------
    public ILayer GetLayer(IDiagramEntity entity) {
      foreach (ILayer layer in mLayers) {
        if (layer.Entities.Contains(entity)) {
          return layer;
        }
      }
      return null;
    }

    public void Paint(Graphics g, bool showGrid) {
      // First paint the grid.
      if (showGrid) {
        float penWidth = 1F;
        float max = 0;

        if (Magnification != Size.Empty) {
          max = Math.Max(
              Magnification.Width / 100F,
              Magnification.Height / 100F);
        }

        // Scale the pen size by the magnification so the grid
        // lines don't get magnified (stays a constant width).
        if (max > 0) {
          penWidth = penWidth / max;
        }

        // Show the grid lines in every 1/4th of the current
        // measurement units.
        float xGridSpacing = g.DpiX / 4;
        float yGridSpacing = g.DpiY / 4;

        Grid.Paint(g, Bounds, xGridSpacing, yGridSpacing, penWidth);
      }

      // The old way, before multiple layers were allowed.
      //foreach (IDiagramEntity entity in mModel.Paintables)
      //{
      //    entity.Paint(g);                
      //}

      // I was going to do it this way, but then decided it's easier
      // to check the 'Layer.IsVisible' property for each entity.
      //foreach (ILayer layer in mModel.CurrentPage.Layers)
      //{
      //    if (layer.IsVisible)
      //    {
      //        foreach (IDiagramEntity entity in layer.Entities)
      //        {
      //            entity.Paint(g);
      //        }
      //    }
      //}          

      foreach (IDiagramEntity entity in this.Entities) {
        // If this entity isn't attached to a layer, then paint
        // it regardless.
        if ((entity.Layer == null) ||
            (entity.Layer.IsVisible)) {
          entity.Paint(g);
        }
        //else if (entity.Layer.IsVisible)
        //{
        //    //entity.Paint(g);
        //}
      }

      g.Transform.Reset();
      g.ResetClip();
    }

    /// <summary>
    /// Raises the <see cref="OnAmbienceChanged"/> event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void RaiseOnAmbienceChanged(AmbienceEventArgs e) {
      EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
      if (handler != null) {
        handler(this, e);
      }
    }

    /// <summary>
    /// Handles the OnClear event of the DefaultLayer.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    void mDefaultLayer_OnClear(object sender, EventArgs e) {
      EventHandler handler = OnClear;
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Handles the OnEntityRemoved event of the DefaultLayer.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    void mDefaultLayer_OnEntityRemoved(object sender, EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityRemoved;
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Handles the OnEntityAdded event of the defaultLayer.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
    void defaultLayer_OnEntityAdded(object sender, EntityEventArgs e) {
      EventHandler<EntityEventArgs> handler = OnEntityAdded;
      if (handler != null)
        handler(sender, e);
    }

    /// <summary>
    /// Handles the OnAmbienceChanged event of the <see cref="Ambience"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.AmbienceEventArgs"/> instance containing the event data.</param>
    void mAmbience_OnAmbienceChanged(object sender, AmbienceEventArgs e) {
      //pass on the good news, eventually the View will be notified and the canvas will be redrawn
      RaiseOnAmbienceChanged(e);
    }
    #endregion

  }
}
