#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm.WindowsForms {
  /// <summary>
  /// Displays the used view.
  /// </summary>
  internal partial class DockForm : DockContent {
    public DockForm(IView view) {
      InitializeComponent();
      this.view = view;
      if (View != null) {
        if (view is UserControl) {
          switch (((UserControl)view).Dock) {
            case DockStyle.Left:
              this.ShowHint = DockState.DockLeft;
              break;
            case DockStyle.Right:
              this.ShowHint = DockState.DockRight;
              break;
            case DockStyle.Top:
              this.ShowHint = DockState.DockTop;
              break;
            case DockStyle.Bottom:
              this.ShowHint = DockState.DockBottom;
              break;
          }
          Control control = (Control)View;
          control.Dock = DockStyle.Fill;
          this.Size = control.Size;
          viewPanel.Controls.Add(control);
          View.CaptionChanged += new EventHandler(View_CaptionChanged);
          UpdateText();
        }
      } else {
        Label errorLabel = new Label();
        errorLabel.Name = "errorLabel";
        errorLabel.Text = "No view available";
        errorLabel.AutoSize = false;
        errorLabel.Dock = DockStyle.Fill;
        viewPanel.Controls.Add(errorLabel);
      }
    }

    private IView view;
    public IView View {
      get { return view; }
    }

    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else
        this.Text = View.Caption;
    }

    #region View Events
    private void View_CaptionChanged(object sender, EventArgs e) {
      UpdateText();
    }
    #endregion
  }
}
