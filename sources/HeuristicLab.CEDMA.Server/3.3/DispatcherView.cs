using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using System.Diagnostics;

namespace HeuristicLab.CEDMA.Server {
  public partial class DispatcherView : ViewBase {
    private SimpleDispatcher dispatcher;
    private ProblemSpecification selectedSpec;
    public DispatcherView(SimpleDispatcher dispatcher)
      : base() {
      this.dispatcher = dispatcher;
      InitializeComponent();
      dispatcher.Changed += (sender, args) => UpdateControls();
      UpdateControls();
      this.inputVariableList.CheckOnClick = true;
    }

    protected override void UpdateControls() {
      if (InvokeRequired) {
        Invoke((Action)UpdateControls);
      } else {
        base.UpdateControls();
        algorithmsListBox.Items.Clear();
        targetVariableList.Items.Clear();
        inputVariableList.Items.Clear();

        foreach (string targetVar in dispatcher.TargetVariables) {
          targetVariableList.Items.Add(targetVar, false);
        }

        foreach (string inputVar in dispatcher.Variables) {
          inputVariableList.Items.Add(inputVar, false);
        }

        targetVariableList.ClearSelected();
        inputVariableList.Enabled = false;
      }
    }

    private void targetVariableList_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (e.NewValue == CheckState.Checked) {
        dispatcher.EnableTargetVariable((string)targetVariableList.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        dispatcher.DisableTargetVariable((string)targetVariableList.Items[e.Index]);
      }
    }

    private void inputVariableList_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (e.NewValue == CheckState.Checked) {
        selectedSpec.AddInputVariable((string)inputVariableList.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        selectedSpec.RemoveInputVariable((string)inputVariableList.Items[e.Index]);
      }
    }

    private void targetVariableList_SelectedValueChanged(object sender, EventArgs e) {
      string selectedTarget = (string)targetVariableList.SelectedItem;
      selectedSpec = dispatcher.GetProblemSpecification(selectedTarget);
      UpdateInputVariableList();
      UpdateAlgorithms();
    }

    private void UpdateAlgorithms() {
      learningTaskGroupBox.Enabled = true;
      algorithmsListBox.Enabled = true;
      switch (selectedSpec.LearningTask) {
        case LearningTask.Classification: {
            classificationRadioButton.Checked = true;
            break;
          }
        case LearningTask.Regression: {
            regressionRadioButton.Checked = true;
            break;
          }
        case LearningTask.TimeSeries: {
            timeSeriesRadioButton.Checked = true;
            break;
          }
        default: { break; }
      }
      algorithmsListBox.Items.Clear();
      foreach (HeuristicLab.Modeling.IAlgorithm algo in dispatcher.GetAlgorithms(selectedSpec.LearningTask)) {
        algorithmsListBox.Items.Add(algo, dispatcher.GetAllowedAlgorithms(selectedSpec.TargetVariable).Contains(algo));
      }
      UpdateAlgorithmConfiguration();
    }

    private void UpdateAlgorithmConfiguration() {
      partitioningGroupBox.Enabled = true;
      trainingSamplesStartTextBox.Text = selectedSpec.TrainingSamplesStart.ToString();
      trainingSamplesEndTextBox.Text = selectedSpec.TrainingSamplesEnd.ToString();
      validationSamplesStartTextBox.Text = selectedSpec.ValidationSamplesStart.ToString();
      validationSamplesEndTextBox.Text = selectedSpec.ValidationSamplesEnd.ToString();
      testSamplesStartTextBox.Text = selectedSpec.TestSamplesStart.ToString();
      testSamplesEndTextBox.Text = selectedSpec.TestSamplesEnd.ToString();
    }

    private void UpdateInputVariableList() {
      inputVariableList.Items.Clear();
      var activatedInputVariables = selectedSpec.InputVariables;
      foreach (string inputVar in dispatcher.Variables) {
        inputVariableList.Items.Add(inputVar, activatedInputVariables.Contains(inputVar));
      }
      inputVariableList.Enabled = true;
    }

