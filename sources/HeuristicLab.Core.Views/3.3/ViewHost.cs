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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views { 
  public partial class ViewHost : UserControl {
    private object obj;
    public object Object {
      get { return obj; }
      set {
        if (value != obj) {
          obj = value;
          Initialize();
        }
      }
    }

    public ViewHost() {
      InitializeComponent();
      Initialize();
    }

    private void Initialize() {
      viewLabel.Visible = false;
      viewComboBox.Items.Clear();
      viewComboBox.Enabled = false;
      viewComboBox.Visible = false;
      if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
      viewPanel.Controls.Clear();
      viewPanel.Enabled = false;

      if (Object != null) {
        var viewTypes = from t in MainFormManager.GetViewTypes(Object.GetType())
                        orderby t.Name ascending
                        select t;
        foreach (Type viewType in viewTypes)
          viewComboBox.Items.Add(viewType);
        if (viewComboBox.Items.Count > 0) {
          viewLabel.Visible = true;
          viewComboBox.Enabled = true;
          viewComboBox.Visible = true;
        }

        Control view = (Control)MainFormManager.CreateDefaultView(Object);
        if (view != null) {
          viewPanel.Controls.Add(view);
          viewPanel.Tag = view;
          view.Dock = DockStyle.Fill;
          viewPanel.Enabled = true;
          viewComboBox.SelectedItem = view.GetType();
        }
      }
    }

    private void viewComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (viewComboBox.SelectedItem != viewPanel.Tag) {
        if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
        viewPanel.Controls.Clear();
        Control view = (Control)MainFormManager.CreateView((Type)viewComboBox.SelectedItem, Object);
        viewPanel.Controls.Add(view);
        viewPanel.Tag = view;
        view.Dock = DockStyle.Fill;
      }
    }
  }
}
