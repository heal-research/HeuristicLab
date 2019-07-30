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

using System;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.GeneticProgramming.Robocode;

namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  [View("Robocode Tank Code View")]
  [Content(typeof(Solution), IsDefaultView = true)]
  public partial class SolutionCodeView : ItemView {
    private const string programName = "BestSolution";

    public new Solution Content {
      get { return (Solution)base.Content; }
      set { base.Content = value; }
    }

    public SolutionCodeView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      programCode.Text = Content == null ? string.Empty : Interpreter.InterpretProgramTree(Content.Tree.Root, programName);
    }

    private void btnSave_Click(object sender, EventArgs e) {
      saveFileDialog.FileName = programName;
      var result = saveFileDialog.ShowDialog(this);
      if (result == DialogResult.OK)
        File.WriteAllText(saveFileDialog.FileName, programCode.Text);
    }

    private void btnRunInRobocode_Click(object sender, EventArgs e) {
      using (var battleRunnerDlg = new BattleRunnerDialog(Content)) {
        var result = battleRunnerDlg.ShowDialog(this);
        if (result == DialogResult.OK) {
          var enemies = battleRunnerDlg.Enemies;
          string path = enemies.RobocodePath;
          int nrOfRounds = battleRunnerDlg.NrOfRounds;
          Interpreter.EvaluateTankProgram(Content.Tree, path, enemies, programName, true, nrOfRounds);
        }
      }
    }
  }
}
