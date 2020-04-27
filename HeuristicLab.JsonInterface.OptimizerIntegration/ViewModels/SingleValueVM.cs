using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class BoolValueVM : JsonItemVMBase<BoolJsonItem> {

    public bool Value { 
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
    
    public override UserControl Control =>
       new JsonItemBoolControl(this);
  }

  public abstract class SingleValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>
    where T : IComparable
    where JsonItemType : class, IIntervalRestrictedJsonItem<T>, IValueJsonItem<T> 
  {

    public T Value { 
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }
}
