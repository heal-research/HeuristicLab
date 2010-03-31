using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Optimization.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ArtificialAnt.Views {
  [View("ArtificialAntProblem View")]
  [Content(typeof(ArtificialAntProblem), true)]
  public partial class ArtificialAntProblemView : ProblemView {
    public new ArtificialAntProblem Content {
      get { return (ArtificialAntProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ArtificialAntProblemView"/>.
    /// </summary>
    public ArtificialAntProblemView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ArtificialAntProblemView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public ArtificialAntProblemView(ArtificialAntProblem content)
      : this() {
      Content = content;
    }
  }
}
