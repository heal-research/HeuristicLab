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
        form.FormClosing += new FormClosingEventHandler(view.FormClosing);
        form.FormClosed += new FormClosedEventHandler(MultipleDocumentFormClosed);
        form.MdiParent = this;
        foreach (IToolStripItem item in ViewChangedToolStripItems)
          view.StateChanged += new EventHandler(item.ViewChanged);
        form.Show();
      }
    }

    private void MultipleDocumentFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((MultipleDocumentForm)sender).View;
      base.StatusStripText = ((MultipleDocumentForm)sender).View.Caption;
    }

    private void MultipleDocumentFormClosed(object sender, FormClosedEventArgs e) {
      MultipleDocumentForm form = (MultipleDocumentForm)sender;
      views.Remove(form.View);
      if (views.Count == 0)
        ActiveView = null;
      form.Activated -= new EventHandler(MultipleDocumentFormActivated);
      form.FormClosing -= new FormClosingEventHandler(form.View.FormClosing);
      form.FormClosed -= new FormClosedEventHandler(MultipleDocumentFormClosed);
      foreach (IToolStripItem item in ViewChangedToolStripItems)
        form.View.StateChanged -= new EventHandler(item.ViewChanged);
    }
  }
}
