using System;
using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ProblemWizardForm : Form {
    public string ProblemName {
      get;
      private set;
    }
    public string ProblemDescription {
      get;
      private set;
    }
    public bool IsSingleObjective {
      get;
      private set;
    }
    public bool IsMultiObjective {
      get;
      private set;
    }
    public string ParameterProperties {
      get;
      private set;
    }
    public string Properties {
      get;
      private set;
    }
    public string ParameterInitializers {
      get;
      private set;
    }
    public string EvaluatorType {
      get;
      private set;
    }
    public string SolutionCreatorType {
      get;
      private set;
    }
    public string ProblemTypeImplementation {
      get;
      private set;
    }

    public ProblemWizardForm() {
      InitializeComponent();
      IsSingleObjective = singleObjectiveRadioButton.Checked;
      IsMultiObjective = multiObjectiveRadioButton.Checked;
      EvaluatorType = evaluatorTypeTextBox.Text;
      SolutionCreatorType = solutionCreatorTypeTextBox.Text;
      nextButton.Enabled = IsSingleObjective || IsMultiObjective;
      finishButton.Enabled = nextButton.Enabled;
    }

    private void finishButton_Click(object sender, System.EventArgs e) {
      SetProperties();
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void SetProperties() {
      ProblemName = problemNameTextBox.Text;
      ProblemDescription = problemDescriptionTextBox.Text;
      IsSingleObjective = singleObjectiveRadioButton.Checked;
      IsMultiObjective = multiObjectiveRadioButton.Checked;
      ParameterProperties = parametersControl.GetParameterProperties("public");
      Properties = parametersControl.GetProperties("public");
      ParameterInitializers = parametersControl.GetInitializers();

      if (IsSingleObjective) {
        ProblemTypeImplementation = "SingleObjectiveProblem<" + EvaluatorType + ", " + SolutionCreatorType + ">";
      } else if (IsMultiObjective) {
        ProblemTypeImplementation = "MultiObjectiveProblem<" + EvaluatorType + ", " + SolutionCreatorType + ">";
      }
    }

    private void nextButton_Click(object sender, EventArgs e) {
      page1Panel.Visible = false;
      page2Panel.Visible = true;
      nextButton.Enabled = false;
      previousButton.Enabled = true;
    }

    private void previousButton_Click(object sender, EventArgs e) {
      page2Panel.Visible = false;
      page1Panel.Visible = true;
      previousButton.Enabled = false;
      nextButton.Enabled = true;
    }

    private void singleObjectiveRadioButton_CheckedChanged(object sender, EventArgs e) {
      IsSingleObjective = singleObjectiveRadioButton.Checked;
      nextButton.Enabled = IsSingleObjective || IsMultiObjective;
      finishButton.Enabled = nextButton.Enabled;
    }
    private void multiObjectiveRadioButton_CheckedChanged(object sender, EventArgs e) {
      IsMultiObjective = multiObjectiveRadioButton.Checked;
      nextButton.Enabled = IsSingleObjective || IsMultiObjective;
      finishButton.Enabled = nextButton.Enabled;
    }
    private void problemNameTextBox_TextChanged(object sender, EventArgs e) {
      ProblemName = problemNameTextBox.Text.Trim();
      nextButton.Enabled = ProblemName != string.Empty;
    }
    private void evaluatorTypeTextBox_TextChanged(object sender, EventArgs e) {
      EvaluatorType = evaluatorTypeTextBox.Text.Trim();
      nextButton.Enabled = EvaluatorType != string.Empty;
    }
    private void solutionCreatorTypeTextBox_TextChanged(object sender, EventArgs e) {
      SolutionCreatorType = solutionCreatorTypeTextBox.Text.Trim();
      nextButton.Enabled = SolutionCreatorType != string.Empty;
    }
  }
}
