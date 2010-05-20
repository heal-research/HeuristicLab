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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.MainForm.WindowsForms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;
using System.IO;
using HeuristicLab.Problems.DataAnalysis.Regression.SupportVectorRegression;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Support Vector Regression Solution View")]
  [Content(typeof(SupportVectorRegressionSolution))]
  public partial class SupportVectorRegressionSolutionView : AsynchronousContentView {

    public new SupportVectorRegressionSolution Content {
      get { return (SupportVectorRegressionSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    public SupportVectorRegressionSolutionView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        dataPanel.Content = Content.ProblemData;
        modelPanel.Content = Content.Model;
        supportVectorViewHost.Content = Content.SupportVectors;
      } else {
        dataPanel.Content = null;
        modelPanel.Content = null;
        supportVectorViewHost.Content = null;
      }
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
    }

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      dataPanel.Content = Content.ProblemData;
    }
  }
}
