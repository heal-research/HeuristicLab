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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Data Analysis Solution View")]
  [Content(typeof(DataAnalysisSolution))]
  public partial class DataAnalysisSolutionView : AsynchronousContentView {
    public DataAnalysisSolutionView() {
      InitializeComponent();
    }

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        modelViewHost.Content = Content.Model;
        dataViewHost.Content = Content.ProblemData;
      } else {
        modelViewHost.Content = null;
        dataViewHost.Content = null;
      }
      SetEnabledStateOfControls();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      dataViewHost.Content = Content.ProblemData;
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
    }
    protected override void OnLockedChanged() {
      base.OnLockedChanged();
    }
    private void SetEnabledStateOfControls() {
    }
  }
}
