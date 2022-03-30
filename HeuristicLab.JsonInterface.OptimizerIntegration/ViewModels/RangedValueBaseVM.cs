using System;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public abstract class RangedValueBaseVM<T, JsonItemType> : JsonItemVMBase<JsonItemType>
    where T : IComparable
    where JsonItemType : IntervalRestrictedJsonItem<T> 
  {
    public override UserControl Control => NumericRangeControl.Create(this);

    public T MinRange {
      get => Item.Minimum;
      set {
        Item.Minimum = value;
        OnPropertyChange(this, nameof(MinRange));
      }
    }

    public T MaxRange {
      get => Item.Maximum;
      set {
        Item.Maximum = value;
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
    
    protected abstract T MinTypeValue { get; }
    protected abstract T MaxTypeValue { get; }
  }
}
