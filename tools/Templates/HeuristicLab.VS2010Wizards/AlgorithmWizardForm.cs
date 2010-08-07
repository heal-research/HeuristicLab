using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class AlgorithmWizardForm : Form {
    public string AlgorithmName {
      get;
      private set;
    }
    public string AlgorithmDescription {
      get;
      private set;
    }
    public bool IsMultiObjective {
      get;
      private set;
    }

    public AlgorithmWizardForm() {
      InitializeComponent();
      AlgorithmName = algorithmNameTextBox.Text;
      AlgorithmDescription = algorithmDescriptionTextBox.Text;
      IsMultiObjective = isMultiObjectiveCheckBox.Checked;
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void algorithmNameTextBox_TextChanged(object sender, EventArgs e) {
      AlgorithmName = algorithmNameTextBox.Text;
    }

    private void algorithmDescriptionTextBox_TextChanged(object sender, EventArgs e) {
      AlgorithmDescription = algorithmDescriptionTextBox.Text;
    }

    private void isMultiObjectiveCheckBox_CheckedChanged(object sender, EventArgs e) {
      IsMultiObjective = isMultiObjectiveCheckBox.Checked;
    }
  }
}
