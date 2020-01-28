using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class IntValueVM : SingleValueVM<int> {
    public override Type JsonItemType => typeof(IntJsonItem);

    protected override int MinTypeValue => int.MinValue;
    protected override int MaxTypeValue => int.MaxValue;

    public override JsonItemBaseControl GetControl() =>
      new JsonItemIntValueControl(this);
  }

  public class DoubleValueVM : SingleValueVM<double> {
    public override Type JsonItemType => typeof(DoubleJsonItem);

    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;

    public override JsonItemBaseControl GetControl() =>
       new JsonItemDoubleValueControl(this);
  }

  public class BoolValueVM : SingleValueVM<bool> {
    public override Type JsonItemType => typeof(BoolJsonItem);

    protected override bool MinTypeValue => false;
    protected override bool MaxTypeValue => true;

    public override JsonItemBaseControl GetControl() =>
       new JsonItemBoolControl(this);
  }

  public abstract class SingleValueVM<T> : JsonItemVMBase {
    
    public T Value { 
      get => Cast(Item.Value);
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    public T MinRange {
      get => Cast(Item.Range.First());
      set {
        SetRange(value, Item.Range.Last());
        OnPropertyChange(this, nameof(MinRange));
      }
    }

    public T MaxRange {
      get => Cast(Item.Range.Last());
      set {
        SetRange(Item.Range.First(), value);
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

    private T Cast(object obj) => (T)Convert.ChangeType(obj, typeof(T));

    private void SetRange(object min, object max) {
      object[] range = new object[] { min, max };
      Item.Range = range;
    }

    protected abstract T MinTypeValue { get; }
    protected abstract T MaxTypeValue { get; }
  }
}
