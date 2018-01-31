using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract base class for complex shapes <see cref="ComplexShapeBase"/>.
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract partial class ComplexShapeBase :
      ShapeBase,
      IComplexShape,
      IMouseListener,
      IHoverListener {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// ComplexShapeBase.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double complexShapeVersion = 1.0;

    /// <summary>
    /// the text on the bundle
    /// </summary>
    protected string mText = string.Empty;

    /// <summary>
    /// the shape children or sub-controls
    /// </summary>
    protected CollectionBase<IShapeMaterial> mChildren;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return complexShapeVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text of the bundle
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(true),
    Description("The text shown on the shape"), Category("Layout")]
    public virtual string Text {
      get {
        return mText;
      }
      set {
        mText = value;
        this.Invalidate();
      }
    }

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    ///<summary>
    ///Default constructor
    ///</summary>
    // ------------------------------------------------------------------
    public ComplexShapeBase()
      : base() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexShapeBase"/> 
    /// class.
    /// </summary>
    /// <param name="model"></param>
    // ------------------------------------------------------------------
    public ComplexShapeBase(IModel model)
      : base(model) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void Initialize() {
      base.Initialize();
      mChildren = new CollectionBase<IShapeMaterial>();
      mChildren.OnItemAdded +=
          new EventHandler<CollectionEventArgs<IShapeMaterial>>
          (mChildren_OnItemAdded);
    }

    #endregion

    #region Methods
    // ------------------------------------------------------------------

    /// <summary>
    /// Sets some contextual properties of the <see cref="Children"/> 
    /// when a new item is added.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    // ------------------------------------------------------------------
    void mChildren_OnItemAdded(object sender, CollectionEventArgs<IShapeMaterial> e) {
      e.Item.Shape = this;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Overrides the abstract paint method
    /// </summary>
    /// <param name="g">a graphics object onto which to paint</param>
    // ------------------------------------------------------------------
    public override void Paint(Graphics g) {

      base.Paint(g);



      foreach (IPaintable material in Children) {
        material.Paint(g);
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the entity with the given shift
    /// </summary>
    /// <param name="p">represent a shift-vector, not the absolute 
    /// position!</param>
    // ------------------------------------------------------------------
    public override void MoveBy(Point p) {
      base.MoveBy(p);
      Rectangle rec;
      foreach (IShapeMaterial material in Children) {
        rec = material.Rectangle;

        rec.Offset(p.X, p.Y);
        material.Transform(rec);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Transforms the entity to the given new rectangle.
    /// </summary>
    /// <param name="x">The x-coordinate of the new rectangle.</param>
    /// <param name="y">The y-coordinate of the new rectangle.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    // ------------------------------------------------------------------
    public override void Transform(int x, int y, int width, int height) {
      if (Children != null && Children.Count > 0) {

        int a, b, w, h;
        foreach (IShapeMaterial material in Children) {

          if (material.Gliding) {
            a = Convert.ToInt32(Math.Round(((double)material.Rectangle.X - (double)Rectangle.X) / (double)Rectangle.Width * width, 1) + x);
            b = Convert.ToInt32(Math.Round(((double)material.Rectangle.Y - (double)Rectangle.Y) / (double)Rectangle.Height * height, 1) + y);
          } else //shift the material, do not scale the position with respect to the sizing of the shape
                    {
            a = material.Rectangle.X - Rectangle.X + x;
            b = material.Rectangle.Y - Rectangle.Y + y;
          }
          if (material.Resizable) {
            w = Convert.ToInt32(Math.Round(((double)material.Rectangle.Width) / ((double)Rectangle.Width), 1) * width);
            h = Convert.ToInt32(Math.Round(((double)material.Rectangle.Height) / ((double)Rectangle.Height), 1) * height);
          } else {
            w = material.Rectangle.Width;
            h = material.Rectangle.Height;
          }

          material.Transform(new Rectangle(a, b, w, h));
        }
      }
      base.Transform(x, y, width, height);
    }
    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the children.
    /// </summary>
    /// <value>The children.</value>
    // ------------------------------------------------------------------
    public virtual CollectionBase<IShapeMaterial> Children {
      get {
        return mChildren;
      }
      set {
        throw new InconsistencyException("You cannot set the children, use the existing collection and the clear() method.");
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an 
    /// object that provides custom support to other objects.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of 
    /// service object to get.</param>
    /// <returns>
    /// A service object of type serviceType.-or- null if there is no 
    /// service object of type serviceType.
    /// </returns>
    // ------------------------------------------------------------------
    public override object GetService(Type serviceType) {
      if (Services.ContainsKey(serviceType))
        return Services[serviceType];
      else {
        return null;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// <see cref="IMouseListener"/> implementation.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override bool MouseDown(MouseEventArgs e) {
      base.MouseDown(e);
      IMouseListener listener;
      foreach (IShapeMaterial material in mChildren) {
        if (material.Rectangle.Contains(e.Location) && material.Visible) {
          listener = material.GetService(typeof(
              IMouseListener)) as IMouseListener;
          if (listener != null)
            if (listener.MouseDown(e))
              return true;
        }

      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// <see cref="IMouseListener"/> implementation.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override void MouseMove(MouseEventArgs e) {
      base.MouseMove(e);
      IMouseListener listener;
      foreach (IShapeMaterial material in mChildren) {
        if ((material.Rectangle.Contains(e.Location)) &&
            (material.Visible)) {
          listener = material.GetService(typeof
              (IMouseListener)) as IMouseListener;

          if (listener != null) {
            listener.MouseMove(e);
          }
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// <see cref="IMouseListener"/> implementation.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override void MouseUp(MouseEventArgs e) {
      base.MouseUp(e);
      IMouseListener listener;
      foreach (IShapeMaterial material in mChildren) {
        if (material.Rectangle.Contains(e.Location) && material.Visible) {
          listener = material.GetService(typeof(IMouseListener)) as IMouseListener;
          if (listener != null) listener.MouseUp(e);
        }
      }
    }



    #region IHoverListener Members

    private IHoverListener currentHoveredMaterial;

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the MouseHover event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override void MouseHover(MouseEventArgs e) {
      base.MouseHover(e);
      IHoverListener listener;
      foreach (IShapeMaterial material in this.Children) {
        if (material.Rectangle.Contains(e.Location) && material.Visible) //we caught an material and it's visible
                {
          listener = material.GetService(typeof(IHoverListener)) as IHoverListener;
          if (listener != null) //the caught material does listen
                    {
            if (currentHoveredMaterial == listener) //it's the same as the previous time
              listener.MouseHover(e);
            else //we moved from one material to another listening material
                        {
              if (currentHoveredMaterial != null) //tell the previous material we are leaving
                currentHoveredMaterial.MouseLeave(e);
              listener.MouseEnter(e); //tell the current one we enter
              currentHoveredMaterial = listener;
            }
          } else //the caught material does not listen
                    {
            if (currentHoveredMaterial != null) {
              currentHoveredMaterial.MouseLeave(e);
              currentHoveredMaterial = null;
            }
          }
          return; //only one material at a time
        }

      }
      if (currentHoveredMaterial != null) {
        currentHoveredMaterial.MouseLeave(e);
        currentHoveredMaterial = null;
      }

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnMouseEnter event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override void MouseEnter(MouseEventArgs e) {
      base.MouseEnter(e);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnMouseLeave event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public override void MouseLeave(MouseEventArgs e) {
      base.MouseLeave(e);
    }

    #endregion
  }
}
