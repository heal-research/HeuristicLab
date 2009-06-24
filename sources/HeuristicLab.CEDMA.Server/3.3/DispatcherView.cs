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
    public DispatcherView(DispatcherBase dispatcher) {
      this.dispatcher = dispatcher;
      InitializeComponent();

      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();

      foreach (string targetVar in dispatcher.AllowedTargetVariables) {
        targetVariableList.Items.Add(targetVar);
      }

      foreach (string inputVar in dispatcher.AllowedInputVariables) {
        inputVariableList.Items.Add(inputVar);
      }
    }
  }
}
