using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class DispatcherView : ViewBase {
    private SimpleDispatcher dispatcher;
    public DispatcherView(SimpleDispatcher dispatcher)
      : base() {
      this.dispatcher = dispatcher;
      InitializeComponent();
      UpdateControls();
      dispatcher.Changed += (sender, args) => UpdateControls();
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

        foreach (string inputVar in dispatcher.InputVariables) {
          inputVariableList.Items.Add(inputVar, false);
        }

        foreach (HeuristicLab.Modeling.IAlgorithm algo in dispatcher.Algorithms) {
          algorithmsListBox.Items.Add(algo, false);
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
      string selectedTarget = (string)targetVariableList.SelectedItem;
      if (e.NewValue == CheckState.Checked) {
        dispatcher.EnableInputVariable(selectedTarget, (string)inputVariableList.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        dispatcher.DisableInputVariable(selectedTarget, (string)inputVariableList.Items[e.Index]);
      }
    }

    private void targetVariableList_SelectedValueChanged(object sender, EventArgs e) {
      string selectedTarget = (string)targetVariableList.SelectedItem;
      UpdateInputVariableList(selectedTarget);
    }

    private void UpdateInputVariableList(string target) {
      inputVariableList.Items.Clear();
      var activatedInputVariables = dispatcher.GetInputVariables(target);
      foreach (string inputVar in dispatcher.InputVariables) {
        inputVariableList.Items.Add(inputVar, activatedInputVariables.Contains(inputVar));
      }
      inputVariableList.Enabled = true;
    }

    private void setAllButton_Click(object sender, EventArgs e) {      
      foreach (string targetVar in dispatcher.TargetVariables) {
        for (int i = 0; i < inputVariableList.Items.Count; i++) {
          if (inputVariableList.GetItemChecked(i)) {
            dispatcher.EnableInputVariable(targetVar, (string)inputVariableList.Items[i]);
          } else {
            dispatcher.DisableInputVariable(targetVar, (string)inputVariableList.Items[i]);
          }
        }
      }
    }

    private void algorithmsListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      if(e.NewValue == CheckState.Checked) {
        dispatcher.EnableAlgorithm((HeuristicLab.Modeling.IAlgorithm)algorithmsListBox.Items[e.Index]);
      } else if(e.NewValue == CheckState.Unchecked) {
        dispatcher.DisableAlgorithm((HeuristicLab.Modeling.IAlgorithm)algorithmsListBox.Items[e.Index]);
      }
    }
  }
}
