using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class LookupJsonItemVM : JsonItemVMBase, ILookupJsonItemVM {
    public override Type JsonItemType => typeof(LookupJsonItem);

    public override UserControl Control => new LookupJsonItemControl(this);

    public string ActualName {
      get => ((ILookupJsonItem)Item).ActualName;
      set {
        ((ILookupJsonItem)Item).ActualName = value;
        OnPropertyChange(this, nameof(ActualName));
      }
    }
  }
}
