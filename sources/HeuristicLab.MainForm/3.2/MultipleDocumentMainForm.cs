#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
    public MultipleDocumentMainForm()
      : base() {
      InitializeComponent();
    }

    public MultipleDocumentMainForm(Type userInterfaceType)
      : base(userInterfaceType) {
      InitializeComponent();
    }

    protected override void CreateGUI() {
      base.CreateGUI();
      ToolStripMenuItem window = new ToolStripMenuItem("Windows");
      base.menuStrip.MdiWindowListItem = window;
      base.menuStrip.Items.Add(window);
    }

    public override void ShowView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)ShowView, view);
      else {
        if (views.Contains(view)) {
          DocumentForm documentForm = FindForm(view);
          if (documentForm != null)
            documentForm.Focus();
        } else {
          base.ShowView(view);
          DocumentForm form = new DocumentForm(view);
          form.Activated += new EventHandler(MultipleDocumentFormActivated);
          form.FormClosing += new FormClosingEventHandler(view.FormClosing);
          form.FormClosed += new FormClosedEventHandler(MultipleDocumentFormClosed);
          form.MdiParent = this;
          foreach (IToolStripItem item in ToolStripItems)
            view.StateChanged += new EventHandler(item.ViewChanged);
          form.Show();
        }
      }
    }

    public override void CloseView(IView view) {
      DocumentForm documentForm = FindForm(view);
      if (documentForm != null)
        documentForm.Close();
    }

    private void MultipleDocumentFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((DocumentForm)sender).View;
    }

    private void MultipleDocumentFormClosed(object sender, FormClosedEventArgs e) {
      DocumentForm form = (DocumentForm)sender;
      views.Remove(form.View);
      if (views.Count == 0)
        ActiveView = null;

      ViewClosed(form.View);
      form.Activated -= new EventHandler(MultipleDocumentFormActivated);
      form.FormClosing -= new FormClosingEventHandler(form.View.FormClosing);
      form.FormClosed -= new FormClosedEventHandler(MultipleDocumentFormClosed);
      foreach (IToolStripItem item in ToolStripItems)
        form.View.StateChanged -= new EventHandler(item.ViewChanged);
    }

    protected DocumentForm FindForm(IView view) {
      IEnumerable<DocumentForm> forms = this.MdiChildren.Cast<DocumentForm>().Where(df => df.View == view);
      if (forms.Count() == 1)
        return forms.Single();
      return null;
    }
  }
}
