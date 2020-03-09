using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class IntRangeVM : RangeVM<int> {
    public override Type JsonItemType => typeof(IntRangeJsonItem);

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemRangeControl(this);
  }

  public class DoubleRangeVM : RangeVM<double> {
    public override Type JsonItemType => typeof(DoubleRangeJsonItem);

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
      new JsonItemRangeControl(this);
  }

  public abstract class RangeVM<T> : RangedValueBaseVM<T> {

    public T MinValue {
      get => Cast(((Array)Item.Value).GetValue(0));
      set {
        SetValue(value, (T)((Array)Item.Value).GetValue(1));
        OnPropertyChange(this, nameof(MinValue));
      }
    }

    public T MaxValue {
      get => Cast(((Array)Item.Value).GetValue(1));
      set {
        SetValue((T)((Array)Item.Value).GetValue(0), value);
        OnPropertyChange(this, nameof(MaxValue));
      }
    }

    private void SetValue(T min, T max) =>
      Item.Value = new T[] { min, max };
  }
}
