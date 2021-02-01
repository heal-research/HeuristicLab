using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public abstract class ArrayValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>, IArrayJsonItemVM
    where T : IComparable
    where JsonItemType : class, IArrayJsonItem, IIntervalRestrictedJsonItem<T> {

    public override UserControl Control => CompoundControl.Create(base.Control, ArrayJsonItemControl.Create(this));

    public ArrayValueVM() { }
    
    public abstract T[] Value { get; set; }
    public bool Resizable { 
      get => Item.Resizable; 
      set {
        Item.Resizable = value;
        OnPropertyChange(this, nameof(IArrayJsonItemVM.Resizable));
      }
    }
  }
}
