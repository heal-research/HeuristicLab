#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public partial class StructIdProblemInjectorView : ViewBase {
    public StructIdProblemInjector StructIdProblemInjector {
      get { return (StructIdProblemInjector)Item; }
      set { base.Item = value; }
    }

    public StructIdProblemInjectorView() {
      InitializeComponent();
    }
    public StructIdProblemInjectorView(StructIdProblemInjector structIdProblemInjector)
      : this() {
      StructIdProblemInjector = structIdProblemInjector;
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = StructIdProblemInjector;
      operatorBaseDescriptionView.Operator = StructIdProblemInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (StructIdProblemInjector == null) {
        nameTextBox.Text = "-";
        variablesTextBox.Text = "-";
        samplesTextBox.Text = "-";
        importInstanceButton.Enabled = false;
      } else {
        Dataset dataset = (Dataset)StructIdProblemInjector.GetVariable("Dataset").Value;
        nameTextBox.Text = dataset.Name;
        samplesTextBox.Text = dataset.Rows+"";
        variablesTextBox.Text = dataset.Columns+"";
        importInstanceButton.Enabled = true;
      }
    }

    private void importInstanceButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        DatasetParser parser = new DatasetParser();
        bool success = false;
        try {
          parser.Import(openFileDialog.FileName, true);
          success = true;
        } catch (Exception) {
          // not possible to parse strictly => try to parse non-strict
          parser.Import(openFileDialog.FileName, false);
          success = true;
        }
        if (success) {
          Dataset dataset = (Dataset)StructIdProblemInjector.GetVariable("Dataset").Value;
          dataset.Rows = parser.TrainingSamplesEnd - parser.TrainingSamplesStart;
          dataset.Columns = parser.Columns;
          dataset.VariableNames = parser.VariableNames;
          dataset.Name = parser.ProblemName;
          dataset.Samples = new double[dataset.Rows * dataset.Columns];
          Array.Copy(parser.Samples, dataset.Samples, dataset.Columns * dataset.Rows);

          ((IntData)StructIdProblemInjector.GetVariable("TargetVariable").Value).Data = parser.TargetVariable;
          ((IntData)StructIdProblemInjector.GetVariable("MaxTreeHeight").Value).Data = parser.MaxTreeHeight;
          ((IntData)StructIdProblemInjector.GetVariable("MaxTreeSize").Value).Data = parser.MaxTreeSize;
          Refresh();
        }
      }
    }
  }
}
