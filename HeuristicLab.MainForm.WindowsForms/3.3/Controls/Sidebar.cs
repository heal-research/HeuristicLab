#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class Sidebar : View {
    public Sidebar() {
      InitializeComponent();
    }

    private bool collapsed;
    public bool Collapsed {
      get { return this.collapsed; }
      set {
        if (this.collapsed != value) {
          this.collapsed = value;

          DockForm form = MainFormManager.GetMainForm<MainForm>().GetForm(this) as DockForm;
          if (form != null) {
            if (form.DockState == DockState.DockLeft || form.DockState == DockState.DockLeftAutoHide)
              form.DockState = collapsed ? DockState.DockLeftAutoHide : DockState.DockLeft;
            else if (form.DockState == DockState.DockRight || form.DockState == DockState.DockRightAutoHide)
              form.DockState = collapsed ? DockState.DockRightAutoHide : DockState.DockRight;
          }

          this.OnCollapsedChanged();
        }
      }
    }

    protected virtual void OnCollapsedChanged() {
    }

    internal protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      if (e.CloseReason == CloseReason.UserClosing) {
        e.Cancel = true;
        this.Hide();
      }
    }
  }
}
