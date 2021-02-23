using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class LookupJsonItemVM : JsonItemVMBase<LookupJsonItem>, ILookupJsonItemVM {
    public override UserControl Control => null;
  }
}
