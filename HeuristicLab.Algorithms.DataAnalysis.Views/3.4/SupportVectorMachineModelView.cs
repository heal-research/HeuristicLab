#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("SVM Model")]
  [Content(typeof(SupportVectorMachineModel), true)]
  public partial class SupportVectorMachineModelView : AsynchronousContentView {

    public new SupportVectorMachineModel Content {
      get { return (SupportVectorMachineModel)base.Content; }
      set { base.Content = value; }
    }

    public SupportVectorMachineModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        textBox.Text = string.Empty;
      else
        UpdateTextBox();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new EventHandler(Content_Changed);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Changed -= new EventHandler(Content_Changed);
    }
    private void Content_Changed(object sender, EventArgs e) {
      UpdateTextBox();
    }

    private void UpdateTextBox() {
      using (MemoryStream s = new MemoryStream()) {
        StreamWriter writer = new StreamWriter(s);
        writer.WriteLine("RangeTransform:");
        writer.Flush();
        using (MemoryStream memStream = new MemoryStream()) {
          RangeTransform.Write(memStream, Content.RangeTransform);
          memStream.Seek(0, SeekOrigin.Begin);
          memStream.WriteTo(s);
        }
        writer.WriteLine("Model:");
        writer.Flush();
        using (MemoryStream memStream = new MemoryStream()) {
          svm.svm_save_model(new StreamWriter(memStream), Content.Model);
          memStream.Seek(0, SeekOrigin.Begin);
          memStream.WriteTo(s);
        }
        s.Flush();
        s.Seek(0, SeekOrigin.Begin);

        StreamReader reader = new StreamReader(s);
        textBox.Text = reader.ReadToEnd();
      }
    }
  }
}
