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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  [Content(typeof(SymbolicRegressionSolution), false)]
  [View("SymbolicRegressionSolution View")]
  public partial class SymbolicRegressionSolutionView : RegressionSolutionView {
    public SymbolicRegressionSolutionView() {
      InitializeComponent();
    }

    protected new SymbolicRegressionSolution Content {
      get { return (SymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      btnSimplify.Enabled = Content != null && !Locked;
      exportButton.Enabled = Content != null && !Locked;
    }

    private void btn_SimplifyModel_Click(object sender, EventArgs e) {
      InteractiveSymbolicRegressionSolutionSimplifierView view = new InteractiveSymbolicRegressionSolutionSimplifierView();
      view.Content = (SymbolicRegressionSolution)this.Content.Clone();
      view.Show();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      var exporter = new SymbolicSolutionExcelExporter();
      exportFileDialog.Filter = exporter.FileTypeFilter;
      if (exportFileDialog.ShowDialog(this) == DialogResult.OK) {

        var name = exportFileDialog.FileName;
        using (BackgroundWorker bg = new BackgroundWorker()) {
          MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, "Exportion solution to " + name + ".");
          bg.DoWork += (o, a) => exporter.Export(Content, name);
          bg.RunWorkerCompleted += (o, a) => MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
          bg.RunWorkerAsync();
        }
      }
    }
  }
}
