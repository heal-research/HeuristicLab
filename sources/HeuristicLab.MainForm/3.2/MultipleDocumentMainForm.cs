using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm {
  public partial class MultipleDocumentMainForm : MainFormBase {
    public MultipleDocumentMainForm(Type userInterfaceType)
      : base(userInterfaceType) {
      InitializeComponent();
      this.IsMdiContainer = true;
    }

    public override void ShowView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)ShowView, view);
      else {
        base.ShowView(view);
        MultipleDocumentForm form = new MultipleDocumentForm(view);
        form.Activated += new EventHandler(MultipleDocumentFormActivated);
        form.FormClosing += new FormClosingEventHandler(MultipleDocumentFormClosing);
        form.MdiParent = this;
        foreach (IToolStripItem item in viewStateChangeToolStripItems)
          view.StateChanged += new EventHandler(item.ViewStateChanged);
        form.Show();
      }
    }

    private void MultipleDocumentFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((MultipleDocumentForm)sender).View;
      base.StatusStripText = ((MultipleDocumentForm)sender).View.Caption;
    }

    private void MultipleDocumentFormClosing(object sender, FormClosingEventArgs e) {
      MultipleDocumentForm form = (MultipleDocumentForm)sender;
      openViews.Remove(form.View);
      if (openViews.Count == 0)
        ActiveView = null;
      form.Activated -= new EventHandler(MultipleDocumentFormActivated);
      form.FormClosing -= new FormClosingEventHandler(MultipleDocumentFormClosing);
      foreach (IToolStripItem item in viewStateChangeToolStripItems)
        form.View.StateChanged -= new EventHandler(item.ViewStateChanged);
    }
  }
}
