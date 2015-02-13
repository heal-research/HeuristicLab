#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Analysis.Views {
  [View("AlleleFrequency View")]
  [Content(typeof(AlleleFrequency), true)]
  public partial class AlleleFrequencyView : ItemView {
    public new AlleleFrequency Content {
      get { return (AlleleFrequency)base.Content; }
      set { base.Content = value; }
    }

    public AlleleFrequencyView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        idTextBox.Text = string.Empty;
        frequencyTextBox.Text = string.Empty;
        averageImpactTextBox.Text = string.Empty;
        averageSolutionQualityTextBox.Text = string.Empty;
        containedInBestKnownSolutionCheckBox.Checked = false;
        containedInBestSolutionCheckBox.Checked = false;
      } else {
        idTextBox.Text = Content.Id;
        frequencyTextBox.Text = Content.Frequency.ToString();
        averageImpactTextBox.Text = Content.AverageImpact.ToString();
        averageSolutionQualityTextBox.Text = Content.AverageSolutionQuality.ToString();
        containedInBestKnownSolutionCheckBox.Checked = Content.ContainedInBestKnownSolution;
        containedInBestSolutionCheckBox.Checked = Content.ContainedInBestSolution;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      idTextBox.Enabled = Content != null;
      frequencyTextBox.Enabled = Content != null;
      averageImpactTextBox.Enabled = Content != null;
      averageSolutionQualityTextBox.Enabled = Content != null;
    }
  }
}
