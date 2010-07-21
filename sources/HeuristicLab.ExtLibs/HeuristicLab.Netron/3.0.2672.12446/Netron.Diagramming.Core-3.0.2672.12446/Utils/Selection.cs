using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// This static class collects functions related to bundle selection
  /// </summary>
  public class Selection {

    public Selection(IController controller, IModel model) {
      this.mController = controller;
      this.mModel = model;
    }

    #region Events
    public event EventHandler OnNewSelection;
    #endregion

    #region Fields
    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies the way entities are selected.
    /// </summary>
    // ------------------------------------------------------------------
    private SelectionTypes mSelectionType = SelectionTypes.Partial;

    // ------------------------------------------------------------------
    /// <summary>
    /// The selected entities.
    /// </summary>
    // ------------------------------------------------------------------
    private CollectionBase<IDiagramEntity> mSelection =
        new CollectionBase<IDiagramEntity>();

    // ------------------------------------------------------------------
    /// <summary>
    /// A pointer to the model.
    /// </summary>
    // ------------------------------------------------------------------
    private IModel mModel;
    private IController mController;

    // ------------------------------------------------------------------
    /// <summary>
    /// A pointer to a selected connector.
    /// </summary>
    // ------------------------------------------------------------------
    private IConnector connector;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the connector selected by the user.
    /// </summary>
    /// <value>The connector.</value>
    // ------------------------------------------------------------------
    public IConnector Connector {
      get {
        return connector;
      }
      set {
        connector = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the Model we're attached to.
    /// </summary>
    // ------------------------------------------------------------------
    public IModel Model {
      get {
        return mModel;
      }
    }

    public IController Controller {
      get {
        return mController;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> SelectedItems {
      get {
        return mSelection;
      }
      internal set {
        if (value == null || value.Count == 0)
          return;
        //clear the current selection
        Clear();

        mSelection = value;
        foreach (IDiagramEntity entity in value) {
          if (entity.Group != null)
            entity.Group.IsSelected = true;
          else
            entity.IsSelected = true;
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the selected items but in flat form, i.e. the entities 
    /// inside an <see cref="IGroup"/> are collected.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> FlattenedSelectionItems {
      get {
        CollectionBase<IDiagramEntity> flatList =
            new CollectionBase<IDiagramEntity>();
        foreach (IDiagramEntity entity in mSelection) {
          if (entity is IGroup)
            Utils.TraverseCollect(entity as IGroup, ref flatList);
          else
            flatList.Add(entity);
        }
        return flatList;
      }
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates a Bitmap from the selected entities.  If no entities are
    /// selected, then 'null' is returned.
    /// </summary>
    /// <returns>Bitmap</returns>
    // ------------------------------------------------------------------
    public Bitmap ToBitmap() {
      if (SelectedItems.Count <= 0) {
        return null;
      }

      Graphics g = mController.View.Graphics;
      g.Transform = mController.View.ViewMatrix;
      g.SmoothingMode = SmoothingMode.HighQuality;
      Bundle bundle = new Bundle(SelectedItems.Copy());
      return ImageExporter.FromBundle(bundle, g);
    }

    public IConnector FindConnector(Predicate<IConnector> predicate) {
      IConnection con;
      IShape sh;

      foreach (IDiagramEntity entity in Model.Paintables) {
        if (typeof(IShape).IsInstanceOfType(entity)) {
          sh = entity as IShape;
          foreach (IConnector cn in sh.Connectors) {
            if (predicate(cn))
              return cn;
          }
        } else if (typeof(IConnection).IsInstanceOfType(entity)) {
          con = entity as IConnection;
          if (predicate(con.From))
            return con.From;
          if (predicate(con.To))
            return con.To;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds the first connector with the highest z-order under the given point
    /// </summary>
    /// <returns></returns>
    public IConnector FindShapeConnector(Point surfacePoint) {


      IShape sh;

      foreach (IDiagramEntity entity in Model.Paintables) {
        if (typeof(IShape).IsInstanceOfType(entity)) {
          sh = entity as IShape;
          foreach (IConnector cn in sh.Connectors) {
            if (cn.Hit(surfacePoint))
              return cn;
          }
        }
      }
      return null;
    }

    public IConnector FindConnectorAt(Point surfacePoint) {
      IConnection con;
      IShape sh;

      foreach (IDiagramEntity entity in Model.Paintables) {
        if (entity is IShape) {
          sh = entity as IShape;
          foreach (IConnector cn in sh.Connectors) {
            if (cn.Hit(surfacePoint))
              return cn;
          }
        } else if (entity is IConnection) {
          con = entity as IConnection;
          if (con.From.Hit(surfacePoint))
            return con.From;
          if (con.To.Hit(surfacePoint))
            return con.To;
        }
      }
      return null;
    }
    /// <summary>
    /// Collects the shapes at the given (transformed surface) location.
    /// The shapes selected in this way are available 
    /// </summary>
    /// <param name="surfacePoint">The surface point.</param>
    public void CollectEntitiesAt(
        Point surfacePoint,
        bool clearSelectionFirst) {
      if (surfacePoint == Point.Empty)
        return;
      if (mModel == null)
        return;

      // Only change the current selection if the mouse did not hit an 
      // already selected element and the element is not a group.  This
      // allows drilling down into group's children.
      if (mSelection.Count > 0) {
        foreach (IDiagramEntity entity in mSelection) {
          if ((entity.Hit(surfacePoint)) &&
              ((entity is IGroup) == false)) {
            return;
          }
        }
      }

      // Clear the current selection only if we're supposed to.
      if (clearSelectionFirst) {
        Clear();
      }

      IConnection con;
      IShape sh;

      //we use the paintables here rather than traversing the scene-graph because only
      //visible things can be collected
      //We traverse the paintables from top to bottom since the highest z-order
      //is at the top of the stack.

      for (int k = Model.Paintables.Count - 1; k >= 0; k--) {
        IDiagramEntity entity = Model.Paintables[k];

        #region we give priority to the connector selection
        if (typeof(IConnection).IsInstanceOfType(entity)) {
          con = entity as IConnection;
          if (con.From.Hit(surfacePoint)) {
            connector = con.From;
            connector.IsSelected = true;
            this.RaiseOnNewSelection();
            Invalidate();
            return;
          }
          if (con.To.Hit(surfacePoint)) {
            connector = con.To;
            connector.IsSelected = true;
            this.RaiseOnNewSelection();
            Invalidate();
            return;
          }
        } else if (entity is IGroup) {
          //should I care about the connectors at this point...?
        } else if (entity is IShape) {
          sh = entity as IShape;
          foreach (IConnector cn in sh.Connectors) {
            //if there are connectors attached to the shape connector, the attached ones should be picked up and not the one of the shape
            if (cn.Hit(surfacePoint) && cn.AttachedConnectors.Count == 0) {
              connector = cn;
              connector.IsSelected = true;
              this.RaiseOnNewSelection();
              Invalidate();//this will invalidate only the selected connector
              return; //we hit a connector and quit the selection. If the user intended to select the entity it had to be away from the connector!
            }
          }
        }

        #endregion

        #region no connector was hit, maybe the entity itself
        if (entity.Hit(surfacePoint)) {
          SelectEntity(entity, surfacePoint);
          break;
        }
        #endregion
      }
      RaiseOnNewSelection();

      // Using a full invalidate is rather expensive, so we'll only 
      // refresh the current selection.
      //Controller.View.Invalidate();
      Invalidate();
    }

    internal void SelectEntity(IDiagramEntity entity, Point surfacePoint) {
      // Groups are treated specially because we can drill-down
      // into the group.  The process of drilling is the first
      // mouse hit will select the group.  The second mouse hit
      // will select a child, if there's a child at that point.
      if (entity is IGroup) {
        if (entity.IsSelected == false) {
          entity.IsSelected = true;
          mSelection.Add(entity);
        } else {
          IGroup group = entity as IGroup;
          for (int j = group.Entities.Count - 1; j >= 0; j--) {
            IDiagramEntity child = group.Entities[j];
            if (child.Hit(surfacePoint)) {
              // Repeat the process because what if this
              // child is too a group!
              SelectEntity(child, surfacePoint);
              group.IsSelected = false;
              if (mSelection.Contains(group)) {
                mSelection.Remove(group);
              }
              break;
            }
          }
        }
      }
        //else if (entity.Group != null)
        //{
        //    //entity.Group.IsSelected = true;
        //    //mSelection.Add(entity.Group);
        //}
      else {
        entity.IsSelected = true;
        mSelection.Add(entity);
      }
    }

    private void RaiseOnNewSelection() {
      if (OnNewSelection != null)
        OnNewSelection(null, EventArgs.Empty);
    }
    /// <summary>
    /// Invalidates the current selection (either a connector or a set of entities).
    /// </summary>
    public void Invalidate() {
      if (connector != null)
        connector.Invalidate();

      foreach (IDiagramEntity entity in mSelection) {
        entity.Invalidate();
      }
    }
    /// <summary>
    /// Collects the entities inside the given rectangle.
    /// </summary>
    /// <param name="surfaceRectangle">The surface rectangle.</param>
    public void CollectEntitiesInside(Rectangle surfaceRectangle) {
      if (surfaceRectangle == Rectangle.Empty)
        return;
      this.Clear();
      foreach (IDiagramEntity entity in Model.Paintables) {
        //if the entity is part of a group we have to look at the bigger picture
        if (mSelectionType == SelectionTypes.Inclusion) {
          if (entity.Group != null) {
            //the rectangle must contain the whole group
            if (surfaceRectangle.Contains(entity.Group.Rectangle)) {
              //add the group if not already present via another group member
              if (!mSelection.Contains(entity.Group))
                mSelection.Add(entity.Group);
              continue;
            }
          } else {
            if (surfaceRectangle.Contains(entity.Rectangle)) {

              mSelection.Add(entity);
              entity.IsSelected = true;
            }
          }
        } else //the selection requires only partial overlap with the rectangle
                {
          if (entity.Group != null) {
            if (surfaceRectangle.IntersectsWith(entity.Group.Rectangle)) {
              if (!mSelection.Contains(entity.Group))
                mSelection.Add(entity.Group);
              continue;
            }
          } else {
            if (surfaceRectangle.IntersectsWith(entity.Rectangle)) {
              if (!mSelection.Contains(entity))//it could be a group which got already selected by one of its children
                            {
                mSelection.Add(entity);
                entity.IsSelected = true;
              }
            }
          }
        }



      }
      RaiseOnNewSelection();

    }
    /// <summary>
    /// Clears the current selection
    /// </summary>
    public void Clear() {
      if (connector != null) {
        connector.IsSelected = false;
        connector = null;
      }

      if (mController == null || mModel == null)
        return;
      //deselect the current ones
      foreach (IDiagramEntity entity in SelectedItems) {
        entity.IsSelected = false;
      }
      //forget the current state
      mSelection.Clear();
      if (Controller.View != null)
        Controller.View.HideTracker();
      this.RaiseOnNewSelection();
    }


    #endregion
  }
}
