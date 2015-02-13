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
using HeuristicLab.Data;

namespace HeuristicLab.Analysis.Statistics.Views {
  public partial class StatisticalTestsConfigurationDialog : Form {

    private StatisticalTestsView view;

    public StatisticalTestsConfigurationDialog(StatisticalTestsView view) {
      InitializeComponent();
      this.view = view;
      significanceLevelTextBox.Content = new DoubleValue(view.SignificanceLevel);
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      double newVal;

      newVal = ((DoubleValue)significanceLevelTextBox.Content).Value;
      if (newVal > 0 && newVal < 1) {
        view.SignificanceLevel = newVal;
        Close();
      } else {
        MessageBox.Show("Please enter a correct value between 0 and 1 for the significance level.", "HeuristicLab",
          MessageBoxButtons.OK);
      }
    }
  }
}
