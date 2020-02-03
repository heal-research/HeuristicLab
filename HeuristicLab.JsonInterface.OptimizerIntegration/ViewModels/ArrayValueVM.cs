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

    protected override void OnItemChanged() {
      IList<IndexedArrayValueVM> list = new List<IndexedArrayValueVM>();
      var arr = ((DoubleArrayJsonItem)Item).Value;
      for(int i = 0; i < arr.Length; ++i) {
        list.Add(new IndexedArrayValueVM(arr, i));
      }
      Value = list.ToArray();
    }
  }

  public class IntArrayValueVM : ArrayValueVM<int> {
    public override Type JsonItemType => typeof(IntArrayJsonItem);

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override JsonItemBaseControl GetControl() => new JsonItemIntArrayValueControl(this);

    protected override void OnItemChanged() {
      IList<IndexedArrayValueVM> list = new List<IndexedArrayValueVM>();
      var arr = ((IntArrayJsonItem)Item).Value;
      for (int i = 0; i < arr.Length; ++i) {
        list.Add(new IndexedArrayValueVM(arr, i));
      }
      Value = list.ToArray();
    }
  }

  public abstract class ArrayValueVM<T> : RangedValueBaseVM<T> {

    public class IndexedArrayValueVM : INotifyPropertyChanged {
      private T[] arr;
      private int index;

      public IndexedArrayValueVM(T[] arr, int index) {
        this.arr = arr;
        this.index = index;
      }

      public T Value {
        get => arr[index];
        set {
          arr[index] = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
      }

      public event PropertyChangedEventHandler PropertyChanged;
    }

    public ArrayValueVM() {
      base.ItemChanged += OnItemChanged;
    }

    protected abstract void OnItemChanged();

    IndexedArrayValueVM[] vms;
    public IndexedArrayValueVM[] Value {
      get => vms; 
      set {
        vms = value;
        OnPropertyChange(this, nameof(Value));
      } 
    }
  }
}
