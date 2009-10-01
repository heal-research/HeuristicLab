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
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm {
  public partial class DockingMainForm : MainFormBase {
    public DockingMainForm()
      : base() {
      InitializeComponent();
    }

    public DockingMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
    }

    public override void ShowView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)ShowView, view);
      else {
        if (views.Contains(view)) {
          DockForm dockform = FindForm(view);
          if (dockform != null)
            dockform.Activate();
        } else {
          base.ShowView(view);
          DockContent dockForm = new DockForm(view);
          dockForm.Activated += new EventHandler(DockFormActivated);
          dockForm.FormClosing += new FormClosingEventHandler(view.FormClosing);
          dockForm.FormClosed += new FormClosedEventHandler(DockFormClosed);
          foreach (IToolStripItem item in ToolStripItems)
            view.StateChanged += new EventHandler(item.ViewChanged);
          dockForm.Show(dockPanel);
        }
      }
    }

    public override void CloseView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)CloseView, view);
      else {
        DockForm dockform = FindForm(view);
        if (dockform != null)
          dockform.Close();
      }
    }

    private void DockFormClosed(object sender, FormClosedEventArgs e) {
      DockForm dockForm = (DockForm)sender;
      ViewClosed(dockForm.View);
      dockForm.Activated -= new EventHandler(DockFormActivated);
      dockForm.FormClosing -= new FormClosingEventHandler(dockForm.View.FormClosing);
      dockForm.FormClosed -= new FormClosedEventHandler(DockFormClosed);
      foreach (IToolStripItem item in ToolStripItems)
        dockForm.View.StateChanged -= new EventHandler(item.ViewChanged);
    }

    private void DockFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((DockForm)sender).View;
    }

    protected DockForm FindForm(IView view) {
      IEnumerable<DockForm> dockforms;

      dockforms = from df in dockPanel.Documents
                  where ((DockForm)df).View == view
                  select (DockForm)df;
      if (dockforms.Count() == 1)
        return dockforms.Single();

      dockforms = from fw in dockPanel.FloatWindows
                  from np in fw.NestedPanes
                  from dc in np.Contents
                  where ((DockForm)dc).View == view
                  select (DockForm)dc;
      if (dockforms.Count() == 1)
        return dockforms.Single();

      dockforms = from dw in dockPanel.DockWindows
                  from np in dw.NestedPanes
                  from dc in np.Contents
                  where ((DockForm)dc).View == view
                  select (DockForm)dc;
      if (dockforms.Count() == 1)
        return dockforms.Single();

      return null;
    }
  }
}
