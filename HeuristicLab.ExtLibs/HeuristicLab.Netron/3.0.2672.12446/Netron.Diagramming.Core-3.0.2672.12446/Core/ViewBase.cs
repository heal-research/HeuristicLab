using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Abstract view base class which serves/renders both the Win and Web 
  /// diagrams.
  /// <remarks>
  /// <para>Click here for an explanation of the Model-View-Controller
  /// pattern: http://st-www.cs.uiuc.edu/users/smarch/st-docs/mvc.html
  /// </para>
  /// <para>See Bob Powell's explaination of the different coordinate 
  /// systems here: http://www.bobpowell.net/mappingmodes.htm
  /// The rendering pipeline with the different ratiometric conversions 
  /// is not really easy.
  /// </para>
  /// </remarks>
  /// </summary>
  // ----------------------------------------------------------------------
  public abstract class ViewBase : IDisposable, IView {

    #region Events
    /// <summary>
    /// Occurs when the cursor changes and the surface has to effectively show a different cursor
    /// </summary>
    public event EventHandler<CursorEventArgs> OnCursorChange;
    /// <summary>
    /// Occurs when the background color has to change. The view could paint the backcolor itself via the PaintBackground method
    /// but this would be less efficient than the built-in painting of the Control.
    /// </summary>
    public event EventHandler<ColorEventArgs> OnBackColorChange;
    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if all shape's connectors are shown.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mShowConnectors = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the grid is drawn.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mShowGrid = true;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if a page's title is drawn when 'ShowPage' is set to
    /// true.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mShowPageTitle = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if the page is rendered and printed in landscape
    /// orientation.
    /// </summary>
    // ------------------------------------------------------------------
    protected bool mLandscape = true;

    protected Matrix mViewMatrix;
    protected Matrix mInvertedViewMatrix;
    private float dpiX = 96F;

    private float dpiY = 96F;
    /// <summary>
    /// the ShowRulers field
    /// </summary>
    private bool mShowRulers;
    private bool mIsSuspended;
    /// <summary>
    /// pointer to the current tracker
    /// </summary>
    private ITracker mTracker;
    /// <summary>
    /// pointer to the current ants or selector
    /// </summary>
    private IAnts mAnts;
    /// <summary>
    /// pointer to the current ghost bundle or region
    /// </summary>
    private IGhost mGhost;
    /// <summary>
    /// the data model
    /// </summary>
    private IModel mModel;
    /// <summary>
    /// the brush to be used for the background
    /// </summary>
    private Brush mBackgroundBrush;
    /// <summary>
    /// the parent control (aka surface)
    /// </summary>
    private IDiagramControl parentControl;
    /// <summary>
    /// the rectangle of the client surface
    /// </summary>
    private Rectangle clientRectangle;
    /// <summary>
    /// the current cursor
    /// </summary>
    private Cursor mCurrentCursor = Cursors.Default;

    /// <summary>
    /// the horizontal ruler
    /// </summary>
    private HorizontalRuler horizontalRuler;

    /// <summary>
    /// the vertical ruler
    /// </summary>                                                
    private VerticalRuler verticalRuler;
    private int mRulerSize = 16;



    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the grid is to be drawn.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowGrid {
      get {
        return mShowGrid;
      }
      set {
        mShowGrid = value;
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if all shape's connectors are shown.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowConnectors {
      get {
        return this.mModel.ShowConnectors;
      }
      set {
        this.mModel.ShowConnectors = value;
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the page's title is drawn when 'ShowPage' is set
    /// to true.
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowPageTitle {
      get {
        return mShowPageTitle;
      }
      set {
        mShowPageTitle = value;
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the size of each page in thousandths of an inch.
    /// </summary>
    // ------------------------------------------------------------------
    public Size PageSize {
      get {
        return mModel.CurrentPage.Ambience.PageSize;
      }
      set {
        foreach (IPage page in Model.Pages) {
          page.Ambience.PageSize = value;
        }
        UpdateViewMatrix();
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if each page is rendered and printed in landscape
    /// orientation.
    /// </summary>
    // ------------------------------------------------------------------
    public bool Landscape {
      get {
        return mLandscape;
      }
      set {
        mLandscape = value;
        foreach (IPage page in mModel.Pages) {
          page.Ambience.Landscape = value;
        }
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets if the rulers are shown are not.  I don't recommend
    /// showing them just yet :)
    /// </summary>
    // ------------------------------------------------------------------
    public bool ShowRulers {
      get {
        return mShowRulers;
      }
      set {
        mShowRulers = value;
        UpdateViewMatrix();
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <value>The view matrix.</value>
    // ------------------------------------------------------------------
    public Matrix ViewMatrix {
      get { return mViewMatrix; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the inverted view matrix.
    /// </summary>
    // ------------------------------------------------------------------
    public Matrix InvertedViewMatrix {
      get { return mInvertedViewMatrix; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the scaling factor of the current page.
    /// </summary>
    // ------------------------------------------------------------------
    public SizeF Magnification {
      get {
        //return mMagnification;
        return Model.CurrentPage.Magnification;
      }
      set {
        Model.CurrentPage.Magnification = value;
        UpdateViewMatrix();
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Pans the current page with the given shift.
    /// </summary>
    // ------------------------------------------------------------------
    public Point Origin {
      get {
        return Model.CurrentPage.Origin;
      }
      set {
        Model.CurrentPage.Origin = value;
        UpdateViewMatrix();
        this.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the graphics object of the surface.
    /// </summary>
    // ------------------------------------------------------------------
    public Graphics Graphics {
      get {
        if (this.parentControl != null)
          return Graphics.FromHwnd((this.parentControl as Control).Handle);
        else
          return null;
      }
    }

    internal readonly static int TrackerOffset = 5;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current cursor
    /// </summary>
    // ------------------------------------------------------------------
    public Cursor CurrentCursor {
      get { return mCurrentCursor; }
      set {
        mCurrentCursor = value;
        RaiseOnCursorChange(value);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the background brush
    /// </summary>
    // ------------------------------------------------------------------
    internal Brush BackgroundBrush {
      get {
        return mBackgroundBrush;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the model
    /// </summary>
    // ------------------------------------------------------------------
    public IModel Model {
      get {
        return mModel;
      }
      set {
        mModel = value;
        AttachToModel(mModel);
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the ghost.
    /// </summary>
    /// <value>The ghost.</value>
    // ------------------------------------------------------------------
    public IGhost Ghost {
      get { return mGhost; }
      protected set { mGhost = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the ants.
    /// </summary>
    /// <value>The ants.</value>
    // ------------------------------------------------------------------
    public IAnts Ants {
      get { return mAnts; }
      protected set { mAnts = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the tracker.
    /// </summary>
    /// <value>The tracker.</value>
    // ------------------------------------------------------------------
    public ITracker Tracker {
      get { return mTracker; }
      set { mTracker = value; }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the client bounds.
    /// </summary>
    // ------------------------------------------------------------------
    public Rectangle Bounds {
      get { return clientRectangle; }
    }

    [BrowsableAttribute(true)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
    public MeasurementsUnit RulerUnits {
      get {
        return MeasurementsUnit.Inches;
      }

    }


    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    ///<summary>
    ///Default constructor
    ///</summary>
    // ------------------------------------------------------------------
    protected ViewBase(IDiagramControl surface) {
      if (surface == null)
        throw new NullReferenceException();

      // Render and print each page in landscape.
      mLandscape = true;

      AttachToSurface(surface);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ViewBase"/> class.
    /// </summary>
    // ------------------------------------------------------------------
    protected ViewBase() { }

    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Updates the view and inverted view matrices.
    /// </summary>
    // ------------------------------------------------------------------
    private void UpdateViewMatrix() {
      mViewMatrix = GetViewMatrix();
      //set the inverted view matrix
      mInvertedViewMatrix = mViewMatrix.Clone();
      mInvertedViewMatrix.Invert();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the SizeChanged event of the parentControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void parentControl_SizeChanged(object sender, EventArgs e) {
      clientRectangle = Rectangle.Round(parentControl.ClientRectangle);
      //redefine the brush in function of the chosen background type
      SetBackgroundType(Model.CurrentPage.Ambience.BackgroundType);
      Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the diagram on the canvas. Adornments and other stuff 
    /// (ghosts, ants and so on) have to be painted by descendants.
    /// </summary>
    /// <param name="g">the graphics onto which the diagram will be 
    /// painted</param>
    // ------------------------------------------------------------------
    public virtual void Paint(Graphics g) {
      if (mIsSuspended) return;
      this.UpdateViewMatrix();
      //Rectangle rectangle = WorkArea;
      g.SetClip(WorkArea);
      //g.PageUnit = Model.MeasurementUnits;
      //g.PageScale = Model.MeasurementScale;

      //RectangleF rectangleF = ViewToWorld(DeviceToView(rectangle));
      g.Transform = mViewMatrix;
      mModel.CurrentPage.Paint(g, mShowGrid);
    }
    /*
     public virtual void Draw(Graphics grfx)
     {
         Global.ViewMatrix = GetViewMatrix();
         grfx.Clear(backgroundColor);
         PaintRulers(grfx);
         Rectangle rectangle = WorkArea;
         grfx.SetClip(rectangle);
         RectangleF rectangleF = ViewToWorld(DeviceToView(rectangle));
         grfx.Transform = Global.ViewMatrix;
         if (model != null)
         {
             grfx.PageUnit = model.MeasurementUnits;
             grfx.PageScale = model.MeasurementScale;
             model.DrawBackground(grfx);
             LayoutGrid layoutGrid = Grid;
             if (layoutGrid != null)
             {
                 layoutGrid.Draw(grfx);
             }
             if (ShowPageBounds)
             {
                 DrawPageBounds(grfx);
             }
             //draw the actual diagram
             model.Draw(grfx, rectangleF);
         }
         grfx.Transform = new Matrix();
         grfx.PageUnit = GraphicsUnit.Pixel;
         grfx.PageScale = 1.0F;
         DrawSelectionHandles(grfx);
         isDirty = false;
     }
      */

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the bounds of the current page, taking into account landscape
    /// orientation and converting inches to dpi given the graphics 
    /// resolution.
    /// </summary>
    /// <param name="g">Graphics</param>
    /// <returns>Rectangle</returns>
    // ------------------------------------------------------------------
    //protected RectangleF GetPageBounds(Graphics g)
    //{
    //    float width = (float)mModel.CurrentPage.Ambience.PageSize.Width / 1000;
    //    float height = (float)mModel.CurrentPage.Ambience.PageSize.Height / 1000;
    //    width = width * g.DpiX;
    //    height = height * g.DpiY;

    //    if (mModel.CurrentPage.Ambience.Landscape)
    //    {
    //        float w = width;
    //        width = height;
    //        height = w;
    //    }

    //    // Offset the upper-left corner by 1 inch.
    //    float x = 1F * g.DpiX;
    //    float y = 1F * g.DpiY;

    //    RectangleF bounds = new RectangleF(x, y, width, height);
    //    return bounds;
    //}

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the background.
    /// </summary>
    /// <param name="g">the graphics object onto which the background or 
    /// canvas will be painted</param>
    // ------------------------------------------------------------------
    public virtual void PaintBackground(Graphics g) {
      if (g == null) return;
      if (mShowRulers) {
        PaintRulers(g);
      }

      g.SetClip(WorkArea);

      if (mModel.CurrentPage.Ambience.BackgroundType ==
          CanvasBackgroundTypes.FlatColor) {
        g.FillRectangle(
            new SolidBrush(mModel.CurrentPage.Ambience.BackgroundColor),
            clientRectangle);
      }

      RectangleF pageBounds = mModel.CurrentPage.Bounds;
      g.Transform = mViewMatrix;
      //g.TranslateTransform(pageLocation.X, pageLocation.Y);
      RectangleF rec = pageBounds;

      // The shadow
      rec.Offset(6, 6);
      g.FillRectangle(ArtPalette.ShadowBrush, rec);

      // The page itself.
      rec.Offset(-6, -6);
      g.FillRectangle(
          new SolidBrush(mModel.CurrentPage.Ambience.PageColor),
          rec);
      g.DrawRectangle(Pens.DimGray, rec.X, rec.Y, rec.Width, rec.Height);

      if (mShowPageTitle) {
        g.DrawString(
            Model.CurrentPage.Ambience.Title,
            ArtPalette.TitleFont,
            Brushes.Silver,
            0,
            -50);
      }
      g.ResetTransform();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches event handlers to the events raised by the model.
    /// </summary>
    // ------------------------------------------------------------------
    public void AttachToModel(IModel model) {
      if (model == null)
        throw new ArgumentNullException(
            "The model assigned to the view cannot be 'null'");
      // Should I do anything to dispose the current model before 
      // attaching the new one?
      mModel = model;

      mModel.OnAmbienceChanged +=
          new EventHandler<AmbienceEventArgs>(mModel_OnAmbienceChanged);

      mModel.OnInvalidate +=
          new EventHandler(mModel_OnInvalidate);

      mModel.OnInvalidateRectangle +=
          new EventHandler<RectangleEventArgs>(
          mModel_OnInvalidateRectangle);

      mModel.OnCursorChange +=
          new EventHandler<CursorEventArgs>(mModel_OnCursorChange);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Attaches the view to the diagram control (aka surface).
    /// </summary>
    /// <param name="surface"></param>
    // ------------------------------------------------------------------
    private void AttachToSurface(IDiagramControl surface) {

      if (surface != null) {
        this.parentControl = surface;
        Graphics graphics = this.Graphics;
        dpiX = graphics.DpiX;
        dpiY = graphics.DpiX;
        graphics.Dispose();


        clientRectangle =
            Rectangle.Round(parentControl.ClientRectangle);

        this.parentControl.SizeChanged +=
            new EventHandler(parentControl_SizeChanged);
      } else
        throw new InconsistencyException("The surface control is 'null'");
    }

    void mModel_OnCursorChange(object sender, CursorEventArgs e) {
      this.RaiseOnCursorChange(e.Cursor);
    }

    void mModel_OnInvalidateRectangle(object sender, RectangleEventArgs e) {

      try {
        if (!mIsSuspended) {
          Invalidate(e.Rectangle);
          ShowTracker();//this makes sure that the tracker is always 
          // up to date, especially when the shape have 
          // changed/trasnformed because of collapse/expand of material
        }
      }
      catch (StackOverflowException) {
        //TODO: automatic exception report here?

        throw;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnInvalidate event of the Model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mModel_OnInvalidate(object sender, EventArgs e) {
      if (!mIsSuspended) {
        parentControl.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the type of the background.
    /// </summary>
    /// <param name="value">The value.</param>
    // ------------------------------------------------------------------
    public void SetBackgroundType(CanvasBackgroundTypes value) {

      if (value == CanvasBackgroundTypes.Gradient) {
        BuildGradientBrush();
      } else if (value == CanvasBackgroundTypes.FlatColor) {
        RaiseOnBackColorChange(
            Model.CurrentPage.Ambience.BackgroundColor);
      }
      //BuildSolidBrush();


    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the OnAmbienceChanged event of the mModel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see 
    /// cref="T:Netron.Diagramming.Core.AmbienceEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    void mModel_OnAmbienceChanged(object sender, AmbienceEventArgs e) {
      //redefine the brush in function of the chosen background type
      SetBackgroundType(mModel.CurrentPage.Ambience.BackgroundType);
      //refresh everything
      Invalidate();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Detaches from a model
    /// </summary>
    /// <param name="model">a diagram model</param>
    // ------------------------------------------------------------------
    public void DetachFromModel(Model model) {
      throw new System.NotImplementedException();
    }

    #region Invalidate overloads

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates this instance.
    /// </summary>
    // ------------------------------------------------------------------
    public void Invalidate() {
      if (parentControl != null) {
        UpdateViewMatrix();
        parentControl.Invalidate();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Invalidates the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    // ------------------------------------------------------------------
    public void Invalidate(Rectangle rectangle) {
      if (parentControl != null) {
        UpdateViewMatrix();
        rectangle = Rectangle.Round(WorldToView(rectangle));
        //rectangle.Offset(parentControl.AutoScrollPosition.X, parentControl.AutoScrollPosition.Y);
        //rectangle = Rectangle.Round(ViewToWorld(DeviceToView(rectangle)));
        this.parentControl.Invalidate(rectangle);
      }
    }
    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Builds the gradient brush.
    /// </summary>
    // ------------------------------------------------------------------
    private void BuildGradientBrush() {
      if ((mModel == null) || (clientRectangle == RectangleF.Empty)) {
        return;
      }

      mBackgroundBrush = new LinearGradientBrush(
          clientRectangle,
          mModel.CurrentPage.Ambience.BackgroundGradientColor1,
          mModel.CurrentPage.Ambience.BackgroundGradientColor2,
          LinearGradientMode.Vertical);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Builds the solid brush.
    /// </summary>
    // ------------------------------------------------------------------
    private void BuildSolidBrush() {
      if (mModel == null || clientRectangle == RectangleF.Empty) return;

      mBackgroundBrush = new SolidBrush(mModel.CurrentPage.Ambience.BackgroundColor);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the <see cref="OnCursorChange"/> event.
    /// </summary>
    /// <param name="cursor">The cursor.</param>
    // ------------------------------------------------------------------
    private void RaiseOnCursorChange(Cursor cursor) {
      EventHandler<CursorEventArgs> handler = OnCursorChange;
      if (handler != null)
        handler(this, new CursorEventArgs(cursor));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Raises the OnBakcColorChange event.
    /// </summary>
    /// <param name="color">The color.</param>
    // ------------------------------------------------------------------
    private void RaiseOnBackColorChange(Color color) {
      EventHandler<ColorEventArgs> handler = OnBackColorChange;
      if (handler != null)
        handler(this, new ColorEventArgs(color));
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the ants rectangle.
    /// </summary>
    /// <param name="ltPoint">The lt point.</param>
    /// <param name="rbPoint">The rb point.</param>
    // ------------------------------------------------------------------
    public virtual void PaintAntsRectangle(Point ltPoint, Point rbPoint) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the ghost rectangle.
    /// </summary>
    /// <param name="ltPoint">The lt point.</param>
    /// <param name="rbPoint">The rb point.</param>
    // ------------------------------------------------------------------
    public virtual void PaintGhostRectangle(Point ltPoint, Point rbPoint) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the ghost line.
    /// </summary>
    /// <param name="ltPoint">The lt point.</param>
    /// <param name="rbPoint">The rb point.</param>
    // ------------------------------------------------------------------
    public virtual void PaintGhostLine(Point ltPoint, Point rbPoint) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the ghost line.
    /// </summary>
    /// <param name="curveType">Type of the curve.</param>
    /// <param name="points">The points.</param>
    // ------------------------------------------------------------------
    public virtual void PaintGhostLine(MultiPointType curveType, Point[] points) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the ghost ellipse.
    /// </summary>
    /// <param name="ltPoint">The lt point.</param>
    /// <param name="rbPoint">The rb point.</param>
    // ------------------------------------------------------------------
    public virtual void PaintGhostEllipse(Point ltPoint, Point rbPoint) {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the tracker.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <param name="showHandles">if set to <c>true</c> shows the 
    /// handles.</param>
    // ------------------------------------------------------------------
    public virtual void PaintTracker(Rectangle rectangle, bool showHandles) { }

    // ------------------------------------------------------------------
    /// <summary>
    /// Resets the ghost.
    /// </summary>
    // ------------------------------------------------------------------
    public void ResetGhost() {
      if (mGhost == null) return;
      Rectangle rec = mGhost.Rectangle;
      rec.Inflate(20, 20);
      //convert it to viewspace
      //rec = Rectangle.Round(WorldToView(rec));
      this.Invalidate(rec);
      mGhost = null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Resets the ants.
    /// </summary>
    // ------------------------------------------------------------------
    public void ResetAnts() {
      mAnts = null;
    }

    #region Tracker methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Resets the tracker.
    /// </summary>
    // ------------------------------------------------------------------
    public void ResetTracker() {
      if (mTracker == null) return;
      Rectangle rec = mTracker.Rectangle;
      rec.Inflate(20, 20);
      mTracker = null;
      Invalidate(rec);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Hides the tracker.
    /// </summary>
    // ------------------------------------------------------------------
    public void HideTracker() {
      if (mTracker == null) return;
      //let's be efficient and refresh only what's necessary
      Rectangle rec = mTracker.Rectangle;
      rec.Inflate(20, 20);
      mTracker = null;
      this.Invalidate(rec);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Shows the tracker.
    /// </summary>
    // ------------------------------------------------------------------
    public void ShowTracker() {
      if (this.Model.Selection.SelectedItems != null &&
         this.Model.Selection.SelectedItems.Count > 0) {
        CollectionBase<IDiagramEntity> ents = this.Model.Selection.SelectedItems;
        bool showHandles = false;

        Rectangle rec = ents[0].Rectangle;
        foreach (IDiagramEntity entity in ents) {
          rec = Rectangle.Union(rec, entity.Rectangle);
          showHandles |= entity.Resizable;
        }
        // Make the tracker slightly bigger than the actual bounding 
        // rectangle.
        rec.Inflate(TrackerOffset, TrackerOffset);

        this.PaintTracker(rec, showHandles);

      } else //if the selection is 'null' or empty we annihilate the current tracker
        ResetTracker();
    }
    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Suspends the invalidation of the view, which means that the
    /// Invalidate() method calls from any entity will be discarded until 
    /// Resume() has been called.
    /// </summary>
    // ------------------------------------------------------------------
    public void Suspend() {
      mIsSuspended = true;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Resumes the invalidation of the view.
    /// </summary>
    // ------------------------------------------------------------------
    public void Resume() {
      mIsSuspended = false;
      parentControl.Invalidate();
    }

    #region Rulers

    // ------------------------------------------------------------------
    /// <summary>
    /// Paints the horizontal and vertical rulers.
    /// </summary>
    /// <param name="g">Graphics</param>
    // ------------------------------------------------------------------
    protected virtual void PaintRulers(Graphics g) {
      PaintMarginIntersection(g);
      HorizontalRuler horizontalRuler = HorizontalRuler;
      if (horizontalRuler != null && horizontalRuler.Visible) {
        horizontalRuler.Paint(g);
      }
      VerticalRuler verticalRuler = VerticalRuler;
      if (verticalRuler != null && verticalRuler.Visible) {
        verticalRuler.Paint(g);
      }
    }

    protected virtual void PaintMarginIntersection(Graphics g) {
      Rectangle rectangle = new Rectangle(Bounds.X, Bounds.Y, LeftMargin, TopMargin);
      g.FillRectangle(ArtPalette.RullerFillBrush, rectangle);
    }

    protected virtual HorizontalRuler CreateHorizontalRuler() {
      return new HorizontalRuler(this);
    }

    protected virtual VerticalRuler CreateVerticalRuler() {
      return new VerticalRuler(this);
    }

    [BrowsableAttribute(true)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
    public int RulerSize {
      get {
        return mRulerSize;
      }

      set {
        mRulerSize = value;
      }
    }

    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    [BrowsableAttribute(false)]
    public virtual int LeftMargin {
      get {
        if ((verticalRuler != null) &&
        (verticalRuler.Visible) &&
        (mShowRulers)) {
          return RulerSize;
        } else {
          return 0;
        }
      }
    }
    [BrowsableAttribute(false)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    public virtual int TopMargin {
      get {
        if ((horizontalRuler != null) &&
            (horizontalRuler.Visible) &&
            (mShowRulers)) {
          return RulerSize;
        } else
          return 0;
      }
    }

    [BrowsableAttribute(false)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    public Rectangle LeftMarginBounds {
      get {
        return new Rectangle(
            Bounds.X,
            Bounds.Y,
            LeftMargin,
            Bounds.Height);
      }
    }



    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    [BrowsableAttribute(false)]
    public Rectangle TopMarginBounds {
      get {
        return new Rectangle(
            Bounds.X,
            Bounds.Y,
            Bounds.Width,
            TopMargin);
      }
    }

    /// <summary>
    /// Gets the work area, i.e. the rectangle of the surface control without the rulers.
    /// </summary>
    /// <value>The work area.</value>
    [BrowsableAttribute(false)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    public virtual Rectangle WorkArea {
      get {
        Rectangle rectangle = Bounds;
        rectangle.X = rectangle.Left + LeftMargin + 1;
        rectangle.Y = rectangle.Top + TopMargin + 1;
        return rectangle;
      }
    }

    [BrowsableAttribute(false)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    public Rectangle HorizontalRulerBounds {
      get {
        int lm = LeftMargin;
        int tm = TopMargin;
        return new Rectangle(Bounds.X + lm, Bounds.Y, Bounds.Width - lm, tm);
      }
    }

    [BrowsableAttribute(false)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
    public Rectangle VerticalRulerBounds {
      get {
        int i = LeftMargin;
        int j = TopMargin;
        return new Rectangle(Bounds.X, Bounds.Y + j, i, Bounds.Height - j);
      }
    }
    [CategoryAttribute("Rulers")]
    [BrowsableAttribute(true)]
    [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
    public HorizontalRuler HorizontalRuler {
      get {
        if (horizontalRuler == null) {
          horizontalRuler = CreateHorizontalRuler();
        }
        return horizontalRuler;
      }
    }

    [DesignerSerializationVisibilityAttribute(
        DesignerSerializationVisibility.Content)]
    [BrowsableAttribute(true)]
    [CategoryAttribute("Rulers")]
    public VerticalRuler VerticalRuler {
      get {
        if (verticalRuler == null) {
          verticalRuler = CreateVerticalRuler();
        }
        return verticalRuler;
      }
    }
    #endregion

    #region World-View-Device Conversions

    #region View to device
    public Point ViewToDevice(PointF ptView) {
      float f1 = 1.0F;
      float f2 = 1.0F;
      float f3 = 1.0F;
      float f4 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f1 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f3 = dpiX / f1;
          f4 = dpiX / f1;
        }
        f2 = mModel.MeasurementScale;
      }
      int i = (int)Math.Round((double)(ptView.X * f2 * f3));
      int j = (int)Math.Round((double)(ptView.Y * f2 * f4));
      return new Point(i, j);
    }

    public Size ViewToDevice(SizeF szView) {
      float f1 = 1.0F;
      float f2 = 1.0F;
      float f3 = 1.0F;
      float f4 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f1 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f3 = dpiX / f1;
          f4 = dpiX / f1;
        }
        f2 = mModel.MeasurementScale;
      }
      int i = (int)(szView.Width * f2 * f3);
      int j = (int)(szView.Height * f2 * f4);
      return new Size(i, j);
    }

    public PointF ViewToDeviceF(PointF ptView) {
      float f3 = 1.0F;
      float f4 = 1.0F;
      float f5 = 1.0F;
      float f6 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f3 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f5 = dpiX / f3;
          f6 = dpiX / f3;
        }
        f4 = mModel.MeasurementScale;
      }
      float f1 = ptView.X * f4 * f5;
      float f2 = ptView.Y * f4 * f6;
      return new PointF(f1, f2);
    }

    public Rectangle ViewToDevice(RectangleF rcView) {
      PointF pointF1 = new PointF(rcView.Left, rcView.Top);
      PointF pointF2 = new PointF(rcView.Right, rcView.Bottom);
      Point point1 = ViewToDevice(pointF1);
      Point point2 = ViewToDevice(pointF2);
      return new Rectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y);
    }

    public void ViewToDevice(PointF[] viewPts, out Point[] devicePts) {
      devicePts = new Point[(int)viewPts.Length];
      for (int i = 0; i < (int)viewPts.Length; i++) {
        devicePts[i] = ViewToDevice(viewPts[i]);
      }
    }

    public SizeF ViewToDeviceF(SizeF szView) {
      float f3 = 1.0F;
      float f4 = 1.0F;
      float f5 = 1.0F;
      float f6 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f3 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f5 = dpiX / f3;
          f6 = dpiX / f3;
        }
        f4 = mModel.MeasurementScale;
      }
      float f1 = szView.Width * f4 * f5;
      float f2 = szView.Height * f4 * f6;
      return new SizeF(f1, f2);
    }

    public RectangleF ViewToDeviceF(RectangleF rcView) {
      PointF pointF1 = new PointF(rcView.Left, rcView.Top);
      PointF pointF2 = new PointF(rcView.Right, rcView.Bottom);
      PointF pointF3 = ViewToDeviceF(pointF1);
      PointF pointF4 = ViewToDeviceF(pointF2);
      return new RectangleF(pointF3.X, pointF3.Y, pointF4.X - pointF3.X, pointF4.Y - pointF3.Y);
    }
    #endregion

    #region World to view

    public RectangleF WorldToView(RectangleF rcWorld) {
      PointF[] pointFs = new PointF[2];
      pointFs[0].X = rcWorld.Left;
      pointFs[0].Y = rcWorld.Top;
      pointFs[1].X = rcWorld.Right;
      pointFs[1].Y = rcWorld.Bottom;
      mViewMatrix.TransformPoints(pointFs);
      return new RectangleF(pointFs[0].X, pointFs[0].Y, pointFs[1].X - pointFs[0].X, pointFs[1].Y - pointFs[0].Y);
    }

    public PointF WorldToView(PointF ptWorld) {
      PointF[] pointFs1 = new PointF[] { ptWorld };
      mViewMatrix.TransformPoints(pointFs1);
      return pointFs1[0];
    }

    public void WorldToView(PointF[] worldPts, out PointF[] viewPts) {
      viewPts = (PointF[])worldPts.Clone();
      mViewMatrix.TransformPoints(viewPts);
    }

    public SizeF WorldToView(SizeF szWorld) {
      PointF[] pointFs1 = new PointF[] { new PointF(szWorld.Width, szWorld.Height) };
      Matrix matrix = new Matrix();
      matrix.Scale(Magnification.Width / 100.0F, Magnification.Height / 100.0F);
      matrix.TransformPoints(pointFs1);
      return new SizeF(pointFs1[0].X, pointFs1[0].Y);
    }

    #endregion

    #region View to world

    /// <summary>
    /// Converts the given point from View coordinates to the absolute World coordinates.
    /// </summary>
    /// <param name="ptView">The pt view.</param>
    /// <returns></returns>
    public PointF ViewToWorld(PointF ptView) {
      PointF[] pointFs1 = new PointF[] { ptView };
      //Matrix matrix = mInvertedViewMatrix;
      //the view transformation has to be inverted since the transform is from world to view coordinates
      //matrix.Invert();
      mInvertedViewMatrix.TransformPoints(pointFs1);
      return pointFs1[0];
    }

    /// <summary>
    /// Converts the given rectangle from View coordinates to the absolute World coordinates.
    /// </summary>
    /// <param name="rcView">The rc view.</param>
    /// <returns></returns>
    public RectangleF ViewToWorld(RectangleF rcView) {
      PointF[] pointFs = new PointF[2];
      pointFs[0].X = rcView.Left;
      pointFs[0].Y = rcView.Top;
      pointFs[1].X = rcView.Right;
      pointFs[1].Y = rcView.Bottom;
      //Matrix matrix = GetViewMatrix();
      //matrix.Invert();
      mInvertedViewMatrix.TransformPoints(pointFs);
      return new RectangleF(pointFs[0].X, pointFs[0].Y, pointFs[1].X - pointFs[0].X, pointFs[1].Y - pointFs[0].Y);
    }

    /// <summary>
    /// Converts the given rectangle from View coordinates to the absolute World coordinates.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns></returns>
    public Rectangle ViewToWorld(Rectangle rectangle) {
      Point[] pts = new Point[2];
      pts[0].X = rectangle.Left;
      pts[0].Y = rectangle.Top;
      pts[1].X = rectangle.Right;
      pts[1].Y = rectangle.Bottom;
      //Matrix matrix = GetViewMatrix();
      //matrix.Invert();
      mInvertedViewMatrix.TransformPoints(pts);
      return new Rectangle(pts[0].X, pts[0].Y, pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
    }

    /// <summary>
    /// Converts the given size from View coordinates to the absolute World coordinates.
    /// </summary>
    /// <param name="szView">The sz view.</param>
    /// <returns></returns>
    public SizeF ViewToWorld(SizeF szView) {
      PointF[] pointFs1 = new PointF[] { new PointF(szView.Width, szView.Height) };
      Matrix matrix = new Matrix();
      matrix.Scale(Magnification.Width / 100.0F, Magnification.Height / 100.0F);
      matrix.Invert();
      matrix.TransformPoints(pointFs1);
      return new SizeF(pointFs1[0].X, pointFs1[0].Y);
    }

    /// <summary>
    /// Converts the given point array from View coordinates to the absolute World coordinates.
    /// </summary>
    /// <param name="viewPts">The view PTS.</param>
    /// <param name="worldPts">The world PTS.</param>
    public void ViewToWorld(PointF[] viewPts, out PointF[] worldPts) {
      worldPts = (PointF[])viewPts.Clone();
      //Matrix matrix = GetViewMatrix();
      //matrix.Invert();
      mInvertedViewMatrix.TransformPoints(worldPts);
    }

    #endregion

    #region Device to view

    public PointF DeviceToView(Point ptDevice) {
      float f3 = 1.0F;
      float f4 = 1.0F;
      float f5 = 1.0F;
      float f6 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f3 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f5 = dpiX / f3;
          f6 = dpiY / f3;
        }
        f4 = mModel.MeasurementScale;
      }
      float f1 = (float)ptDevice.X / (f4 * f5);
      float f2 = (float)ptDevice.Y / (f4 * f6);
      return new PointF(f1, f2);
    }

    public void DeviceToView(Point[] devicePts, out PointF[] viewPts) {
      viewPts = new PointF[(int)devicePts.Length];
      for (int i = 0; i < (int)devicePts.Length; i++) {
        viewPts[i] = DeviceToView(devicePts[i]);
      }
    }

    public RectangleF DeviceToView(Rectangle rcDevice) {
      Point point1 = new Point(rcDevice.Left, rcDevice.Top);
      Point point2 = new Point(rcDevice.Right, rcDevice.Bottom);
      PointF pointF1 = DeviceToView(point1);
      PointF pointF2 = DeviceToView(point2);
      return new RectangleF(pointF1.X, pointF1.Y, pointF2.X - pointF1.X, pointF2.Y - pointF1.Y);
    }

    public SizeF DeviceToView(Size szDevice) {
      float f3 = 1.0F;
      float f4 = 1.0F;
      float f5 = 1.0F;
      float f6 = 1.0F;
      if (mModel != null) {
        if (mModel.MeasurementUnits != GraphicsUnit.Pixel) {
          f3 = Measurements.UnitsPerInch(mModel.MeasurementUnits);
          f5 = dpiX / f3;
          f6 = dpiY / f3;
        }
        f4 = mModel.MeasurementScale;
      }
      float f1 = (float)szDevice.Width / (f4 * f5);
      float f2 = (float)szDevice.Height / (f4 * f6);
      return new SizeF(f1, f2);
    }

    #endregion

    #endregion

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns the matrix corresponding to the translation and scaling 
    /// of the canvas.
    /// </summary>
    /// <returns>Matrix</returns>
    // ------------------------------------------------------------------
    public Matrix GetViewMatrix() {
      Matrix matrix = new Matrix();

      matrix.Scale(
          Magnification.Width / 100.0F,
          Magnification.Height / 100.0F);

      matrix.Translate(-Origin.X, -Origin.Y);

      Size size = new Size(LeftMargin, TopMargin);
      SizeF sizeF = ViewToWorld(DeviceToView(size));
      matrix.Translate(sizeF.Width, sizeF.Height);
      return matrix;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Sets the magnification and origin of the diagram such that
    /// all entities in the current page are in view.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual void ZoomFit() {
      Bundle bundle = new Bundle(this.Model.CurrentPage.Entities);
      Rectangle bundleBounds = bundle.Rectangle;
      bundleBounds.Inflate(20, 20);
      bundleBounds.Offset(-10, -10);
      this.ZoomArea(bundleBounds);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Zooms the diagram to the area specified.
    /// </summary>
    /// <param name="areaInWorldCoords">Rectangle: The area to zoom in world 
    /// coordinates.</param>
    // ------------------------------------------------------------------
    public virtual void ZoomArea(Rectangle areaInWorldCoords) {
      Rectangle areaToZoom = Rectangle.Round(WorldToView(areaInWorldCoords));
      areaToZoom = areaInWorldCoords;

      float zoom = Math.Min(
          ((float)WorkArea.Width / (float)areaToZoom.Width) * 100.0F,
           ((float)WorkArea.Height / (float)areaToZoom.Height) * 100.0F);

      if (zoom == 0) {
        zoom = 1F;
      }

      Magnification = new SizeF(
          zoom,
          zoom);
      Origin = areaInWorldCoords.Location;
    }

    #endregion

    #region Standard IDispose implementation
    /// <summary>
    /// Disposes the view
    /// </summary>
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);


    }
    /// <summary>
    /// Disposes this instance.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing) {
      if (disposing) {
        #region free managed resources
        if (mBackgroundBrush != null) {
          mBackgroundBrush.Dispose();
          mBackgroundBrush = null;
        }
        #endregion
      }

    }

    #endregion

  }
}