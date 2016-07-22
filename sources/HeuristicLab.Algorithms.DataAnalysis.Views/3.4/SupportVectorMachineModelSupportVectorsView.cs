#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("SVM Support Vectors")]
  [Content(typeof(SupportVectorMachineModel), false)]
  public partial class SupportVectorMachineModelSupportVectorsView : AsynchronousContentView {

    public new SupportVectorMachineModel Content {
      get { return (SupportVectorMachineModel)base.Content; }
      set { base.Content = value; }
    }

    public SupportVectorMachineModelSupportVectorsView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null)
        stringConvertibleMatrixView.Content = Content.SupportVectors;
      else
        stringConvertibleMatrixView.Content = null;
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
      OnContentChanged();
    }
  }
}
