using System;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class BoolValueVM : JsonItemVMBase<BoolJsonItem> {
    public override UserControl Control => null;
  }

  public abstract class SingleValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>
    where T : IComparable
    where JsonItemType : class, IIntervalRestrictedJsonItem<T>, IValueJsonItem<T> 
  { }
}
