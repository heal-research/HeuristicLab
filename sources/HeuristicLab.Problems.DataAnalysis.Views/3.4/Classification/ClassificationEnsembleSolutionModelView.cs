#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
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
      if (Content != null)
        view.Content = Content.ClassificationSolutions;
      else
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
    }
  }
}
