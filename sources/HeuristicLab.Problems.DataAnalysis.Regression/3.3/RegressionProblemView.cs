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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Regression {
  [View("Regression Problem View")]
  [Content(typeof(RegressionProblem), true)]
  public partial class RegressionProblemView : NamedItemView {
    private OpenFileDialog openFileDialog;

    public new RegressionProblem Content {
      get { return (RegressionProblem)base.Content; }
      set { base.Content = value; }
    }

    public RegressionProblemView() {
      InitializeComponent();
    }

    public RegressionProblemView(RegressionProblem content)
      : this() {
      Content = content;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        parameterCollectionView.Enabled = false;
        importButton.Enabled = false;
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        parameterCollectionView.Enabled = true;
        importButton.Enabled = true;
      }
    }

    private void importButton_Click(object sender, System.EventArgs e) {
      if (openFileDialog == null) openFileDialog = new OpenFileDialog();

      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ImportFromFile(openFileDialog.FileName);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
  }
}