    private void setAllButton_Click(object sender, EventArgs e) {
      foreach (string targetVar in dispatcher.TargetVariables) {
        ProblemSpecification spec = dispatcher.GetProblemSpecification(targetVar);
        for (int i = 0; i < inputVariableList.Items.Count; i++) {
          if (inputVariableList.GetItemChecked(i)) {
            spec.AddInputVariable((string)inputVariableList.Items[i]);
          } else {
            spec.RemoveInputVariable((string)inputVariableList.Items[i]);
          }
        }
      }
    }

    private void algorithmsListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (e.NewValue == CheckState.Checked) {
        dispatcher.EnableAlgorithm(selectedSpec.TargetVariable, (HeuristicLab.Modeling.IAlgorithm)algorithmsListBox.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        dispatcher.DisableAlgorithm(selectedSpec.TargetVariable, (HeuristicLab.Modeling.IAlgorithm)algorithmsListBox.Items[e.Index]);
      }
    }

    private void radioButton_CheckedChanged(object sender, EventArgs e) {
      string selectedTarget = (string)targetVariableList.SelectedItem;
      minTimeOffsetLabel.Enabled = timeSeriesRadioButton.Checked;
      minTimeOffsetTextBox.Enabled = timeSeriesRadioButton.Checked;
      maxTimeOffsetLabel.Enabled = timeSeriesRadioButton.Checked;
      maxTimeOffsetTextBox.Enabled = timeSeriesRadioButton.Checked;
      autoregressiveCheckBox.Enabled = timeSeriesRadioButton.Checked;
      autoregressiveLabel.Enabled = timeSeriesRadioButton.Checked;
      if (timeSeriesRadioButton.Checked) selectedSpec.LearningTask = LearningTask.TimeSeries;
      else if (classificationRadioButton.Checked) selectedSpec.LearningTask = LearningTask.Classification;
      else if (regressionRadioButton.Checked) selectedSpec.LearningTask = LearningTask.Regression;
      UpdateAlgorithms();
    }

    private void timeOffsetTextBox_Validating(object sender, CancelEventArgs e) {
      int min, max;
      e.Cancel = !int.TryParse(minTimeOffsetTextBox.Text, out min);
      e.Cancel = !int.TryParse(maxTimeOffsetTextBox.Text, out max);
      e.Cancel = min > max;
    }

    private void timeOffsetTextBox_Validated(object sender, EventArgs e) {
      selectedSpec.MinTimeOffset = int.Parse(minTimeOffsetTextBox.Text);
      selectedSpec.MaxTimeOffset = int.Parse(maxTimeOffsetTextBox.Text);
    }

    private void samplesTextBox_Validated(object sender, EventArgs e) {
      selectedSpec.TrainingSamplesStart = int.Parse(trainingSamplesStartTextBox.Text);
      selectedSpec.TrainingSamplesEnd = int.Parse(trainingSamplesEndTextBox.Text);
      selectedSpec.ValidationSamplesStart = int.Parse(validationSamplesStartTextBox.Text);
      selectedSpec.ValidationSamplesEnd = int.Parse(validationSamplesEndTextBox.Text);
      selectedSpec.TestSamplesStart = int.Parse(testSamplesStartTextBox.Text);
      selectedSpec.TestSamplesEnd = int.Parse(testSamplesEndTextBox.Text);
    }

    private void ColorSamplesTextBoxes(Color color) {
      trainingSamplesStartTextBox.BackColor = color;
      trainingSamplesEndTextBox.BackColor = color;
      validationSamplesStartTextBox.BackColor = color;
      validationSamplesEndTextBox.BackColor = color;
      testSamplesStartTextBox.BackColor = color;
      testSamplesEndTextBox.BackColor = color;
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
          trainingEnd >= selectedSpec.Dataset.Rows || validationEnd >= selectedSpec.Dataset.Rows || testEnd >= selectedSpec.Dataset.Rows ||
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
      catch (FormatException) {
        ColorSamplesTextBoxes(Color.Red);
      }
    }

    private void autoregressiveCheckBox_CheckedChanged(object sender, EventArgs e) {
      selectedSpec.AutoRegressive = autoregressiveCheckBox.Checked;
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
  }
}
