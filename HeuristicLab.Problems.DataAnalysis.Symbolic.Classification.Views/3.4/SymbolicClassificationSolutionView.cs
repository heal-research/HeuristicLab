﻿#region License Information
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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  [Content(typeof(SymbolicClassificationSolution), false)]
  [View("SymbolicDiscriminantFunctionClassificationSolution View")]
  public partial class SymbolicClassificationSolutionView : ClassificationSolutionView {
    public SymbolicClassificationSolutionView() {
      InitializeComponent();
    }

    protected new SymbolicClassificationSolution Content {
      get { return (SymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      btnSimplify.Enabled = Content != null && !Locked && Content.ProblemData.TrainingIndices.Any(); // simplification is only possible if there are trainings samples
      exportButton.Enabled = Content != null && !Locked;
    }

    private void btn_SimplifyModel_Click(object sender, EventArgs e) {
      var view = new InteractiveSymbolicClassificationSolutionSimplifierView();
      view.Content = (SymbolicClassificationSolution)this.Content.Clone();
      view.Show();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      var exporter = new SymbolicSolutionExcelExporter();
      exportFileDialog.Filter = exporter.FileTypeFilter;
      if (exportFileDialog.ShowDialog(this) == DialogResult.OK) {

        var name = exportFileDialog.FileName;
        using (BackgroundWorker bg = new BackgroundWorker()) {
          Progress.Show(this, "Exportion solution to " + name + ".", ProgressMode.Indeterminate);
          bg.DoWork += (o, a) => exporter.Export(Content, name);
          bg.RunWorkerCompleted += (o, a) => Progress.Hide(this);
          bg.RunWorkerAsync();
        }
      }
    }
  }
}
