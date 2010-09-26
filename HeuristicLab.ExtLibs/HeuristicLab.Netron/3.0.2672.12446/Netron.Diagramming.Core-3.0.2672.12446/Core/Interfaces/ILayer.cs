using System;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Interface of a diagram layer
  /// </summary>
  // ----------------------------------------------------------------------
  public interface ILayer {
    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is added to the layer.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityAdded;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when an entity is removed from the layer.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityRemoved;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the layer is cleared.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler OnClear;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all connections in this layer.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IConnection> Connections { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets all shapes in this layer.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IShape> Shapes { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    /// <value>The model.</value>
    // ------------------------------------------------------------------
    IModel Model { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if this layer is visible.
    /// </summary>
    // ------------------------------------------------------------------
    bool IsVisible { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the name of this layer.
    /// </summary>
    // ------------------------------------------------------------------
    string Name { get; set; }

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the entities of this layer.
    /// </summary>
    /// <value>The entities.</value>
    // ------------------------------------------------------------------
    CollectionBase<IDiagramEntity> Entities { get; }
  }
}
