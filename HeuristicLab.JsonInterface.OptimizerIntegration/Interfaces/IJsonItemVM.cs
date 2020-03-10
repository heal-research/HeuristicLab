using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public interface IJsonItemVM : INotifyPropertyChanged, IDisposable 
  {
    event Action ItemChanged;

    Type TargetedJsonItemType { get; }

    UserControl Control { get; }
    bool Selected { get; set; }

    string Name { get; set; }

    string Description { get; set; }
    
    TreeNode TreeNode { get; set; }

    TreeView TreeView { get; set; }
    IJsonItem Item { get; set; }

  }

  public interface IJsonItemVM<JsonItemType> : IJsonItemVM
    where JsonItemType : IJsonItem
  {
    new JsonItemType Item { get; set; }
  }
}
