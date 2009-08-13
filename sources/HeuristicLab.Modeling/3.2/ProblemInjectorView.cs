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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public partial class ProblemInjectorView : ViewBase {
    public ProblemInjector ProblemInjector {
      get { return (ProblemInjector)Item; }
      set { base.Item = value; }
    }

    public ProblemInjectorView()
      : base() {
      InitializeComponent();
    }
    public ProblemInjectorView(ProblemInjector problemInjector)
      : this() {
      ProblemInjector = problemInjector;
      variablesView.Operator = problemInjector;
      variablesView.Update();
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = ProblemInjector;
      operatorBaseDescriptionView.Operator = ProblemInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProblemInjector == null) {
        importInstanceButton.Enabled = false;
      } else {
        Dataset dataset = (Dataset)ProblemInjector.GetVariable("Dataset").Value;
        datasetView.Dataset = dataset;
        importInstanceButton.Enabled = true;
      }
    }

    private void importInstanceButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        DatasetParser parser = new DatasetParser();
        bool success = false;
        try {
          try {
            parser.Import(openFileDialog.FileName, true);
            success = true;
          }
          catch (DataFormatException ex) {
            ShowWarningMessageBox(ex);
            // not possible to parse strictly => clear and try to parse non-strict
            parser.Reset();
            parser.Import(openFileDialog.FileName, false);
            success = true;
          }
        }
        catch (DataFormatException ex) {
          // if the non-strict parsing also failed then show the exception
          ShowErrorMessageBox(ex);
        }
        if (success) {
          Dataset dataset = (Dataset)ProblemInjector.GetVariable("Dataset").Value;
          dataset.Rows = parser.Rows;
          dataset.Columns = parser.Columns;
          dataset.Name = parser.ProblemName;
          dataset.Samples = new double[dataset.Rows * dataset.Columns];
          Array.Copy(parser.Samples, dataset.Samples, dataset.Columns * dataset.Rows);
          TrySetVariable("TrainingSamplesStart", parser.TrainingSamplesStart);
          TrySetVariable("TrainingSamplesStart", parser.TrainingSamplesStart);
          TrySetVariable("TrainingSamplesEnd", parser.TrainingSamplesEnd);
          TrySetVariable("ValidationSamplesStart", parser.ValidationSamplesStart);
          TrySetVariable("ValidationSamplesEnd", parser.ValidationSamplesEnd);
          TrySetVariable("TestSamplesStart", parser.TestSamplesStart);
          TrySetVariable("TestSamplesEnd", parser.TestSamplesEnd);
          TrySetVariable("TargetVariable", parser.TargetVariable);

          for (int i = 0; i < parser.VariableNames.Length; i++) {
            dataset.SetVariableName(i, parser.VariableNames[i]);
          }

          IVariable var = ProblemInjector.GetVariable("AllowedFeatures");
          if (var != null) {
            ItemList<IntData> allowedFeatures = (ItemList<IntData>)var.Value;
            allowedFeatures.Clear();
            List<int> nonInputVariables = parser.NonInputVariables;
            for (int i = 0; i < dataset.Columns; i++) {
              if (!nonInputVariables.Contains(i)) allowedFeatures.Add(new IntData(i));
            }
          }
          Refresh();
        }
      }
    }

    private void TrySetVariable(string name, int value) {
      IVariable var = ProblemInjector.GetVariable(name);
      if (var != null) {
        ((IntData)var.Value).Data = value;
      }
    }

    private void ShowWarningMessageBox(Exception ex) {
      MessageBox.Show(ex.Message,
                      "Warning - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
    }
    private void ShowErrorMessageBox(Exception ex) {
      MessageBox.Show(BuildErrorMessage(ex),
                      "Error - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
    private string BuildErrorMessage(Exception ex) {
      StringBuilder sb = new StringBuilder();
      sb.Append("Sorry, but something went wrong!\n\n" + ex.Message + "\n\n" + ex.StackTrace);

      while (ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append("\n\n-----\n\n" + ex.Message + "\n\n" + ex.StackTrace);
      }
      return sb.ToString();
    }
  }
}
