using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class IntValueVM : SingleValueVM<int, IntJsonItem> {
    public override Type TargetedJsonItemType => typeof(IntJsonItem);

    protected override int MinTypeValue => int.MinValue;
    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemIntValueControl(this);
  }

  public class DoubleValueVM : SingleValueVM<double, DoubleJsonItem> {
    public override Type TargetedJsonItemType => typeof(DoubleJsonItem);

    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
       new JsonItemDoubleValueControl(this);
  }

  public class BoolValueVM : JsonItemVMBase<BoolJsonItem> {
    public override Type TargetedJsonItemType => typeof(BoolJsonItem);

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
