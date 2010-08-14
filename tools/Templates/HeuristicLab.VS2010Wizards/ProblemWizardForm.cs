using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
    public string ProblemSpecificParameterProperties {
      get;
      private set;
    }
    public string ProblemSpecificProperties {
      get;
      private set;
    }
    public string ProblemSpecificParameterInitializers {
      get;
      private set;
    }

    public ProblemWizardForm() {
      InitializeComponent();
      IsSingleObjective = singleObjectiveCheckBox.Checked;
      IsMultiObjective = multiObjectiveCheckBox.Checked;
      EvaluatorType = evaluatorTypeTextBox.Text;
      SolutionCreatorType = solutionCreatorTypeTextBox.Text;
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
      IsMultiObjective = multiObjectiveCheckBox.Checked;
      ParameterProperties = parametersControl.GetParameterProperties("public");
      Properties = parametersControl.GetProperties("public");
      ParameterInitializers = parametersControl.GetInitializers();

      if (IsMultiObjective && IsSingleObjective) {
        ProblemTypeImplementation = "ISingleObjectiveProblem, IMultiObjectiveProblem";
      } else if (IsMultiObjective) {
        ProblemTypeImplementation = "IMultiObjectiveProblem";
      }  else if (IsSingleObjective)
        ProblemTypeImplementation = "ISingleObjectiveProblem";

      ProblemSpecificParameterProperties = GetParamProps();
      ProblemSpecificProperties = GetProps();
      ProblemSpecificParameterInitializers = GetParamInitializers();
    }

    private string GetParamProps() {
      StringBuilder builder = new StringBuilder();
      if (IsSingleObjective) {
        builder.Append(@"public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters[""BestKnownQuality""]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }");
        builder.Append(Environment.NewLine);
        builder.Append(@"public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters[""Maximization""]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }");
        builder.Append(Environment.NewLine);
      }
      if (IsMultiObjective) {
        builder.Append(@" public ValueParameter<BoolArray> MultiObjectiveMaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters[""MultiObjectiveMaximization""]; }
    }
    IParameter IMultiObjectiveProblem.MaximizationParameter {
      get { return MultiObjectiveMaximizationParameter; }
    }");
        builder.Append(Environment.NewLine);
      }
      builder.Append(@"public ValueParameter<" + SolutionCreatorType + @"> SolutionCreatorParameter {
      get { return (ValueParameter<" + SolutionCreatorType + @">)Parameters[""SolutionCreator""]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<" + EvaluatorType + @"> EvaluatorParameter {
      get { return (ValueParameter<" + EvaluatorType + @">)Parameters[""Evaluator""]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }");
      return builder.ToString();
    }

    private string GetProps() {
      StringBuilder builder = new StringBuilder();
      builder.Append(@"public " + SolutionCreatorType + @" SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public " + EvaluatorType + @" Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }");
      builder.Append(Environment.NewLine);
      if (IsSingleObjective) {
        builder.Append(@"ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }");
        builder.Append(Environment.NewLine);
      }
      if (IsMultiObjective) {
        builder.Append(@"IMultiObjectiveEvaluator IMultiObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }");
        builder.Append(Environment.NewLine);
      }
      builder.Append(@"IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }");
      builder.Append(Environment.NewLine);
      if (IsSingleObjective) {
        builder.Append(@"public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }");
        builder.Append(Environment.NewLine);
      }
      return builder.ToString();
    }

    private string GetParamInitializers() {
      StringBuilder builder = new StringBuilder();
      if (IsSingleObjective) {
        builder.Append(@"Parameters.Add(new ValueParameter<BoolValue>(""Maximization"", ""TODO: Set to the false for minimization, true for maximization and add a description."", new BoolValue(false)));");
        builder.Append(Environment.NewLine);
      }
      if (IsMultiObjective) {
        builder.Append("// TODO: Change the default value to the number of objectives." + Environment.NewLine);
        builder.Append(@"Parameters.Add(new ValueParameter<BoolArray>(""MultiObjectiveMaximization"", ""Set to false as the error of the regression model should be minimized."", new BoolArray(new bool[] {false, false})));");
        builder.Append(Environment.NewLine);
      }
      builder.Append("// TODO: Add a default solution creator." + Environment.NewLine);
      builder.Append("Parameters.Add(new ValueParameter<" + SolutionCreatorType + ">(\"SolutionCreator\", \"The operator which should be used to create new solutions.\", null));");
      builder.Append(Environment.NewLine);
      builder.Append("//TODO: Add a default evaluator." + Environment.NewLine);
      builder.Append("Parameters.Add(new ValueParameter<" + EvaluatorType + ">(\"Evaluator\", \"The operator which should be used to evaluate solutions.\", null));");
      builder.Append(Environment.NewLine);
      if (IsSingleObjective) {
        builder.Append("Parameters.Add(new OptionalValueParameter<DoubleValue>(\"BestKnownQuality\", \"The quality of the best known solution.\"));");
        builder.Append(Environment.NewLine);
      }
      return builder.ToString();
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

    private void singleObjectiveCheckBox_CheckedChanged(object sender, EventArgs e) {
      IsSingleObjective = singleObjectiveCheckBox.Checked;
      nextButton.Enabled = IsSingleObjective || IsMultiObjective;
      finishButton.Enabled = nextButton.Enabled;
    }
    private void multiObjectiveCheckBox_CheckedChanged(object sender, EventArgs e) {
      IsMultiObjective = multiObjectiveCheckBox.Checked;
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
