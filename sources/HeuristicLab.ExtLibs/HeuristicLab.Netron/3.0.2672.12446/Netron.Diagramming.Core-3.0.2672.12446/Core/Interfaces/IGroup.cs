
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The group interface.
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IGroup : IDiagramEntity {
    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the entities contained in this group.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IDiagramEntity> Entities { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the group as a shape should be painted on 
    /// the canvas.
    /// </summary>
    // ------------------------------------------------------------------
    bool EmphasizeGroup { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the bounding rectangle that encompasses all entities.
    /// </summary>
    // ------------------------------------------------------------------
    void CalculateRectangle();

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if un-grouping of the entities is allowed.
    /// </summary>
    // ------------------------------------------------------------------
    bool CanUnGroup {
      get;
      set;
    }
  }
}
