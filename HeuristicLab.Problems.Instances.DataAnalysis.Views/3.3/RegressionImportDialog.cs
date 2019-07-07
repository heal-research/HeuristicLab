#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  public partial class RegressionImportDialog : DataAnalysisImportDialog {
    public new RegressionImportType ImportType {
      get {
        return new RegressionImportType() {
          Shuffle = ShuffleDataCheckbox.Checked,
          TrainingPercentage = TrainingTestTrackBar.Value,
          TargetVariable = (String)TargetVariableComboBox.SelectedValue
        };
      }
    }

    public RegressionImportDialog() {
      InitializeComponent();
    }

    protected override void CheckAdditionalConstraints(TableFileParser csvParser) {
      base.CheckAdditionalConstraints(csvParser);
      SetPossibleTargetVariables();
    }

    protected void SetPossibleTargetVariables() {
      var dataset = PreviewDatasetMatrix.Content as Dataset;
      if (dataset != null) {
        // Remove " (Double)" at the end of the variable name (last 9 chars)
        TargetVariableComboBox.DataSource = dataset.DoubleVariables.Select(x => x.Substring(0, x.Length - 9)).ToList();
      }
    }
  }
}
