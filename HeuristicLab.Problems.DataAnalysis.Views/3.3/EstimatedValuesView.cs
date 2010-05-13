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
using HeuristicLab.Data.Views;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Values View")]
  [Content(typeof(DataAnalysisSolution))]
  public partial class EstimatedValuesView : AsynchronousContentView {
    private const string TARGETVARIABLE_SERIES_NAME = "TargetVariable";
    private const string ESTIMATEDVALUES_SERIES_NAME = "EstimatedValues";

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    private StringConvertibleMatrixView matrixView;

    public EstimatedValuesView()
      : base() {
      InitializeComponent();
      matrixView = new StringConvertibleMatrixView();
      matrixView.Dock = DockStyle.Fill;
      this.Controls.Add(matrixView);
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EstimatedValuesChanged += new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.EstimatedValuesChanged -= new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateEstimatedValues();
    }

    private void UpdateEstimatedValues() {
      if (InvokeRequired) Invoke((Action)UpdateEstimatedValues);
      else {
        double[,] values =
        MatrixExtensions<double>.Create(
          Content.ProblemData.Dataset[Content.ProblemData.TargetVariable.Value],
          Content.EstimatedValues.ToArray());
        var content = new DoubleMatrix(values);
        content.ColumnNames = new string[] { "Original", "Estimated" };
        matrixView.Content = content;
      }
    }
    #endregion
  }
}
