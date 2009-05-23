using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  class TooltipListener:IMouseEventListener {
    public delegate void ShowToolTipHandler(Point location);

    public event ShowToolTipHandler ShowToolTip;
    
    private readonly Timer timer;
    private Point lastMouseLocation;


    public TooltipListener() {
      timer= new Timer {Interval = 300};
      timer.Tick += Tick;
      timer.Start();
    }

    private void Tick(object sender, EventArgs e) {
      if(ShowToolTip != null) {
        ShowToolTip(lastMouseLocation);
      }
    }

    #region Implementation of IMouseEventListener

    public void MouseMove(object sender, MouseEventArgs e) {
      if(e.Location==lastMouseLocation)
        return;
      timer.Stop();
      timer.Start();
      lastMouseLocation = e.Location;
    }

    public void MouseUp(object sender, MouseEventArgs e) {
      //Not needed, so do Nothing
    }

    #endregion
  }
}
