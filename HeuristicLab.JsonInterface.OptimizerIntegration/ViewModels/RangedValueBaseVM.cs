using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public abstract class RangedValueBaseVM : RangedValueBaseVM<object> { }

  public abstract class RangedValueBaseVM<T> : JsonItemVMBase {
    public T MinRange {
      get => Cast(Item.Range?.First());
      set {
        SetRange(value, Item.Range?.Last());
        OnPropertyChange(this, nameof(MinRange));
      }
    }

    public T MaxRange {
      get => Cast(Item.Range?.Last());
      set {
        SetRange(Item.Range?.First(), value);
        OnPropertyChange(this, nameof(MaxRange));
      }
    }

    private bool enableMinRange = false;
    public bool EnableMinRange {
      get => enableMinRange;
      set {
        enableMinRange = value;
        if (!enableMinRange)
          MinRange = MinTypeValue;
        OnPropertyChange(this, nameof(EnableMinRange));
      }
    }

    private bool enableMaxRange = false;
    public bool EnableMaxRange {
      get => enableMaxRange;
      set {
        enableMaxRange = value;
        if (!enableMaxRange)
          MaxRange = MaxTypeValue;
        OnPropertyChange(this, nameof(EnableMaxRange));
      }
    }

    protected T Cast(object obj) => (obj==null) ? default(T) : (T)Convert.ChangeType(obj, typeof(T));

    private void SetRange(object min, object max) {
      object[] range = new object[] { min, max };
      Item.Range = range;
    }

    protected abstract T MinTypeValue { get; }
    protected abstract T MaxTypeValue { get; }
  }
}
