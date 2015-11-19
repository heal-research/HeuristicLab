using System.Drawing;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The interface of a shape. This interface comes in two inherited 
  /// flavors; <see cref="ISimpleShape"/> and <see cref="IComplexShape"/>.
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IShape : IDiagramEntity {
    #region Events

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the connectors are drawn.
    /// </summary>
    // ------------------------------------------------------------------
    bool ShowConnectors {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the connectors of the shape.
    /// </summary>
    /// <remarks>Connectors are defined at design time, hence the fact 
    /// that you cannot set the connector collection.</remarks>
    // ------------------------------------------------------------------
    CollectionBase<IConnector> Connectors { get; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the location of the shape
    /// </summary>
    // ------------------------------------------------------------------
    Point Location { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the height of the shape
    /// </summary>
    // ------------------------------------------------------------------
    int Height { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the width of the shape.
    /// </summary>
    // ------------------------------------------------------------------
    int Width { get; set; }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the image for this shape when displaying in a library (i.e.
    /// toolbox).
    /// </summary>
    // ------------------------------------------------------------------
    Image LibraryImage {
      get;
    }

    #endregion

    #region Methods
    // ------------------------------------------------------------------
    /// <summary>
    /// Returns whether the given point hits one of the connectors of 
    /// the bundle.
    /// </summary>
    /// <param name="p">a location, usually corresponds to the mouse 
    /// location.</param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    IConnector HitConnector(Point p);

    // ------------------------------------------------------------------
    /// <summary>
    /// Maps the shape to another rectangle, including all its sub-entities 
    /// and materials.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    // ------------------------------------------------------------------
    void Transform(int x, int y, int width, int height);
    #endregion
  }
}