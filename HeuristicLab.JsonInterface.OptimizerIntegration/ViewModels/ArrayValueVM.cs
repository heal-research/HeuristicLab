using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class ArrayValueVM : JsonItemVMBase {
    public override Type JsonItemType => typeof(DoubleArrayJsonItem);
    public override JsonItemBaseControl GetControl() => new JsonItemArrayValueControl(this);
  }
}
