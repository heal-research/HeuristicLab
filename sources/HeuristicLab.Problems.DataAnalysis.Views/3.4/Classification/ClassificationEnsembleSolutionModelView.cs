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


namespace HeuristicLab.Problems.DataAnalysis.Views {
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
  using HeuristicLab.Core;
  using HeuristicLab.Core.Views;
  using HeuristicLab.MainForm;
  using HeuristicLab.MainForm.WindowsForms;

  [View("Ensemble Solutions")]
  [Content(typeof(IItemCollection<IClassificationSolution>), false)]
  internal sealed partial class ClassificationEnsembleSolutionModelView : ItemCollectionView<IClassificationSolution> {
    public ClassificationEnsembleSolutionModelView() {
      InitializeComponent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addButton.Enabled = Content != null && !Content.IsReadOnly && !Locked;
      removeButton.Enabled = Content != null && !Content.IsReadOnly && !Locked && itemsListView.SelectedItems.Count > 0;
      itemsListView.Enabled = Content != null;
      detailsGroupBox.Enabled = Content != null && itemsListView.SelectedItems.Count == 1;
    }
  }
}
