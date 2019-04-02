using System;
using System.Windows.Forms;
using HeuristicLab.ExactOptimization.LinearProgramming;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.ExactOptimization.Views {

  [View(nameof(LinearProgrammingAlgorithmView))]
  [Content(typeof(LinearProgrammingAlgorithm), IsDefaultView = true)]
  public partial class LinearProgrammingAlgorithmView : BasicAlgorithmView {
    public LinearProgrammingAlgorithmView() {
      InitializeComponent();
    }

    protected override void newProblemButton_Click(object sender, EventArgs e) {
      ((LinearProblem)Content.Problem).ProblemDefinition = new ProgrammableLinearProblemDefinition();
    }

    protected override void openProblemButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Import Model";
      openFileDialog.Filter = "All Supported Files (*.mps;*.bin;*.prototxt)|*.mps;*.bin;*.prototxt|" +
                              "Mathematical Programming System Files (*.mps)|*.mps|" +
                              "Google OR-Tools Protocol Buffers Files (*.bin;*.prototxt)|*.bin;*.prototxt|" +
                              "All Files (*.*)|*.*";

      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newProblemButton.Enabled = openProblemButton.Enabled = false;
        problemViewHost.Enabled = false;

        try {
          ((LinearProblem)Content.Problem).ProblemDefinition = new FileBasedLinearProblemDefinition {
            FileName = openFileDialog.FileName
          };
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        } finally {
          problemViewHost.Enabled = true;
          newProblemButton.Enabled = openProblemButton.Enabled = true;
        }
      }
    }

    private void exportModelButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Export Model";
      saveFileDialog.Filter = "CPLEX LP File (*.lp)|*.lp|" +
                              "Mathematical Programming System File (*.mps)|*.mps|" +
                              "Google OR-Tools Protocol Buffers Text File (*.prototxt)|*.prototxt|" +
                              "Google OR-Tools Protocol Buffers Binary File (*.bin)|*.bin";

      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        ((LinearProblem)Content.Problem).ExportModel(saveFileDialog.FileName);
      }
    }

    private void showInRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null)
        ((LinearProblem)Content.Problem).ProblemDefinitionParameter.GetsCollected = showInRunCheckBox.Checked;
    }
  }
}
