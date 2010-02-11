using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  [ToolboxItem(true)]
  public partial class NetronVisualization : DiagramControlBase {
    public NetronVisualization()
      : base() {
      InitializeComponent();

      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.ResizeRedraw, true);

      if (!DesignMode) {
        this.Controller = new Controller(this);
        this.View = new View(this);

        this.Document = new Document();
        this.AttachToDocument(Document);
        this.Controller.View = View;
        TextEditor.Init(this);

        View.OnCursorChange += new EventHandler<CursorEventArgs>(mView_OnCursorChange);
        View.OnBackColorChange += new EventHandler<ColorEventArgs>(View_OnBackColorChange);

        this.SizeChanged += new EventHandler(AlgorithmVisualization_SizeChanged);
        this.AllowDrop = true;
      }
    }

    void AlgorithmVisualization_SizeChanged(object sender, EventArgs e) {
      Size newSize = new Size((int) (this.Size.Width * this.View.Magnification.Width),
        (int) (this.Size.Height * this.View.Magnification.Height));
      this.View.PageSize = newSize;
    }

    protected override void OnScroll(ScrollEventArgs se) {
      //base.OnScroll(se);
      if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
        Origin = new Point(se.NewValue, Origin.Y);
        //System.Diagnostics.Trace.WriteLine(se.NewValue);
      } else {
        Origin = new Point(Origin.X, se.NewValue);
        //System.Diagnostics.Trace.WriteLine(se.NewValue);
      } 
    }

    private void mView_OnCursorChange(object sender, CursorEventArgs e) {
      this.Cursor = e.Cursor;
    }
  }
}
