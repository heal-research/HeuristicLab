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
      get => Item.Value.ToString();
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }
}
