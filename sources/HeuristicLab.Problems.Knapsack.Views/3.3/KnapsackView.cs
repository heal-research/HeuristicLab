using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.Knapsack.Views {
  [View("Knapsack View")]
  [Content(typeof(Knapsack), true)]
  public partial class KnapsackView : HeuristicLab.Optimization.Views.ProblemView {
    public new Knapsack Content {
      get { return (Knapsack)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public KnapsackView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public KnapsackView(Knapsack content)
      : this() {
      Content = content;
    }
  }
}
