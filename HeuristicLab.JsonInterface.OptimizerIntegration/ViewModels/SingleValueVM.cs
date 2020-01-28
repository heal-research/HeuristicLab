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
    public override JsonItemBaseControl GetControl() =>
      new JsonItemIntValueControl(this);
  }

  public class DoubleValueVM : SingleValueVM<double> {
    public override Type JsonItemType => typeof(DoubleJsonItem); 
    public override JsonItemBaseControl GetControl() =>
       new JsonItemDoubleValueControl(this);
  }

  public class BoolValueVM : SingleValueVM<bool> {
    public override Type JsonItemType => typeof(BoolJsonItem);
    public override JsonItemBaseControl GetControl() =>
       new JsonItemBoolControl(this);
  }

  public abstract class SingleValueVM<T> : JsonItemVM {
    
    public T Value { 
      get => Cast(base.Item.Value);
      set {
        base.Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    public T MinRange {
      get => Cast(base.Item.Range.First());
      set {
        SetRange(value, base.Item.Range.Last());
        OnPropertyChange(this, nameof(MinRange));
      }
    }

    public T MaxRange {
      get => Cast(base.Item.Range.Last());
      set {
        SetRange(base.Item.Range.First(), value);
        OnPropertyChange(this, nameof(MaxRange));
      }
    }

    private T Cast(object obj) => (T)Convert.ChangeType(obj, typeof(T));

    private void SetRange(object min, object max) {
      object[] range = new object[] { min, max };
      base.Item.Range = range;
    }

  }
}
