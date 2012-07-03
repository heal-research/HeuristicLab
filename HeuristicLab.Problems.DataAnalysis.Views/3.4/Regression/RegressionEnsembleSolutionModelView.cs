#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Ensemble Solutions")]
  [Content(typeof(RegressionEnsembleSolution), false)]
  internal sealed partial class RegressionEnsembleSolutionModelView : DataAnalysisSolutionEvaluationView {
    private ModelsView view;

    public RegressionEnsembleSolutionModelView() {
      InitializeComponent();
      view = new ModelsView();
      view.Dock = DockStyle.Fill;
      Controls.Add(view);
    }

    public new RegressionEnsembleSolution Content {
      get { return (RegressionEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        view.Locked = Content.ProblemData == RegressionEnsembleProblemData.EmptyProblemData;
        view.Content = Content.RegressionSolutions;
      } else
        view.Content = null;
    }

    public override System.Drawing.Image ViewImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Properties; }
    }

    private class ModelsView : ItemCollectionView<IRegressionSolution> {
      protected override void SetEnabledStateOfControls() {
        base.SetEnabledStateOfControls();
        addButton.Enabled = Content != null && !Content.IsReadOnly && !Locked;
        removeButton.Enabled = Content != null && !Content.IsReadOnly && !Locked && itemsListView.SelectedItems.Count > 0;
        itemsListView.Enabled = Content != null && !Locked;
        detailsGroupBox.Enabled = Content != null && itemsListView.SelectedItems.Count == 1;
      }

      protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
        validDragOperation = false;
        if (ReadOnly || Locked) return;
        if (Content.IsReadOnly) return;

        var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        validDragOperation = dropData.GetObjectGraphObjects().OfType<IRegressionSolution>().Any();
      }
      protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
        if (e.Effect != DragDropEffects.None) {
          var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          var solutions = dropData.GetObjectGraphObjects().OfType<IRegressionSolution>();
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            Cloner cloner = new Cloner();
            solutions = solutions.Select(s => cloner.Clone(s));
          }
          foreach (var solution in solutions)
            Content.Add(solution);
        }
      }
    }
  }
}
