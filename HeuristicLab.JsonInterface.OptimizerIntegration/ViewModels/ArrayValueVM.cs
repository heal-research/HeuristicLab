﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class DoubleArrayValueVM : ArrayValueVM<double, DoubleArrayJsonItem> {
    public override Type TargetedJsonItemType => typeof(DoubleArrayJsonItem);

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
      new JsonItemDoubleArrayValueControl(this);
    
    public override double[] Value {
      get => ((DoubleArrayJsonItem)Item).Value;
      set {
        ((DoubleArrayJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public class IntArrayValueVM : ArrayValueVM<int, IntArrayJsonItem> {
    public override Type TargetedJsonItemType => typeof(IntArrayJsonItem);

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemBaseControl(this, new JsonItemIntArrayValueControl(this));
    
    public override int[] Value {
      get => ((IntArrayJsonItem)Item).Value;
      set {
        ((IntArrayJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public abstract class ArrayValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>, IArrayJsonItemVM
    where T : IComparable
    where JsonItemType : class, IArrayJsonItem, IIntervalRestrictedJsonItem<T> {
    
    public ArrayValueVM() { }

    public void SetIndexValue(T data, int index) {
      T[] tmp = Value;
      if(index >= tmp.Length) { // increasing array
        T[] newArr = new T[index+1];
        Array.Copy(tmp, 0, newArr, 0, tmp.Length);
        tmp = newArr;
      }
      tmp[index] = data;
      Value = tmp;
    }

    public abstract T[] Value { get; set; }
    public bool Resizable { 
      get => ((IArrayJsonItem)Item).Resizable; 
      set {
        ((IArrayJsonItem)Item).Resizable = value;
        OnPropertyChange(this, nameof(IArrayJsonItemVM.Resizable));
      }
    }
  }
}