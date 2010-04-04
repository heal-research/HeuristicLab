using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Optimization.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [View("SymbolicRegressionProblem View")]
  [Content(typeof(SymbolicRegressionProblem), true)]
  public partial class SymbolicRegressionProblemView : RegressionProblemView {
    public new SymbolicRegressionProblem Content {
      get { return (SymbolicRegressionProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SymbolicRegressionProblemView"/>.
    /// </summary>
    public SymbolicRegressionProblemView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="SymbolicRegressionProblemView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public SymbolicRegressionProblemView(SymbolicRegressionProblem content)
      : this() {
      Content = content;
    }
  }
}
