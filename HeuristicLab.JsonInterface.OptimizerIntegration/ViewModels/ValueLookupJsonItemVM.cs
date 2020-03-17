using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class ValueLookupJsonItemVM : LookupJsonItemVM, IValueLookupJsonItemVM {
    public override Type TargetedJsonItemType => typeof(ValueLookupJsonItem);
    public override UserControl Control => new ValueLookupJsonItemControl(this);
    public IJsonItem JsonItemReference => ((IValueLookupJsonItem)Item).ActualValue;

    public ValueLookupJsonItemVM() {
      base.SelectedChanged += () => {
        if (JsonItemReference != null)
          JsonItemReference.Active = base.Selected;
      };
    }
  }
}
