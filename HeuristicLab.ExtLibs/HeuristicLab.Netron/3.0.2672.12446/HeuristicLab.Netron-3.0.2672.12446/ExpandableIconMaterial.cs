using System;
using System.Drawing;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public class ExpandableIconMaterial : IconMaterial, IMouseListener {
    private Bitmap collapsedBitmap;
    private Bitmap expandedBitmap;

    public ExpandableIconMaterial(Bitmap collapsedBitmap, Bitmap expandedBitmap)
      : base() {
      this.collapsedBitmap = collapsedBitmap;
      this.expandedBitmap = expandedBitmap;

      this.Gliding = false;
      this.Collapsed = true;
    }

    public event EventHandler OnCollapse;
    public event EventHandler OnExpand;

    private bool collapsed;
    public bool Collapsed {
      get { return this.collapsed; }
      set {
        if (value != this.collapsed) {
          if (value) {
            this.Icon = this.collapsedBitmap;
          } else
            this.Icon = this.expandedBitmap;
          collapsed = value;
          RaiseOnExpand();
        }
      }
    }

    private void RaiseOnExpand() {
      if (OnExpand != null)
        OnExpand(this, EventArgs.Empty);
    }
    private void RaiseOnCollapse() {
      if (OnCollapse != null)
        OnCollapse(this, EventArgs.Empty);
    }


    #region IMouseListener Members
    public override object GetService(Type serviceType) {
      if (serviceType.Equals(typeof(IMouseListener)))
        return this;
      return null;
    }

    public bool MouseDown(MouseEventArgs e) {
      if (e.Clicks == 1) {
        Collapsed = !Collapsed;
        return true;
      }
      return false;
    }

    public void MouseMove(System.Windows.Forms.MouseEventArgs e) {
    }

    public void MouseUp(System.Windows.Forms.MouseEventArgs e) {
    }
    #endregion
  }
}
