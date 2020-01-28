using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class IntRangeVM : RangeVM<int> {
    public override Type JsonItemType => typeof(IntArrayJsonItem); 
  }

  public class DoubleRangeVM : RangeVM<double> {
    public override Type JsonItemType => typeof(DoubleArrayJsonItem);
    public override JsonItemBaseControl GetControl() =>
      new JsonItemRangeControl(this);
  }

  public abstract class RangeVM<T> : JsonItemVMBase {

    public T MinValue {
      get => Cast(((Array)Item.Value).GetValue(0));
      set {
        SetValue(value, ((Array)Item.Value).GetValue(1));
        OnPropertyChange(this, nameof(MinValue));
      }
    }

    public T MaxValue {
      get => Cast(((Array)Item.Value).GetValue(1));
      set {
        SetValue(((Array)Item.Value).GetValue(0), value);
        OnPropertyChange(this, nameof(MaxValue));
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

    private T Cast(object obj) => (T)Convert.ChangeType(obj, typeof(T));

    private void SetValue(object min, object max) =>
      Item.Value = new object[] { min, max };

    private void SetRange(object min, object max) =>
      Item.Range = new object[] { min, max };
  }
}
