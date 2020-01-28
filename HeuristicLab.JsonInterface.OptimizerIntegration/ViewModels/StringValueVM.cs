using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class StringValueVM : JsonItemVMBase {
    public override Type JsonItemType => typeof(StringJsonItem);
    public override JsonItemBaseControl GetControl() =>
       new JsonItemValidValuesControl(this);

    public string Value {
      get => base.Item.Value.ToString();
      set {
        base.Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }
}
