#region License Information
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

using System.IO;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Problems.GeneticProgramming.Robocode;

namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  public partial class BattleRunnerDialog : Form {
    public int NrOfRounds {
      get { return (int)nrOfRoundsNumericUpDown.Value; }
      set { nrOfRoundsNumericUpDown.Value = value; }
    }

    public EnemyCollection Enemies {
      get { return enemyCollectionView.Content; }
      set { enemyCollectionView.Content = value; }
    }

    public BattleRunnerDialog(Solution solution) {
      InitializeComponent();
      errorProvider.SetIconAlignment(robocodePathTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(robocodePathTextBox, 2);
      nrOfRoundsNumericUpDown.Maximum = int.MaxValue;
      NrOfRounds = solution.NrOfRounds;
      Enemies = (EnemyCollection)solution.Enemies.Clone(new Cloner());
      robocodePathTextBox.Text = Enemies.RobocodePath;
    }

    private void searchButton_Click(object sender, System.EventArgs e) {
      var result = folderBrowserDialog.ShowDialog(this);
      if (result == DialogResult.OK)
        Enemies.RobocodePath = robocodePathTextBox.Text = folderBrowserDialog.SelectedPath;
    }

    private void runInRobocodeButton_Click(object sender, System.EventArgs e) {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void robocodePathTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      string robocodePath = robocodePathTextBox.Text;
      if (string.IsNullOrEmpty(robocodePath)) {
        e.Cancel = true;
        errorProvider.SetError(robocodePathTextBox, "Robocode directory has to be set");
        robocodePathTextBox.SelectAll();
        return;
      }
      var info = new DirectoryInfo(robocodePath);
      if (!info.Exists) {
        e.Cancel = true;
        errorProvider.SetError(robocodePathTextBox, "This directory does not exist");
        robocodePathTextBox.SelectAll();
        return;
      }
      Enemies.RobocodePath = robocodePathTextBox.Text;
    }

    private void robocodePathTextBox_Validated(object sender, System.EventArgs e) {
      errorProvider.SetError(robocodePathTextBox, string.Empty);
    }
  }
}
