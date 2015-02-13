#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  internal partial class DocumentForm : Form {
    private IView view;

    public DocumentForm() {
      InitializeComponent();
    }
    public DocumentForm(IView view)
      : this() {
      this.view = view;
      if (view != null) {
        Type viewType = view.GetType();
        Control control = (Control)view;
        control.Dock = DockStyle.Fill;
        view.CaptionChanged += new EventHandler(View_CaptionChanged);
        UpdateText();
        viewPanel.Controls.Add(control);
      } else {
        Label errorLabel = new Label();
        errorLabel.Name = "errorLabel";
        errorLabel.Text = "No view available";
        errorLabel.AutoSize = false;
        errorLabel.Dock = DockStyle.Fill;
        viewPanel.Controls.Add(errorLabel);
      }
    }

    protected override void Dispose(bool disposing) {
      if (view != null)
        view.CaptionChanged -= new System.EventHandler(View_CaptionChanged);
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else
        Text = view.Caption;
    }

    #region View Events
    private void View_CaptionChanged(object sender, EventArgs e) {
      UpdateText();
    }
    #endregion
  }
}
