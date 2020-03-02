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

    public override JsonItemBaseControl Control =>
      new JsonItemBaseControl(this, new JsonItemIntValueControl(this));
  }

  public class DoubleValueVM : SingleValueVM<double> {
    public override Type JsonItemType => typeof(DoubleJsonItem);

    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;

    public override JsonItemBaseControl Control =>
       new JsonItemBaseControl(this, new JsonItemDoubleValueControl(this));
  }

  public class BoolValueVM : SingleValueVM<bool> {
    public override Type JsonItemType => typeof(BoolJsonItem);

    protected override bool MinTypeValue => false;
    protected override bool MaxTypeValue => true;

    public override JsonItemBaseControl Control =>
       new JsonItemBaseControl(this, new JsonItemBoolControl(this));
  }

  public abstract class SingleValueVM<T> : RangedValueBaseVM<T> {
    
    public T Value { 
      get => Cast(Item.Value);
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }
}
