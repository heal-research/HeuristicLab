using System;
using System.Drawing;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The interface of a page
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IPage {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is added.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityAdded;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is removed.
    /// <remarks>This event usually is bubbled from one of the 
    /// layers</remarks>
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityRemoved;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the page is cleared
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler OnClear;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the Ambience has changed
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<AmbienceEventArgs> OnAmbienceChanged;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Name of this page.
    /// </summary>
    // ------------------------------------------------------------------
    string Name {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all the connections in the page.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IConnection> Connections {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all entities in this page.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IDiagramEntity> Entities {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all shapes in this page.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IShape> Shapes {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the layers.
    /// </summary>
    /// <value>The layers.</value>
    // ------------------------------------------------------------------
    CollectionBase<ILayer> Layers {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the default layer.
    /// </summary>
    /// <value>The default layer.</value>
    // ------------------------------------------------------------------
    ILayer DefaultLayer {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the ambience.
    /// </summary>
    /// <value>The ambience.</value>
    // ------------------------------------------------------------------
    Ambience Ambience {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets a reference to the model
    /// </summary>
    // ------------------------------------------------------------------
    IModel Model {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the scaling factor of this page.
    /// </summary>
    // ------------------------------------------------------------------
    SizeF Magnification {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the origin of this page.
    /// </summary>
    // ------------------------------------------------------------------
    Point Origin {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the page bounds, taking into account landscape and page
    /// settings (for printing).
    /// </summary>
    // ------------------------------------------------------------------
    RectangleF Bounds {
      get;
    }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the layer that has the entity specified.  If the entity
    /// specified could not be found, 'null' is returned.
    /// </summary>
    /// <param name="entity">IDiagramEntity</param>
    /// <returns>ILayer</returns>
    // ------------------------------------------------------------------
    ILayer GetLayer(IDiagramEntity entity);

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the page to the drawing surface specified.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <param name="showGrid">bool</param>
    // ------------------------------------------------------------------
    void Paint(Graphics g, bool showGrid);

  }
}
