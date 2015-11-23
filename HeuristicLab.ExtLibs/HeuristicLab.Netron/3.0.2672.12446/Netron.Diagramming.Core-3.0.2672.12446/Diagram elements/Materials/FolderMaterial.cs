using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Netron.Diagramming.Core {
  /// <summary>
  /// This material holds other shape material entries which can be 
  /// collapsed or expanded like a tree-node or folder.
  /// </summary>
  public partial class FolderMaterial :
      ShapeMaterialBase,
      IMouseListener,
      IHoverListener {
    #region Constants
    /// <summary>
    /// the height of the entries
    /// </summary>
    public const int ItemHeight = 16;
    /// <summary>
    /// the height of the folder header
    /// </summary>
    public const int HeaderHeight = 16;
    /// <summary>
    /// the vertical spacing between entries
    /// </summary>
    public const int ItemSpacing = 1;
    /// <summary>
    /// the horizontal shift between the plusminus icon and the header
    /// </summary>
    public const int HeaderSpacing = 4;
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the state of this folder has changed; on collasping or expanding the entries this folder contains.
    /// </summary>
    public event EventHandler<RectangleEventArgs> OnFolderChanged;
    #endregion

    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// FolderMaterial.
    /// </summary>
    // ------------------------------------------------------------------
    protected double folderMaterialVersion = 1.0;

    /// <summary>
    /// the Text field
    /// </summary>
    protected string mText;

    /// <summary>
    /// volatile pointer to a hovered material
    /// </summary>
    protected IHoverListener currentHoveredMaterial = null;

    /// <summary>
    /// the width of the individual items
    /// </summary>
    protected int itemWidth = 100;

    /// <summary>
    /// the collection of material entries
    /// </summary>
    protected CollectionBase<IShapeMaterial> mEntries;

    /// <summary>
    /// the plus/minus icon to collapse expand the items
    /// </summary>
    protected SwitchIconMaterial plusminus;

    /// <summary>
    /// the header of the folder
    /// </summary>
    protected ClickableLabelMaterial header;

    /// <summary>
    /// whether the folder is collapsed
    /// </summary>
    protected bool mCollapsed = true;

    /// <summary>
    /// the ShowLines field
    /// </summary>
    protected bool mShowLines = true;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public override double Version {
      get {
        return folderMaterialVersion;
      }
    }

    /// <summary>
    /// Gets or sets the text style for all nodes.
    /// </summary>
    public override ITextStyle TextStyle {
      get {
        return base.TextStyle;
      }
      set {
        base.TextStyle = value;
        header.TextStyle = myTextStyle;
        myTextStyle.TextStyleChanged +=
            new TextStyleChangedEventHandler(
            HandleTextStyleChanged);

        foreach (IShapeMaterial matl in Entries) {
          matl.TextStyle = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets whether the connecting line between nodes is desplayed.
    /// </summary>
    public bool ShowLines {
      get { return mShowLines; }
      set { mShowLines = value; }
    }
    /// <summary>
    /// Gets or sets the Shape
    /// </summary>
    /// <value></value>
    public override IShape Shape {
      get {
        return base.Shape;
      }
      set {
        base.Shape = value;
        plusminus.Shape = value;
        header.Shape = value;
        //have to go one level deeper than the normal material here
        foreach (IShapeMaterial material in mEntries) {
          material.Shape = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the text or name of the folder
    /// </summary>
    public string Text {
      get { return header.Text; }
      set { header.Text = value; }
    }
    /// <summary>
    /// Gets the entries.
    /// </summary>
    /// <value>The entries.</value>
    [Editor(typeof(ShapeMaterialCollectionEditor<IconLabelMaterial>), typeof(UITypeEditor))]
    public CollectionBase<IShapeMaterial> Entries {
      get {
        return mEntries;
      }
      internal set { mEntries = value; }
    }

    /// <summary>
    /// Gets if the root node is collapsed.
    /// </summary>
    public bool IsCollapsed {
      get {
        return mCollapsed;
      }
      set {
        mCollapsed = value;
        plusminus.Collapsed = value;
      }
    }
    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor
    ///</summary>
    public FolderMaterial(string title)
      : base() {
      myTextStyle = new TextStyle(
          Color.Black,
          new Font("Arial", 10),
          StringAlignment.Near,
          StringAlignment.Near);
      mEntries = new CollectionBase<IShapeMaterial>();
      mEntries.OnItemAdded +=
          new EventHandler<CollectionEventArgs<IShapeMaterial>>(
          mEntries_OnItemAdded);

      Gliding = false;
      Resizable = true;

      plusminus = new SwitchIconMaterial(SwitchIconType.PlusMinus);
      plusminus.TextStyle = myTextStyle;
      plusminus.Transform(new Rectangle(0, 0, 16, 16));
      plusminus.Visible = true;
      plusminus.Gliding = false;
      plusminus.OnExpand += new EventHandler(plusminus_OnExpand);
      plusminus.OnCollapse += new EventHandler(plusminus_OnCollapse);


      header = new ClickableLabelMaterial();
      header.TextStyle = myTextStyle;
      header.Transform(new Rectangle(0, 0, 100, HeaderHeight));
      header.Text = title;
      header.Gliding = false;
      header.Visible = true;
      header.Resizable = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:FolderMaterial"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="entries">The entries.</param>
    public FolderMaterial(string title, string[] entries)
      : this(title) {
      myTextStyle = new TextStyle(
          Color.Black,
          new Font("Arial", 10),
          StringAlignment.Near,
          StringAlignment.Near);
      IconLabelMaterial label;
      for (int k = 0; k < entries.Length; k++) {
        label = new IconLabelMaterial("Resources.Text.png", entries[k]);

        // We attach our ITextStyle to items when they're added to the
        // collection.
        mEntries.Add(label);
      }
      Collapse();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:FolderMaterial"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="entries">The entries.</param>
    /// <param name="defaultIconLocation">The default icon location.</param>
    public FolderMaterial(
        string title,
        string[] entries,
        string defaultIconLocation)
      : this(title) {
      IconLabelMaterial label;
      myTextStyle = new TextStyle(
           Color.Black,
           new Font("Arial", 10),
           StringAlignment.Near,
           StringAlignment.Near);

      for (int k = 0; k < entries.Length; k++) {
        label = new IconLabelMaterial(entries[k], defaultIconLocation);
        mEntries.Add(label);
      }
      Collapse();

      myTextStyle.TextStyleChanged +=
          new TextStyleChangedEventHandler(HandleTextStyleChanged);
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when our TextStyle has changed, each item in our mEntries
    /// is updated.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">TextStyleChangedEventArgs</param>
    // ------------------------------------------------------------------
    protected virtual void HandleTextStyleChanged(
        object sender,
        TextStyleChangedEventArgs e) {
      header.TextStyle = myTextStyle;
      foreach (IShapeMaterial item in mEntries) {
        item.TextStyle = myTextStyle;
      }
    }


    #endregion

    #region Methods
    /// <summary>
    /// Recalculates and initializes things when an entry has been added.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    protected virtual void mEntries_OnItemAdded(
        object sender,
        CollectionEventArgs<IShapeMaterial> e) {
      e.Item.Shape = this.Shape;
      e.Item.TextStyle = myTextStyle;
      Transform(this.Rectangle);//simply recalculate with the same base rectangle
      //notify the hierarchy
      RaiseOnFolderChanged(new RectangleEventArgs(Rectangle));
      e.Item.Visible = !mCollapsed;

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Calculates the size needed to fit all items for the specified
    /// GDI+ drawing surface.
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    public override Size CalculateMinSize(Graphics g) {
      // When collapsed, the only thing drawn is the header (root node).
      Size minSizeNeeded = header.CalculateMinSize(g);
      Size plusMinusSize;
      Size materialSize;
      int linesLength = 0;  // The lenght of the lines that are drawn
      // connecting the folders.

      int padding = 20;  // Accounts for the spacing around materials.

      if (mShowLines) {
        // 7 for the lines themselves, another 7 for the space between
        // the line and the material.
        linesLength = 14;
      }

      if (minSizeNeeded == Size.Empty) {
        minSizeNeeded = new Size(1, 1);
      } else {
        // Account for the padding around the header.
        minSizeNeeded.Height += (padding / 2);
        minSizeNeeded.Width += padding;
      }

      // The plus-minus icon is placed to the left of the header text,
      // so it's width gets added to the current min size.
      if (plusminus != null) {
        plusMinusSize = plusminus.CalculateMinSize(g);

        if (plusMinusSize != Size.Empty) {
          plusMinusSize.Width += linesLength;
          plusMinusSize.Width += padding;

          minSizeNeeded.Width += plusMinusSize.Width;

          if (plusMinusSize.Height > minSizeNeeded.Height) {
            minSizeNeeded.Height = plusMinusSize.Height;
          }
        }
      }

      foreach (IShapeMaterial matl in Entries) {
        if (matl.Visible) {
          materialSize = matl.CalculateMinSize(g);
          if (materialSize != Size.Empty) {
            materialSize.Width += linesLength;
            materialSize.Width += padding;
            if (materialSize.Width > minSizeNeeded.Width) {
              minSizeNeeded.Width = materialSize.Width;
            }
            minSizeNeeded.Height += materialSize.Height + 3;
          }
        }
      }

      return minSizeNeeded;
    }

    /// <summary>
    /// Handles the OnExpand event of the plusminus control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    protected virtual void plusminus_OnExpand(
        object sender,
        EventArgs e) {
      Expand();
    }
    /// <summary>
    /// Handles the OnCollapse event of the plusminus control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    protected virtual void plusminus_OnCollapse(
        object sender,
        EventArgs e) {
      Collapse();
    }
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
    /// </returns>
    public override object GetService(Type serviceType) {
      if (serviceType.Equals(typeof(IMouseListener)))
        return this;
      else if (serviceType.Equals(typeof(IHoverListener)))
        return this;
      else
        return null;
    }
    /// <summary>
    /// Expands the items.
    /// </summary>
    public virtual void Expand() {
      mCollapsed = false;
      foreach (IShapeMaterial material in mEntries) {
        material.Visible = true;
      }

      Transform(Rectangle);
      RaiseOnFolderChanged(new RectangleEventArgs(Rectangle));
    }
    /// <summary>
    /// Collapses the items.
    /// </summary>
    public virtual void Collapse() {
      mCollapsed = true;

      foreach (IShapeMaterial material in mEntries) {
        material.Visible = false;
      }

      Transform(Rectangle);

      RaiseOnFolderChanged(new RectangleEventArgs(Rectangle));
    }
    /// <summary>
    /// Transforms the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle of the COLLAPSED FOLDER!
    /// </param>
    public override void Transform(Rectangle rectangle) {
      //IMPORTANT: the rectangle given as the transformation parameter 
      // is supposed to be the rectangle of the collapsed folder!

      itemWidth = rectangle.Width - plusminus.Rectangle.Width - HeaderSpacing;
      //shift the plusminus icon
      plusminus.Transform(new Rectangle(rectangle.Location, plusminus.Rectangle.Size));
      //header
      header.Transform(new Rectangle(rectangle.Location.X + plusminus.Rectangle.Width + 10, rectangle.Location.Y, itemWidth, header.Rectangle.Height));
      //transform the entries
      int k = 0;
      foreach (IShapeMaterial material in mEntries) {
        material.Transform(new Rectangle(rectangle.X + plusminus.Rectangle.Width + 10, rectangle.Y + HeaderHeight + 7 + k * (ItemHeight + ItemSpacing), itemWidth, ItemHeight));
        k++;
      }

      if (mCollapsed)
        base.Transform(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, HeaderHeight));
      else
        base.Transform(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, HeaderHeight + ItemSpacing + mEntries.Count * (ItemSpacing + ItemHeight)));
    }
    /// <summary>
    /// Raises the OnFolderChanged event.
    /// </summary>
    /// <param name="e">The <see cref="T:Netron.Diagramming.Core.RectangleEventArgs"/> instance containing the event data.</param>
    protected virtual void RaiseOnFolderChanged(RectangleEventArgs e) {
      EventHandler<RectangleEventArgs> handler = OnFolderChanged;
      if (handler != null) {
        handler(this, e);
      }
    }
    /// <summary>
    /// Paints the entity using the given graphics object
    /// </summary>
    /// <param name="g"></param>
    public override void Paint(System.Drawing.Graphics g) {
      if (!Visible) return;

      //g.DrawRectangle(Pens.OrangeRed, Rectangle);
      GraphicsContainer cto = g.BeginContainer();
      g.SetClip(Shape.Rectangle);
      plusminus.Paint(g);
      header.Paint(g);
      if (!mCollapsed && mShowLines) {
        g.DrawLine(
            ArtPalette.FolderLinesPen,
            Rectangle.X + 7,
            plusminus.Rectangle.Bottom,
            Rectangle.X + 7,
            plusminus.Rectangle.Bottom + mEntries.Count * (ItemHeight + ItemSpacing));
      }
      int k = 1;
      foreach (IShapeMaterial material in mEntries) {
        material.Paint(g);
        if (!mCollapsed && mShowLines) {
          g.DrawLine(
              ArtPalette.FolderLinesPen,
              Rectangle.X + 7,
              plusminus.Rectangle.Bottom + k * (ItemHeight + ItemSpacing),
              Rectangle.X + 16,
              plusminus.Rectangle.Bottom + k * (ItemHeight + ItemSpacing));
          k++;
        }
      }
      g.EndContainer(cto);
    }
    /// <summary>
    /// Handles the mouse-down event
    /// </summary>
    /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
    public virtual bool MouseDown(MouseEventArgs e) {

      if (header.Rectangle.Contains(e.Location)) {
        header.MouseDown(e);
        return true;
      }
      if (plusminus.Rectangle.Contains(e.Location)) {
        plusminus.MouseDown(e);
        return true;
      }

      if (mCollapsed) return false;
      IMouseListener listener;
      foreach (IShapeMaterial material in Entries) {
        if (material.Rectangle.Contains(e.Location)) {
          listener = material.GetService(typeof(IMouseListener)) as IMouseListener;
          if (listener != null)
            if (listener.MouseDown(e))
              return true;
        }

      }
      return false;
    }
    /// <summary>
    /// Handles the mouse-move event
    /// </summary>
    /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
    public virtual void MouseMove(MouseEventArgs e) {
      //System.Diagnostics.Trace.WriteLine(e.Location.ToString());
    }
    /// <summary>
    /// Handles the mouse-up event
    /// </summary>
    /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
    public virtual void MouseUp(MouseEventArgs e) {

    }
    #region IHoverListener Members
    /// <summary>
    /// <see cref="IHoverListener"/> implementation
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public virtual void MouseHover(MouseEventArgs e) {
      if (header.Rectangle.Contains(e.Location)) {
        HoverEntity(e, header);
        return;
      }
      if (plusminus.Rectangle.Contains(e.Location)) {
        HoverEntity(e, plusminus);
        return;
      }

      if (!mCollapsed) //no need to check the rest if the folder is collapsed!
            {
        IHoverListener listener;

        foreach (IShapeMaterial material in this.Entries) {
          if (material.Rectangle.Contains(e.Location)) //we caught an material
                    {
            listener = HoverEntity(e, material);
            return; //only one material at a time
          }

        }
      }
      if (currentHoveredMaterial != null) {
        currentHoveredMaterial.MouseLeave(e);
        currentHoveredMaterial = null;
      }

    }
    protected virtual IHoverListener HoverEntity(
        MouseEventArgs e,
        IShapeMaterial material) {
      IHoverListener listener;
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
      return listener;
    }

    /// <summary>
    /// <see cref="IHoverListener"/> implementation
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public virtual void MouseEnter(MouseEventArgs e) {

    }
    /// <summary>
    /// <see cref="IHoverListener"/> implementation
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public virtual void MouseLeave(MouseEventArgs e) {
      currentHoveredMaterial = null;
      this.Shape.Model.RaiseOnCursorChange(Cursors.Default);//HACK: should be the cursor before entering into another one
    }

    #endregion
    #endregion

  }


}
