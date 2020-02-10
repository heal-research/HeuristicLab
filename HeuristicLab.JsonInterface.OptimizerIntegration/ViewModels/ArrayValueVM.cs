using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class DoubleArrayValueVM : ArrayValueVM<double> {
    public override Type JsonItemType => typeof(DoubleArrayJsonItem);

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;

    public override JsonItemBaseControl GetControl() => new JsonItemDoubleArrayValueControl(this);
    
    public override double[] Value {
      get => ((DoubleArrayJsonItem)Item).Value;
      set {
        ((DoubleArrayJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public class IntArrayValueVM : ArrayValueVM<int> {
    public override Type JsonItemType => typeof(IntArrayJsonItem);

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override JsonItemBaseControl GetControl() => new JsonItemIntArrayValueControl(this);
    
    public override int[] Value {
      get => ((IntArrayJsonItem)Item).Value;
      set {
        ((IntArrayJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public abstract class ArrayValueVM<T> : RangedValueBaseVM<T> {
    
    public ArrayValueVM() {
      //base.ItemChanged += OnItemChanged;
    }

    public void SetIndexValue(object obj, int index) {
      T[] tmp = Value;
      if(index >= tmp.Length) { // increasing array
        T[] newArr = new T[index+1];
        Array.Copy(tmp, 0, newArr, 0, tmp.Length);
        tmp = newArr;
      }
      tmp[index] = (T)Convert.ChangeType(obj, typeof(T));
      Value = tmp;
    }

    public abstract T[] Value { get; set; }
  }
}
