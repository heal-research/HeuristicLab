using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class ResultItemVM : JsonItemVMBase<ResultJsonItem> {
    public override Type TargetedJsonItemType => typeof(ResultJsonItem);
    public override UserControl Control =>
      new JsonItemBaseControl(this);
  }
}
