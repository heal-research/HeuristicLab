using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.Knapsack.Views {
  [View("KnapsackProblem View")]
  [Content(typeof(KnapsackProblem), true)]
  public partial class KnapsackProblemView : HeuristicLab.Optimization.Views.ProblemView {
    public new KnapsackProblem Content {
      get { return (KnapsackProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public KnapsackProblemView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public KnapsackProblemView(KnapsackProblem content)
      : this() {
      Content = content;
    }
  }
}
