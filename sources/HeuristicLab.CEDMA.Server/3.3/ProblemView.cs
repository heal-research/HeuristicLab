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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using System.Diagnostics;
using HeuristicLab.CEDMA.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class ProblemView : ViewBase {
    private Problem problem;

    public ProblemView(Problem problem) {
      this.problem = problem;
      problem.Changed += (sender, args) => UpdateControls();
      InitializeComponent();
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      trainingSamplesStartTextBox.Text = problem.TrainingSamplesStart.ToString();
      trainingSamplesEndTextBox.Text = problem.TrainingSamplesEnd.ToString();
      validationSamplesStartTextBox.Text = problem.ValidationSamplesStart.ToString();
      validationSamplesEndTextBox.Text = problem.ValidationSamplesEnd.ToString();
      testSamplesStartTextBox.Text = problem.TestSamplesStart.ToString();
      testSamplesEndTextBox.Text = problem.TestSamplesEnd.ToString();
      minTimeOffsetTextBox.Text = problem.MinTimeOffset.ToString();
      maxTimeOffsetTextBox.Text = problem.MaxTimeOffset.ToString();
      autoregressiveCheckBox.Checked = problem.AutoRegressive;
      switch (problem.LearningTask) {
        case LearningTask.Classification: classificationRadioButton.Checked = true; break;
        case LearningTask.Regression: regressionRadioButton.Checked = true; break;
        case LearningTask.TimeSeries: timeSeriesRadioButton.Checked = true; break;
      }
    }

    private void importButton_Click(object sender, EventArgs e) {
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
          Dataset dataset = (Dataset)problem.Dataset;
          dataset.Rows = parser.Rows;
          dataset.Columns = parser.Columns;
          dataset.Name = parser.ProblemName;
          dataset.Samples = new double[dataset.Rows * dataset.Columns];
          Array.Copy(parser.Samples, dataset.Samples, dataset.Columns * dataset.Rows);
          datasetView1.Dataset = problem.Dataset;

          problem.TrainingSamplesStart = parser.TrainingSamplesStart;
          problem.ValidationSamplesEnd = parser.TrainingSamplesStart;
          problem.TrainingSamplesEnd = parser.TrainingSamplesEnd;
          problem.ValidationSamplesStart = parser.ValidationSamplesStart;
          problem.ValidationSamplesEnd = parser.ValidationSamplesEnd;
          problem.TestSamplesStart = parser.TestSamplesStart;
          problem.TestSamplesEnd = parser.TestSamplesEnd;
          for (int i = 0; i < parser.VariableNames.Length; i++) {
            dataset.SetVariableName(i, parser.VariableNames[i]);
          }

          problem.FireChanged();
          Refresh();
        }
      }
    }

    private void autoregressiveCheckBox_CheckedChanged(object sender, EventArgs e) {
      problem.AutoRegressive = autoregressiveCheckBox.Checked;
      problem.FireChanged();
    }

    private void samplesTextBox_Validating(object sender, CancelEventArgs e) {
      try {
        int trainingStart = int.Parse(trainingSamplesStartTextBox.Text);
        int trainingEnd = int.Parse(trainingSamplesEndTextBox.Text);
        int validationStart = int.Parse(validationSamplesStartTextBox.Text);
        int validationEnd = int.Parse(validationSamplesEndTextBox.Text);
        int testStart = int.Parse(testSamplesStartTextBox.Text);
        int testEnd = int.Parse(testSamplesEndTextBox.Text);
        if (trainingStart < 0 || validationStart < 0 || testStart < 0 ||
          trainingEnd >= problem.Dataset.Rows || validationEnd >= problem.Dataset.Rows || testEnd >= problem.Dataset.Rows ||
          trainingStart >= trainingEnd ||
          validationStart >= validationEnd ||
          testStart >= testEnd ||
          IsOverlapping(trainingStart, trainingEnd, validationStart, validationEnd) ||
          IsOverlapping(trainingStart, trainingEnd, testStart, testEnd) ||
          IsOverlapping(validationStart, validationEnd, testStart, testEnd))
          ColorSamplesTextBoxes(Color.Red);
        else
          ColorSamplesTextBoxes(Color.White);
      }
      catch (FormatException ex) {
        ColorSamplesTextBoxes(Color.Red);
      }
    }

    private void samplesTextBox_Validated(object sender, EventArgs e) {
      problem.TrainingSamplesStart = int.Parse(trainingSamplesStartTextBox.Text);
      problem.TrainingSamplesEnd = int.Parse(trainingSamplesEndTextBox.Text);
      problem.ValidationSamplesStart = int.Parse(validationSamplesStartTextBox.Text);
      problem.ValidationSamplesEnd = int.Parse(validationSamplesEndTextBox.Text);
      problem.TestSamplesStart = int.Parse(testSamplesStartTextBox.Text);
      problem.TestSamplesEnd = int.Parse(testSamplesEndTextBox.Text);
      problem.FireChanged();
    }

    private void ColorSamplesTextBoxes(Color color) {
      trainingSamplesStartTextBox.BackColor = color;
      trainingSamplesEndTextBox.BackColor = color;
      validationSamplesStartTextBox.BackColor = color;
      validationSamplesEndTextBox.BackColor = color;
      testSamplesStartTextBox.BackColor = color;
      testSamplesEndTextBox.BackColor = color;
    }

    private bool IsOverlapping(int x0, int y0, int x1, int y1) {
      Trace.Assert(x0 <= y0 && x1 <= y1);
      int tmp;
      // make sure that x0,y0 is the left interval
      if (x1 < x0) {
        tmp = x1;
        x1 = x0;
        x0 = tmp;
        tmp = y1;
        y1 = y0;
        y0 = tmp;
      }
      return y0 > x1;
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

    private void radioButton_CheckedChanged(object sender, EventArgs e) {
      minTimeOffsetLabel.Enabled = timeSeriesRadioButton.Checked;
      minTimeOffsetTextBox.Enabled = timeSeriesRadioButton.Checked;
      maxTimeOffsetLabel.Enabled = timeSeriesRadioButton.Checked;
      maxTimeOffsetTextBox.Enabled = timeSeriesRadioButton.Checked;
      autoregressiveCheckBox.Enabled = timeSeriesRadioButton.Checked;
      autoregressiveLabel.Enabled = timeSeriesRadioButton.Checked;
      if (timeSeriesRadioButton.Checked) problem.LearningTask = LearningTask.TimeSeries;
      else if (classificationRadioButton.Checked) problem.LearningTask = LearningTask.Classification;
      else if (regressionRadioButton.Checked) problem.LearningTask = LearningTask.Regression;
      problem.FireChanged();
    }

    private void timeOffsetTextBox_Validating(object sender, CancelEventArgs e) {
      int min, max;
      e.Cancel = !int.TryParse(minTimeOffsetTextBox.Text, out min);
      e.Cancel = !int.TryParse(maxTimeOffsetTextBox.Text, out max);
      e.Cancel = min > max;
    }
    private void timeOffsetTextBox_Validated(object sender, EventArgs e) {
      problem.MinTimeOffset = int.Parse(minTimeOffsetTextBox.Text);
      problem.MaxTimeOffset = int.Parse(maxTimeOffsetTextBox.Text);
      problem.FireChanged();
    }
  }
}
