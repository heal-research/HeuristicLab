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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private IAlgorithm algorithm;
    public IAlgorithm Algorithm {
      get { return algorithm; }
      set {
        algorithm = value;
        experiment = null;
        okButton.Enabled = algorithm != null;
      }
    }
    
    private Experiment experiment;
    public Experiment Experiment {
      get { return experiment; }
    }

    public CreateExperimentDialog() {
      experiment = null;
      algorithm = null;
      InitializeComponent();
    }
    public CreateExperimentDialog(IAlgorithm algorithm) : this() {
      Algorithm = algorithm;
    }

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
    }
    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
    }
    private void okButton_Click(object sender, EventArgs e) {
      experiment = new Experiment(Algorithm.Name);
      if (createBatchRunCheckBox.Checked) {
        BatchRun batchRun = new BatchRun(Algorithm.Name);
        batchRun.Repetitions = (int)repetitionsNumericUpDown.Value;
        batchRun.Algorithm = (IAlgorithm)Algorithm.Clone();
        Experiment.Optimizers.Add(batchRun);
      } else {
        Experiment.Optimizers.Add((IAlgorithm)Algorithm.Clone());
      }
      Experiment.Prepare(true);
    }
  }
}
