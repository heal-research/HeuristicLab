using System;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public abstract class ArrayValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>, IArrayJsonItemVM
    where T : IComparable
    where JsonItemType : class, IArrayJsonItem, IIntervalRestrictedJsonItem<T> {

    public override UserControl Control => CompoundControl.Create(base.Control, ArrayJsonItemControl.Create(this));

    public ArrayValueVM() { }
    
    public bool Resizable { 
      get => Item.Resizable; 
      set {
        Item.Resizable = value;
        OnPropertyChange(this, nameof(IArrayJsonItemVM.Resizable));
      }
    }
  }
}
