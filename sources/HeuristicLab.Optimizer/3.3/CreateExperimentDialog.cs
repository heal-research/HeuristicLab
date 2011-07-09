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

using System;
using System.Windows.Forms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        optimizer = value;
        experiment = null;
        okButton.Enabled = optimizer != null;
      }
    }

    private Experiment experiment;
    public Experiment Experiment {
      get { return experiment; }
    }

    public CreateExperimentDialog() {
      experiment = null;
      optimizer = null;
      InitializeComponent();
    }
    public CreateExperimentDialog(IOptimizer optimizer)
      : this() {
      Optimizer = optimizer;
    }

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
    }
    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
    }
    private void okButton_Click(object sender, EventArgs e) {
      experiment = new Experiment();
      if (createBatchRunCheckBox.Checked) {
        BatchRun batchRun = new BatchRun();
        batchRun.Repetitions = (int)repetitionsNumericUpDown.Value;
        batchRun.Optimizer = (IOptimizer)Optimizer.Clone();
        Experiment.Optimizers.Add(batchRun);
      } else {
        Experiment.Optimizers.Add((IOptimizer)Optimizer.Clone());
      }
      Experiment.Prepare(true);
    }
  }
}
