using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The diagram entity is the base entity of all elements of a diagram.
  /// </summary>
  // ----------------------------------------------------------------------
  public interface IDiagramEntity :
        IPaintable,
        IMouseListener,
        IHoverListener,
        IServiceProvider,
        ISerializable,
        IVersion {

    #region Events

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the entity's properties have changed
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntityChange;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the entity is selected.  This can be different than 
    /// the OnClick because the selector can select and entity without 
    /// clicking on it.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnEntitySelect;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the user clicks on the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityEventArgs> OnClick;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a mouse button is pressed while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseDown;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when a mouse button is released while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseUp;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse is moved while over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseMove;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse enters the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseEnter;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse hovers over the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseHover;

    // ------------------------------------------------------------------
    /// <summary>
    /// Occurs when the mouse leaves the entity.
    /// </summary>
    // ------------------------------------------------------------------
    event EventHandler<EntityMouseEventArgs> OnMouseLeave;


    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center point of the paintable entity (the center of the
    /// Rectangle).
    /// </summary>
    /// <value>Point</value>
    // ------------------------------------------------------------------
    Point Center {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center of the bottom edge of the bounding rectangle.
    /// </summary>
    // ------------------------------------------------------------------
    Point BottomCenter {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the center of the top edge of the bounding rectangle.
    /// </summary>
    // ------------------------------------------------------------------
    Point TopCenter {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the top left corner of this entity, which is the same as
    /// 'Rectangle.Location'.
    /// </summary>
    // ------------------------------------------------------------------
    Point TopLeftCorner {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the top right corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    Point TopRightCorner {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bottom left corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    Point BottomLeftCorner {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bottom right corner of this entity.
    /// </summary>
    // ------------------------------------------------------------------
    Point BottomRightCorner {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the minimum size of the paintable entity.
    /// </summary>
    /// <value>Size</value>
    // ------------------------------------------------------------------
    Size MinSize {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the maximum size of the paintable entity.
    /// </summary>
    /// <value>Size</value>
    // ------------------------------------------------------------------
    Size MaxSize {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the magnification currently used by the view.
    /// </summary>
    // ------------------------------------------------------------------
    SizeF Magnification {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="T:IDiagramEntity"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    bool Enabled {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the entity is selected
    /// </summary>
    // ------------------------------------------------------------------
    bool IsSelected {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the canvas to which the entity belongs
    /// </summary>
    // ------------------------------------------------------------------
    IModel Model {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the parent of the entity.
    /// <remarks>If the parent is null the entity is a branch node in the 
    /// scene graph. The layer is never a parent because an entity can 
    /// belong to multiple layers.
    /// </remarks>
    /// </summary>
    // ------------------------------------------------------------------
    object Parent {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the name of the entity
    /// </summary>
    // ------------------------------------------------------------------
    string Name {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets whether the entity is hovered by the mouse
    /// </summary>
    // ------------------------------------------------------------------
    bool Hovered {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// General purpose tag
    /// </summary>
    // ------------------------------------------------------------------
    object Tag {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the friendly name of the entity to be displayed in the UI
    /// </summary>
    // ------------------------------------------------------------------
    string EntityName {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the index of this entity in the scene-graph.
    /// </summary>
    /// <value>The index of the scene.</value>
    // ------------------------------------------------------------------
    int SceneIndex {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the unique top-group to which this entity belongs.
    /// </summary>
    // ------------------------------------------------------------------
    IGroup Group {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="IDiagramEntity"/> is movable.
    /// </summary>
    /// <value><c>true</c> if movable; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    bool AllowMove {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="IDiagramEntity"/> can be deleted.
    /// </summary>
    /// <value><c>true</c> if deletable; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    bool AllowDelete {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this 
    /// <see cref="IDiagramEntity"/> is resizable.
    /// </summary>
    /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
    // ------------------------------------------------------------------
    bool Resizable {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the globally unique identifier of this entity
    /// </summary>
    // ------------------------------------------------------------------
    Guid Uid {
      get;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the paint style.
    /// </summary>
    /// <value>The paint style.</value>
    // ------------------------------------------------------------------
    IPaintStyle PaintStyle {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the pen style.
    /// </summary>
    /// <value>The pen style.</value>
    // ------------------------------------------------------------------
    IPenStyle PenStyle {
      get;
      set;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the layer this entity is attached to.
    /// </summary>
    // ------------------------------------------------------------------
    ILayer Layer {
      get;
      set;
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called after an entity is deleted.
    /// </summary>
    /// <param name="deleteCommand">DeleteCommand: The un/redoable command
    /// that's part of the undo/redo mechanism.</param>
    // ------------------------------------------------------------------
    void OnAfterDelete(DeleteCommand deleteCommand);

    // ------------------------------------------------------------------
    /// <summary>
    /// Called before an entity is deleted.
    /// </summary>
    /// <param name="deleteCommand">DeleteCommand: The un/redoable command
    /// that's part of the undo/redo mechanism.</param>
    // ------------------------------------------------------------------
    void OnBeforeDelete(DeleteCommand deleteCommand);

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the entity is detached from the canvas (temporarily
    /// deleted but not disposed).
    /// </summary>
    // ------------------------------------------------------------------
    void Detached(ILayer layer);

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the entity is attached to the canvas (after being
    /// temporarily detached, like in a paste operation).
    /// </summary>
    // ------------------------------------------------------------------
    void Attached(ILayer layer);

    // ------------------------------------------------------------------
    /// <summary>
    /// Generates a new Uid for this entity.
    /// </summary>
    /// <param name="recursive">if the Uid has to be changed recursively 
    /// down to the sub-entities, set to true, otherwise false.</param>
    // ------------------------------------------------------------------
    void NewUid(bool recursive);

    // ------------------------------------------------------------------
    /// <summary>
    /// The custom elements to be added to the menu on a per-entity basis
    /// </summary>
    /// <returns>ToolStripItem[]</returns>
    // ------------------------------------------------------------------
    ToolStripItem[] Menu();

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the onclick event.
    /// </summary>
    /// <param name="e"></param>
    // ------------------------------------------------------------------
    void RaiseOnClick(EntityEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseDown event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseDown(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseUp event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseUp(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseMove event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseMove(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseEnter event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseEnter(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseHover event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseHover(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnMouseLeave event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> 
    /// instance containing the event data.</param>
    // ------------------------------------------------------------------
    void RaiseOnMouseLeave(EntityMouseEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns whether the entity was hit at the given location
    /// </summary>
    /// <param name="p">a Point location</param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    bool Hit(Point p);

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the entity
    /// </summary>
    // ------------------------------------------------------------------
    void Invalidate();

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates a certain rectangle of the canvas
    /// </summary>
    /// <param name="rectangle"></param>
    // ------------------------------------------------------------------
    void Invalidate(Rectangle rectangle);

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the entity to the given location
    /// </summary>
    /// <param name="p">a Point location</param>
    // ------------------------------------------------------------------
    void MoveBy(Point p);

    #endregion
  }
}
