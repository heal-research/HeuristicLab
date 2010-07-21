using System;
using System.Drawing;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// Abstract base class for shape materials
  /// </summary>
  public abstract partial class ShapeMaterialBase : IShapeMaterial {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// ShapeMaterialBase.
    /// </summary>
    // ------------------------------------------------------------------
    protected double shapeMaterialBaseVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Gliding field
    /// </summary>
    // ------------------------------------------------------------------
    private bool mGliding = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Resizable field
    /// </summary>
    // ------------------------------------------------------------------
    private bool mResizable = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Rectangle field
    /// </summary>
    // ------------------------------------------------------------------
    private Rectangle mRectangle = Rectangle.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Shape field
    /// </summary>
    // ------------------------------------------------------------------
    private IShape mShape;

    // ------------------------------------------------------------------
    /// <summary>
    /// the Visible field
    /// </summary>
    // ------------------------------------------------------------------
    private bool mVisible = true;

    protected ITextStyle myTextStyle = new TextStyle(
        Color.Black,
        new Font("Arial", 10),
        StringAlignment.Near,
        StringAlignment.Near);

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return shapeMaterialBaseVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the TextStyle.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual ITextStyle TextStyle {
      get {
        return myTextStyle;
      }
      set {
        myTextStyle = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Gliding
    /// </summary>
    // ------------------------------------------------------------------
    public bool Gliding {
      get { return mGliding; }
      set { mGliding = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Resizable
    /// </summary>
    // ------------------------------------------------------------------
    public bool Resizable {
      get { return mResizable; }
      set { mResizable = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Rectangle
    /// </summary>
    // ------------------------------------------------------------------
    public virtual Rectangle Rectangle {
      get { return mRectangle; }
      internal set {
        mRectangle = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Shape
    /// </summary>
    // ------------------------------------------------------------------
    public virtual IShape Shape {
      get { return mShape; }
      set { mShape = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if this material is visible.
    /// </summary>
    // ------------------------------------------------------------------
    public bool Visible {
      get { return mVisible; }
      set { mVisible = value; }
    }
    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor
    ///</summary>
    protected ShapeMaterialBase() {
    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the min size needed to fit this material in.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <returns>Size</returns>
    // ------------------------------------------------------------------
    public abstract Size CalculateMinSize(Graphics g);

    /// <summary>
    /// Transforms the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    public virtual void Transform(Rectangle rectangle) {
      this.mRectangle = rectangle;
    }

    /// <summary>
    /// Paints the entity using the given graphics object
    /// </summary>
    /// <param name="g"></param>
    public abstract void Paint(Graphics g);


    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
    /// </returns>
    public virtual object GetService(Type serviceType) {
      return null;
    }

    #endregion

  }
}
