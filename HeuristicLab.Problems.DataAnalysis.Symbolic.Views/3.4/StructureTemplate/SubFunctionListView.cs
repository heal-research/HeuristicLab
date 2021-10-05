using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public partial class SubFunctionListView : ItemListView<SubFunction> {
    public SubFunctionListView() {
      InitializeComponent();
    }
  }
}
