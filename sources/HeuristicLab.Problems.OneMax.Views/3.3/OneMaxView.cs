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
  [View("OneMax View")]
  [Content(typeof(OneMax), true)]
  public partial class OneMaxView : ProblemView {
    public new OneMax Content {
      get { return (OneMax)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public OneMaxView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public OneMaxView(OneMax content)
      : this() {
      Content = content;
    }
  }
}
