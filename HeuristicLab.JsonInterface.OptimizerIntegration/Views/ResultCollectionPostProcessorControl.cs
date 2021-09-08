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

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ResultCollectionPostProcessorControl : Core.Views.CheckedItemListView<IResultCollectionProcessor> {
    public ResultCollectionPostProcessorControl() {
      InitializeComponent();
    }

    protected override string GroupBoxText => "Result Collection Processors";

  }
}
