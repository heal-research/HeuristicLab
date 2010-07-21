using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// This tool pastes items from the clipboard to the canvas.
  /// </summary>
  // ----------------------------------------------------------------------
  public class PasteTool : AbstractTool {
    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies where the items from the clipboard will be 
    /// inserted at.
    /// </summary>
    // ------------------------------------------------------------------
    public static Point InsertionPoint = new Point(50, 50);

    #region Fields
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
    /// </summary>
    /// <param name="name">The name of the tool.</param>
    public PasteTool(string name)
      : base(name) {
    }
    #endregion

    #region Methods

    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    protected override void OnActivateTool() {
      try {
        // Used to get a collection of entities from the 
        // NetronClipboard.
        Type entitiesType = typeof(CollectionBase<IDiagramEntity>);

        // First calculate the insertion point based on the
        // current location of the cursor.
        InsertionPoint = Point.Round(
        Controller.View.ViewToWorld(
        Controller.View.DeviceToView(
        Controller.ParentControl.PointToClient(Cursor.Position))));

        IDataObject data = Clipboard.GetDataObject();
        string format = typeof(CopyTool).FullName;
        if (data.GetDataPresent(format)) {
          MemoryStream stream = data.GetData(format) as MemoryStream;
          CollectionBase<IDiagramEntity> collection = null;
          GenericFormatter<BinaryFormatter> f =
              new GenericFormatter<BinaryFormatter>();
          //Anchors collection is a helper collection to re-connect connectors to their parent
          Anchors.Clear();
          //but is it actually a stream coming this application?
          collection = f.Deserialize<CollectionBase<IDiagramEntity>>(stream);
          UnwrapBundle(collection);

        } else if (NetronClipboard.ContainsData(entitiesType)) {
          CollectionBase<IDiagramEntity> collection =
              (CollectionBase<IDiagramEntity>)NetronClipboard.Get(
              entitiesType);

          UnwrapBundle(collection.DeepCopy());
        } else if (data.GetDataPresent(DataFormats.Bitmap)) {
          Bitmap bmp = data.GetData(DataFormats.Bitmap) as Bitmap;
          if (bmp != null) {
            #region Unwrap into an image shape
            //TODO: Insert the image shape here
            ImageShape shape = new ImageShape();
            shape.Image = bmp;
            shape.Location = InsertionPoint;
            Controller.Model.AddShape(shape);
            #endregion
          }
        } else if (data.GetDataPresent(DataFormats.Text)) {
          string text = (string)data.GetData(typeof(string));
          TextOnly textShape = new TextOnly(Controller.Model);
          textShape.Text = text;
          textShape.Location = InsertionPoint;
          Controller.Model.AddShape(textShape);
        }
      }
      catch (Exception exc) {
        throw new InconsistencyException("The Paste operation failed.", exc);
      }
      finally {
        DeactivateTool();
      }

    }

    #endregion

    /// <summary>
    /// Unwraps the given bundle to the diagram.
    /// </summary>
    /// <param name="collection">CollectionBase<IDiagramEntity></param>
    void UnwrapBundle(CollectionBase<IDiagramEntity> collection) {
      if (collection != null) {
        #region Unwrap the bundle
        this.Controller.Model.Unwrap(collection);

        Rectangle rec = Utils.BoundingRectangle(collection);

        // The InsertionPoint specifies where the pasted
        // entities should be.  So, shift each entity in the
        // collection to the InsertionPoint.
        Point delta = new Point(
            InsertionPoint.X - rec.Location.X,
            InsertionPoint.Y - rec.Location.Y);
        //delta.Offset(rec.Location);

        foreach (IDiagramEntity entity in collection) {
          entity.MoveBy(delta);
        }

        // Recalculate the bounding rectangle and invalidate
        // that area on the canvas.
        rec = Utils.BoundingRectangle(collection);
        rec.Inflate(30, 30);
        this.Controller.View.Invalidate(rec);
        #endregion
      }
    }
  }

}
