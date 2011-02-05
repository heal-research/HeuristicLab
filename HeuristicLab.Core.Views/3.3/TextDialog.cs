#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  public partial class TextDialog : Form {
    public string Caption {
      get { return this.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.Caption = x), value);
        else
          this.Text = value;
      }
    }
    public string Content {
      get { return textTextBox.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.Content = x), value);
        else
          textTextBox.Text = value;
      }
    }
    public bool ReadOnly {
      get { return textTextBox.ReadOnly; }
      set {
        if (InvokeRequired)
          Invoke(new Action<bool>(x => this.ReadOnly = x), value);
        else {
          textTextBox.ReadOnly = value;
          okButton.Enabled = !value;
        }
      }
    }

    public TextDialog() {
      InitializeComponent();
    }
    public TextDialog(string caption, string content, bool readOnly)
      : this() {
      Caption = caption;
      Content = content;
      ReadOnly = readOnly;
    }

    protected virtual void okButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
