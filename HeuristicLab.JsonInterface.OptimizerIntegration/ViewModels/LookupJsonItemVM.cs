using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class LookupJsonItemVM : JsonItemVMBase<LookupJsonItem>, ILookupJsonItemVM {

    public override UserControl Control => new LookupJsonItemControl(this);

    public string ActualName {
      get => Item.ActualName;
      set {
        Item.ActualName = value;
        OnPropertyChange(this, nameof(ActualName));
      }
    }
  }
}
