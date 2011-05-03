
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Interface of a <see cref="Bundle"/>
  /// </summary>
  public interface IBundle : IDiagramEntity {
    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the entities in the <see cref="Bundle"/>
    /// </summary>
    /// <value>The entities.</value>
    // ------------------------------------------------------------------
    CollectionBase<IDiagramEntity> Entities { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the 'IsSelected' property to false for all entities in the 
    /// bundle.
    /// </summary>
    // ------------------------------------------------------------------
    void DeSelectAll();

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the 'Hovered' property for all entities in the bundle to
    /// the value specified.
    /// </summary>
    // ------------------------------------------------------------------
    void SetHovered(bool isHovered);
  }
}
