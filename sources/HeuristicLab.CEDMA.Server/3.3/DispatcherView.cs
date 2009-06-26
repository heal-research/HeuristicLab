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
    private DispatcherBase dispatcher;
    public DispatcherView(DispatcherBase dispatcher) : base() {
      this.dispatcher = dispatcher;
      InitializeComponent();
      UpdateControls();
      dispatcher.Changed += (sender, args) => UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();

      foreach (string targetVar in dispatcher.TargetVariables) {
        targetVariableList.Items.Add(targetVar);
      }

      foreach (string inputVar in dispatcher.InputVariables) {
        inputVariableList.Items.Add(inputVar);
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
        dispatcher.EnableInputVariable((string)inputVariableList.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        dispatcher.DisableInputVariable((string)inputVariableList.Items[e.Index]);
      }
    }
  }
}
