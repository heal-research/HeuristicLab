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
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HeuristicLab.Core;

namespace HeuristicLab.AdvancedOptimizationFrontend {
  public partial class ViewForm : DockContent {
    private IView myView;
    public IView View {
      get { return myView; }
    }

    public ViewForm() {
      InitializeComponent();
    }
    public ViewForm(IView view)
      : this() {
      myView = view;
      if (View != null) {
        Control control = (Control)View;
        control.Dock = DockStyle.Fill;
        viewPanel.Controls.Add(control);
        View.CaptionChanged += new EventHandler(View_CaptionChanged);
        UpdateText();
      } else {
        Label errorLabel = new Label();
        errorLabel.Name = "errorLabel";
        errorLabel.Text = "No view available";
        errorLabel.AutoSize = false;
        errorLabel.Dock = DockStyle.Fill;
        viewPanel.Controls.Add(errorLabel);
      }
    }

    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else
        Text = View.Caption;
    }

    private void ViewForm_TextChanged(object sender, EventArgs e) {
      TabText = Text;
    }

    #region View Events
    private void View_CaptionChanged(object sender, EventArgs e) {
      UpdateText();
    }
    #endregion
  }
}
