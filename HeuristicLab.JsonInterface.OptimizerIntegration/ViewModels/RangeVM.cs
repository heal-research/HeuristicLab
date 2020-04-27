using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public abstract class RangeVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>
    where T : IComparable 
    where JsonItemType : class, IRangedJsonItem<T>
  {

    public T MinValue {
      get => Item.MinValue;
      set {
        Item.MinValue = value;
        OnPropertyChange(this, nameof(MinValue));
      }
    }

    public T MaxValue {
      get => Item.MaxValue;
      set {
        Item.MaxValue = value;
        OnPropertyChange(this, nameof(MaxValue));
      }
    }
  }
}
