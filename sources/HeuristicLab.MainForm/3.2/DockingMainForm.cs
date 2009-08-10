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

    private void DockFormClosed(object sender, FormClosedEventArgs e) {
      DockForm dockForm = (DockForm)sender;
      views.Remove(dockForm.View);
      if (views.Count == 0)
        ActiveView = null;
      dockForm.Activated -= new EventHandler(DockFormActivated);
      dockForm.FormClosing -= new FormClosingEventHandler(dockForm.View.FormClosing);
      dockForm.FormClosed -= new FormClosedEventHandler(DockFormClosed);
      foreach (IToolStripItem item in ToolStripItems)
        dockForm.View.StateChanged -= new EventHandler(item.ViewChanged);
    }

    private void DockFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((DockForm)sender).View;
      base.StatusStripText = ((DockForm)sender).View.Caption;
    }
  }
}
