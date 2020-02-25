using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class ResultItemVM : JsonItemVMBase {
    public override Type JsonItemType => typeof(ResultJsonItem);
    public override JsonItemBaseControl Control =>
      new JsonItemBaseControl(this);


  }
}
