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

using System;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Ensemble Solutions")]
  [Content(typeof(ClassificationEnsembleSolution), false)]
  internal sealed partial class ClassificationEnsembleSolutionModelView : DataAnalysisSolutionEvaluationView {
    private ModelsView view;

    public ClassificationEnsembleSolutionModelView() {
      InitializeComponent();
      view = new ModelsView();
      view.Dock = DockStyle.Fill;
      Controls.Add(view);
    }

    public new ClassificationEnsembleSolution Content {
      get { return (ClassificationEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        view.Locked = Content.ProblemData == ClassificationEnsembleProblemData.EmptyProblemData;
        view.Content = Content.ClassificationSolutions;
      } else
        view.Content = null;
    }

    public override System.Drawing.Image ViewImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Properties; }
    }

    private class ModelsView : ItemCollectionView<IClassificationSolution> {
      protected override void SetEnabledStateOfControls() {
        base.SetEnabledStateOfControls();
        addButton.Enabled = Content != null && !Content.IsReadOnly && !Locked;
        removeButton.Enabled = Content != null && !Content.IsReadOnly && !Locked && itemsListView.SelectedItems.Count > 0;
        itemsListView.Enabled = Content != null;
        detailsGroupBox.Enabled = Content != null && itemsListView.SelectedItems.Count == 1;
      }

      protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
        validDragOperation = false;
        if (ReadOnly || Locked) return;
        if (Content.IsReadOnly) return;

        var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        validDragOperation = dropData.GetObjectGraphObjects().OfType<IClassificationSolution>().Any();
      }
      protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
        if (e.Effect != DragDropEffects.None) {
          var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          var solutions = dropData.GetObjectGraphObjects().OfType<IClassificationSolution>();
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            Cloner cloner = new Cloner();
            solutions = solutions.Select(s => cloner.Clone(s));
          }
          var solutionCollection = Content as ItemCollection<IClassificationSolution>;
          if (solutionCollection != null) {
            solutionCollection.AddRange(solutions);
          } else {
            foreach (var solution in solutions)
              Content.Add(solution);
          }
        }
      }
      protected override void itemsListView_KeyDown(object sender, KeyEventArgs e) {
        var solutionCollection = Content as ItemCollection<IClassificationSolution>;
        if (e.KeyCode == Keys.Delete && solutionCollection != null) {
          if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
            solutionCollection.RemoveRange(itemsListView.SelectedItems.Cast<ListViewItem>().Select(x => (IClassificationSolution)x.Tag));
          }
        } else {
          base.itemsListView_KeyDown(sender, e);
        }
      }
      protected override void removeButton_Click(object sender, EventArgs e) {
        var solutionCollection = Content as ItemCollection<IClassificationSolution>;
        if (itemsListView.SelectedItems.Count > 0 && solutionCollection != null) {
          solutionCollection.RemoveRange(itemsListView.SelectedItems.Cast<ListViewItem>().Select(x => (IClassificationSolution)x.Tag));
          itemsListView.SelectedItems.Clear();
        } else {
          base.removeButton_Click(sender, e);
        }
      }
    }
  }
}
