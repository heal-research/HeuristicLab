using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Optimization.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.OneMax.Views {
  [View("OneMaxProblem View")]
  [Content(typeof(OneMaxProblem), true)]
  public partial class OneMaxProblemView : ProblemView {
    public new OneMaxProblem Content {
      get { return (OneMaxProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public OneMaxProblemView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public OneMaxProblemView(OneMaxProblem content)
      : this() {
      Content = content;
    }
  }
}
