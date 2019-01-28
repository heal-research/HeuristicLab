#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class InfoBox : Form {
    protected string embeddedResourceName;
    protected IView parentView;

    public InfoBox(string caption, string embeddedResourceName, IView parentView) {
      InitializeComponent();
      this.Text = caption;
      this.parentView = parentView;
      this.embeddedResourceName = embeddedResourceName;
    }

    protected virtual void LoadEmbeddedResource() {
      string extension = Path.GetExtension(embeddedResourceName);
      Assembly assembly = Assembly.GetAssembly(parentView.GetType());
      try {
        using (Stream stream = assembly.GetManifestResourceStream(embeddedResourceName)) {
          if (extension == ".rtf") {
            infoRichTextBox.LoadFile(stream, RichTextBoxStreamType.RichText);
          } else if (extension == ".txt") {
            infoRichTextBox.LoadFile(stream, RichTextBoxStreamType.UnicodePlainText);
          } else {
            infoRichTextBox.Text = "Error: Unsupported help format: " + extension;
          }
        }
      }
      catch (Exception ex) {
        infoRichTextBox.Text = "Error: Could not load help text. Exception is: " + Environment.NewLine + ex.ToString();
      }
    }

    private void InfoBox_Load(object sender, EventArgs e) {
      LoadEmbeddedResource();
    }

    private void infoRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(e.LinkText);
    }
  }
}
