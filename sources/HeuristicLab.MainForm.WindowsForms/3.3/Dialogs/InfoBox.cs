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

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class InfoBox : Form {
    public string Caption {
      get { return this.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.Text = x), value);
        else
          this.Text = value;
      }
    }

    protected string embeddedResourceName;
    public string EmbeddedResourceName {
      get { return embeddedResourceName; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.EmbeddedResourceName = x), value);
        else {
          embeddedResourceName = value;
          LoadEmbeddedResource(embeddedResourceName);
        }
      }
    }

    public IView ParentView { get; set; }

    public InfoBox() {
      InitializeComponent();
    }

    public InfoBox(string caption, string embeddedResourceName, IView parentView)
      : this() {
      Caption = caption;
      ParentView = parentView;
      EmbeddedResourceName = embeddedResourceName;
    }

    protected virtual void LoadEmbeddedResource(string name) {
      Assembly assembly = Assembly.GetAssembly(ParentView.GetType());
      try {
        using (Stream stream = assembly.GetManifestResourceStream(name))
          infoRichTextBox.LoadFile(stream, RichTextBoxStreamType.RichText);
      }
      catch (Exception ex) {
        infoRichTextBox.Text = "Error: Could not load help text. Exception is: " + Environment.NewLine + ex.ToString();
      }
    }

    protected virtual void okButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
