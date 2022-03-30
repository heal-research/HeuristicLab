using System;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class ValueLookupJsonItemVM : LookupJsonItemVM, IValueLookupJsonItemVM {
    public override Type TargetedJsonItemType => typeof(ValueLookupJsonItem);
    public override UserControl Control => ValueLookupJsonItemControl.Create(this);
    public IJsonItem JsonItemReference => ((IValueLookupJsonItem)Item).ActualValue;

    public ValueLookupJsonItemVM() {
      base.SelectedChanged += () => {
        if (JsonItemReference != null)
          JsonItemReference.Active = base.Selected;
      };
    }
  }
}
