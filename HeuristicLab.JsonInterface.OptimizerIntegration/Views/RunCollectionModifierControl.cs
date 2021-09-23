using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class RunCollectionModifierControl : Core.Views.CheckedItemListView<IRunCollectionModifier> {
    public RunCollectionModifierControl() {
      InitializeComponent();
    }

    protected override string GroupBoxText => "Run Collection Modifiers";

  }
}
