using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract base class for simple shapes. 
  /// <para>A simple shape does not contain sub-elements (aka shape 
  /// material, see <see cref="IShapeMaterial"/>) but this does
  /// not mean you cannot paint sub-elements. In fact, this shape type 
  /// corresponds to the old base class in the previous Netron graph 
  /// library (i.e. before version 2.3).
  /// <seealso cref="ComplexShapeBase"/>
  /// </para>
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract partial class SimpleShapeBase :
      ShapeBase,
      ISimpleShape,
      ITextProvider {
    // ------------------------------------------------------------------
    /// <summary>
    /// The amount to inflate the text area (mTextRectangle) by.
    /// </summary>
    // ------------------------------------------------------------------
    protected const int TextRectangleInflation = 5;

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// SimpleShapeBase.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double simpleShapeBaseVersion = 1.0;

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if this shape is auto-sized to fit the text.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mAutoSize = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if this shape allows in-place text editing.  If set to
    /// true, then the TextEditor is displayed when the shape is 
    /// double-clicked.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mAllowTextEditing = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies the number of clicks required to launch the in-place
    /// TextEditor.
    /// </summary>
    // ------------------------------------------------------------------
    protected int mEditTextClicks = 2;

    // ------------------------------------------------------------------
    /// <summary>
    /// The text for this shape.
    /// </summary>
    // ------------------------------------------------------------------
    protected string mText = string.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// The Rectangle inside which the text is drawn.
    /// </summary>
    // ------------------------------------------------------------------
    protected Rectangle mTextArea;

    // ------------------------------------------------------------------
    /// <summary>
    /// The style of the text to draw.
    /// </summary>
    // ------------------------------------------------------------------
    protected ITextStyle mTextStyle = new TextStyle(
        Color.Black,
        new Font("Arial", 10),
        StringAlignment.Center,
        StringAlignment.Center);

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return simpleShapeBaseVersion;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the style of the text to use.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual ITextStyle TextStyle {
      get {
        return mTextStyle;
      }
      set {
        mTextStyle = value;
        mTextStyle.TextStyleChanged +=
            new TextStyleChangedEventHandler(HandleTextStyleChanged);
        Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns if text editing is allowed.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual bool AllowTextEditing {
      get {
        return this.mAllowTextEditing;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the number of mouse clicks required to launch the in-place
    /// text editor.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual int EditTextClicks {
      get {
        return mEditTextClicks;
      }
      set {
        mEditTextClicks = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether to autosize the label in 
    /// function of the string content.
    /// </summary>
    /// <value><c>true</c> if [auto size]; otherwise, <c>false</c>.
    /// </value>
    // ------------------------------------------------------------------
    public virtual bool AutoSize {
      get {
        return mAutoSize;
      }
      set {
        mAutoSize = value;
        AutoReSize(mText);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text to display in the TextArea.
    /// </summary>
    // ------------------------------------------------------------------
    [Browsable(true),
    Description("The text shown on the shape"),
    Category("Layout")]
    public virtual string Text {
      get {
        return mText;
      }
      set {
        if (value == null) {
          throw new InconsistencyException(
              "The text property cannot be 'null'");
        }

        mText = value;
        if (mAutoSize) {
          AutoReSize(value);
        }
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the text rectangle.
    /// </summary>
    /// <value>The text rectangle.</value>
    // ------------------------------------------------------------------
    public virtual Rectangle TextArea {
      get {
        return mTextArea;
      }
      set {
        mTextArea = value;
      }
    }
    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    ///<summary>
    ///Default constructor.
    ///</summary>
    // ------------------------------------------------------------------
    public SimpleShapeBase()
      : base() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:SimpleShapeBase"/> 
    /// class.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/></param>
    // ------------------------------------------------------------------
    public SimpleShapeBase(IModel model)
      : base(model) {
    }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void Initialize() {
      base.Initialize();
      mTextArea = Rectangle;

      mTextArea.Inflate(
          -TextRectangleInflation,
          -TextRectangleInflation);

      this.mTextStyle = new TextStyle(
          Color.Black,
          new Font("Arial", 12),
          StringAlignment.Center,
          StringAlignment.Center);

      this.mTextStyle.TextStyleChanged +=
          new TextStyleChangedEventHandler(HandleTextStyleChanged);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the text style is changed.  This shape is invalidated
    /// so the new text style is reflected.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">TextStyleChangedEventArgs</param>
    // ------------------------------------------------------------------
    protected virtual void HandleTextStyleChanged(
        object sender,
        TextStyleChangedEventArgs e) {
      Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Moves the entity with the given shift vector
    /// </summary>
    /// <param name="p">Represent a shift-vector, not the absolute 
    /// position!</param>
    // ------------------------------------------------------------------
    public override void MoveBy(Point p) {
      base.MoveBy(p);
      this.mTextArea.X += p.X;
      this.mTextArea.Y += p.Y;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Transforms this shape to the given new rectangle.
    /// </summary>
    /// <param name="x">The x-coordinate of the new rectangle.</param>
    /// <param name="y">The y-coordinate of the new rectangle.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    // ------------------------------------------------------------------
    public override void Transform(
        int x,
        int y,
        int width,
        int height) {
      base.Transform(x, y, width, height);
      mTextArea = Rectangle;

      mTextArea.Inflate(
          -TextRectangleInflation,
          -TextRectangleInflation);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Re-calculates the bounds of this shape to fit the text specified.
    /// </summary>
    /// <param name="value">string: The text to fit to.</param>
    // ------------------------------------------------------------------
    private void AutoReSize(string value) {
      SizeF size = TextRenderer.MeasureText(
          value,
          mTextStyle.Font);

      Rectangle rec = new Rectangle(
          Rectangle.Location,
          Size.Round(size));

      rec.Inflate(TextRectangleInflation, TextRectangleInflation);

      rec.Offset(TextRectangleInflation, TextRectangleInflation);

      Transform(rec);
    }

    #endregion
  }
}
