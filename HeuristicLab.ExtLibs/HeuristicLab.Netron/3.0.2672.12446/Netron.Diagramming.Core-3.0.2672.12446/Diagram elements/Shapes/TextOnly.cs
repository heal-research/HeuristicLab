using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Like a <see cref="TextLabel"/> but without shadows and colors.
    /// </summary>
    public partial class TextOnly : SimpleShapeBase
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// Implementation of IVersion - the current version of 
        /// TextLabel.
        /// </summary>
        // ------------------------------------------------------------------
        protected const double textOnlyVersion = 1.0;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the current version.
        /// </summary>
        // ------------------------------------------------------------------
        public override double Version
        {
            get
            {
                return textOnlyVersion;
            }
        }

        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "TextOnly Shape"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s"></param>
        public TextOnly(IModel s)
            : base(s)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextOnly"/> class.
        /// </summary>
        public TextOnly()
            : base()
        { }

        #endregion

        #region Methods

        // -----------------------------------------------------------------
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        // -----------------------------------------------------------------
        protected override void Initialize()
        {
            base.Initialize();
            this.PaintStyle = ArtPalette.GetTransparentPaintStyle();
        }

        /// <summary>
        /// Tests whether the mouse hits this shape.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            Rectangle r = new Rectangle(p, new Size(5, 5));
            return Rectangle.Contains(r);
        }

        /// <summary>
        /// Paints the bundle on the canvas
        /// </summary>
        /// <param name="g">a Graphics object onto which to paint</param>
        public override void Paint(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("The Graphics object is 'null'.");

            g.SmoothingMode = SmoothingMode.HighQuality;

            //the edge
            if (Hovered)
                g.DrawRectangle(new Pen(Color.Red, 2F), Rectangle);
            //the text		
            if (!string.IsNullOrEmpty(Text))
            {
                //StringFormat format = new StringFormat();
                //format.Alignment = this.VerticalAlignment;
                //format.LineAlignment = this.HorizontalAlignment;

                //g.DrawString(
                //    Text,
                //    ArtPalette.DefaultFont,
                //    Brushes.Black,
                //    Rectangle,
                //    format);
                g.DrawString(
                    mTextStyle.GetFormattedText(Text),
                    mTextStyle.Font,
                    mTextStyle.GetBrush(),
                    TextArea,
                    mTextStyle.StringFormat);
            }
        }







        #endregion

    }
}
