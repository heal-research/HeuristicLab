using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Interface of the "Model" in the Model - View - Controller framework.
    /// </summary>    
    // ----------------------------------------------------------------------
    public interface  IModel : IVersion
    {

        #region Events

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when the current page has changed.
        /// </summary>
        // ------------------------------------------------------------------
        event CurrentPageChangedEventHandler OnCurrentPageChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when the <see cref="Ambience"/> is changed.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<AmbienceEventArgs> OnAmbienceChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs the collection of connections is changed
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<ConnectionCollectionEventArgs> OnConnectionCollectionChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when the diagram information has changed
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<DiagramInformationEventArgs> OnDiagramInformationChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when an underlying element (usually an entity) asks to 
        /// repaint the whole canvas.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler OnInvalidate;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when an underlying element (usually an entity) asks to 
        /// repaint part of the canvas.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<RectangleEventArgs> OnInvalidateRectangle;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when an entity is added to the model.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<EntityEventArgs> OnEntityAdded;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when an entity is removed from the model.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<EntityEventArgs> OnEntityRemoved;

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when the cursor changes and the surface has to effectively 
        /// show a different cursor.
        /// </summary>
        // ------------------------------------------------------------------
        event EventHandler<CursorEventArgs> OnCursorChange;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets if all shape's connectors should be shown.
        /// </summary>
        // ------------------------------------------------------------------
        bool ShowConnectors
        {
            get;
            set;
        }

        Selection Selection { get; set; }

        float MeasurementScale 
        { 
            get; 
            set;
        }

        GraphicsUnit MeasurementUnits 
        { 
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        // ------------------------------------------------------------------
        IShape LayoutRoot 
        { 
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        // ------------------------------------------------------------------
        CollectionBase<IConnection> Connections 
        { 
            get; 
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the shapes.
        /// </summary>
        /// <value>The shapes.</value>
        // ------------------------------------------------------------------
        CollectionBase<IShape> Shapes 
        { 
            get; 
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the paintables.
        /// </summary>
        /// <value>The paintables.</value>
        // ------------------------------------------------------------------
        CollectionBase<IDiagramEntity> Paintables 
        { 
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        // ------------------------------------------------------------------
        IPage CurrentPage 
        { 
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the pages of the diagram control.  Use method 'AddPage' to
        /// add a page so the page gets attached to this Model.
        /// </summary>
        /// <value>The pages.</value>
        // ------------------------------------------------------------------
        CollectionBase<IPage> Pages 
        { 
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the default page.
        /// </summary>
        /// <value>The default page.</value>
        // ------------------------------------------------------------------
        IPage DefaultPage 
        { 
            get;
        }

        #endregion

        #region Methods

        #region Diagram actions

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the number of shapes in the current page that are of
        /// the type specified.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>int</returns>
        // ------------------------------------------------------------------
        int NumberOfShapes(Type type);

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds an entity to the diagram
        /// </summary>
        /// <param name="entity">IDiagramEntity: The entity to add.</param>
        /// <returns>IDiagramEntity: The added entity.</returns>
        // ------------------------------------------------------------------
        IDiagramEntity AddEntity(IDiagramEntity entity);

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="connection">The connection.</param>
        // ------------------------------------------------------------------
        IConnection AddConnection(IConnection connection);

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a page.  This should be used when adding pages rather than
        /// though the Pages property so the page gets attached to the Model.
        /// </summary>
        /// <param name="page">IPage: The page to add.</param>
        /// <returns>IPage</returns>
        // ------------------------------------------------------------------
        IPage AddPage(IPage page);

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a page.  This should be used when adding pages rather than
        /// though the Pages property so the page gets attached to the Model.
        /// The page name is set to "Page" plus the new number of pages.
        /// For example, if there are currently two pages, then "Page3" is 
        /// set as the new page name.
        /// </summary>
        /// <returns>IPage</returns>
        // ------------------------------------------------------------------
        IPage AddPage();

        // ------------------------------------------------------------------
        /// <summary>
        /// Deletes the page specified if it is not the default page.
        /// </summary>
        /// <param name="page">IPage: The page to remove.</param>
        /// <param name="allowWarnings">bool: Specifies if the user should
        /// be given the option to cancel the action if the current page
        /// has entities.  Also, when set to true, if the current page is
        /// the default page, then a message box is shown informing the
        /// user that the default page cannot be deleted.</param>
        /// <returns>bool: If the delete was successful.  True is returned
        /// if the current page was removed.</returns>
        // ------------------------------------------------------------------
        bool RemovePage(IPage page, bool allowWarnings);

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns a new page name that's unique from all the others.
        /// </summary>
        /// <returns>string: Returns "Page" plus the number of pages IF A
        /// NEW PAGE WERE ADDED.  For example, if there are currently two
        /// pages, then "Page3" is returned.</returns>
        // ------------------------------------------------------------------
        string GetDefaultNewPageName();

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a shape to the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        // ------------------------------------------------------------------
        IShape AddShape(IShape shape);

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the shape at the specified location.  If no shape could be
        /// found then 'null' is returned.
        /// </summary>
        /// <param name="surfacePoint">Point: The location.</param>
        /// <returns>IShape</returns>
        // ------------------------------------------------------------------
        IShape GetShapeAt(Point surfacePoint);

        // ------------------------------------------------------------------
        /// <summary>
        /// Clears the diagram.
        /// </summary>
        // ------------------------------------------------------------------
        void Clear();

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the shape from the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        // ------------------------------------------------------------------
        void RemoveShape(IShape shape);

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        // ------------------------------------------------------------------
        void Remove(IDiagramEntity entity);

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes all entities that are currently selected.
        /// </summary>
        /// <param name="entity">The entity.</param>
        // ------------------------------------------------------------------
        void RemoveSelectedItems();

        // ------------------------------------------------------------------
        /// <summary>
        /// Sends the given entity to the front of the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        // ------------------------------------------------------------------
        void SendToFront(IDiagramEntity entity);

        // ------------------------------------------------------------------
        /// <summary>
        /// Sends the given entity backwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        // ------------------------------------------------------------------
        void SendBackwards(IDiagramEntity entity, int zShift);

        // ------------------------------------------------------------------
        /// <summary>
        /// Sends the given entity to the back of the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        // ------------------------------------------------------------------
        void SendToBack(IDiagramEntity entity);

        // ------------------------------------------------------------------
        /// <summary>
        /// Sends the given entity forwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        // ------------------------------------------------------------------
        void SendForwards(IDiagramEntity entity);

        // ------------------------------------------------------------------
        /// <summary>
        /// Sends the given entity forwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        // ------------------------------------------------------------------
        void SendForwards(IDiagramEntity entity, int zShift);

        // ------------------------------------------------------------------
        /// <summary>
        /// Unwraps the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        // ------------------------------------------------------------------
        void Unwrap(CollectionBase<IDiagramEntity> collection);

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="page">The page.</param>
        // ------------------------------------------------------------------
        void SetCurrentPage(IPage page);

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        // ------------------------------------------------------------------
        void SetCurrentPage(int pageIndex);

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the on invalidate.
        /// </summary>
        // ------------------------------------------------------------------
        void RaiseOnInvalidate();

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the OnInvalidateRectangle event.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        // ------------------------------------------------------------------
        void RaiseOnInvalidateRectangle(Rectangle rectangle);

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the OnCursorChange event.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        // ------------------------------------------------------------------
        void RaiseOnCursorChange(Cursor cursor);

        #endregion
    }
}
