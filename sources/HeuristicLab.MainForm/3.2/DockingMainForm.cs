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
      DockForm dockform = FindForm(view);
      if (dockform != null)
        dockform.Close();
    }

    private void DockFormClosed(object sender, FormClosedEventArgs e) {
      DockForm dockForm = (DockForm)sender;
      views.Remove(dockForm.View);
      if (views.Count == 0)
        ActiveView = null;

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

      if (dockPanel.Documents.Count() != 0) {
        dockforms = dockPanel.Documents.Cast<DockForm>().Where(df => df.View == view);
        if (dockforms.Count() == 1)
          return dockforms.Single();
      }
      if (dockPanel.FloatWindows.Count() != 0) {
        foreach (FloatWindow fw in dockPanel.FloatWindows) {
          foreach (DockContentCollection dc in fw.NestedPanes.Select(np => np.Contents)) {
            dockforms = dc.Cast<DockForm>().Where(df => df.View == view);
            if (dockforms.Count() == 1)
              return dockforms.Single();
          }
        }
      }
      if (dockPanel.DockWindows.Count != 0) {
        foreach (DockWindow dw in dockPanel.DockWindows) {
          foreach (DockContentCollection dc in dw.NestedPanes.Select(np => np.Contents)) {
            dockforms = dc.Cast<DockForm>().Where(df => df.View == view);
            if (dockforms.Count() == 1)
              return dockforms.Single();
          }
        }
      }
      return null;
    }
  }
}
